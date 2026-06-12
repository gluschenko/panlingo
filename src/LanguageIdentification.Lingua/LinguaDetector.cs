using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Panlingo.LanguageIdentification.Lingua.Internal;
using Panlingo.LanguageIdentification.Lingua.Native;

namespace Panlingo.LanguageIdentification.Lingua
{
    /// <summary>
    /// <inheritdoc cref="LinguaDetectorBuilder"/>
    /// </summary>
    public class LinguaDetector : IDisposable
    {
        private readonly Lazy<ImmutableHashSet<LinguaLanguage>> _labels;

        private IntPtr _detector;
        private bool _disposed = false;
        private int _activeOperations = 0;
        private readonly object _lifetimeLock = new object();

        internal LinguaDetector(IntPtr builder)
        {
            if (!IsSupported())
            {
                throw new NotSupportedException(
                    $"{nameof(LinguaDetector)} is not yet supported on {RuntimeInformation.RuntimeIdentifier} ({RuntimeInformation.OSArchitecture})"
                );
            }

            _detector = LinguaDetectorWrapper.LinguaLanguageDetectorCreate(builder);
            if (_detector == IntPtr.Zero)
            {
                throw new LinguaDetectorException($"Failed to create {nameof(LinguaDetector)}");
            }

            _labels = new Lazy<ImmutableHashSet<LinguaLanguage>>(
                () =>
                {
                    var result = ImmutableHashSet.CreateRange(Enum.GetValues<LinguaLanguage>());
                    return result;
                }
            );
        }

        /// <summary>
        /// Checks the suitability of the current platform for use. Key criteria are the operating system and processor architecture
        /// </summary>
        public static bool IsSupported()
        {
            return LinguaNativeLibrary.IsSupported();
        }

        /// <summary>
        /// Produces a prediction for 'text'
        /// </summary>
        /// <param name="text">Some text in natural language</param>
        /// <returns>Language prediction</returns>
        /// <exception cref="LinguaDetectorException"></exception>
        public IEnumerable<LinguaPrediction> PredictLanguages(string text, int count = 10)
        {
            var textBytes = LinguaDetectorWrapper.EncodeText(text);
            var detector = AcquireDetector();
            try
            {
                var status = LinguaDetectorWrapper.LinguaDetectSingle(
                    detector: detector,
                    text: textBytes,
                    textLength: (UIntPtr)textBytes.Length,
                    result: out var nativeResult
                );

                if (status == LinguaStatus.DetectFailure)
                {
                    return Array.Empty<LinguaPrediction>();
                }

                CheckNativeStatus(status);

                try
                {
                    var result = new LinguaPredictionResult[nativeResult.PredictionsCount];
                    var structSize = Marshal.SizeOf(typeof(LinguaPredictionResult));

                    for (var i = 0; i < nativeResult.PredictionsCount; i++)
                    {
                        result[i] = Marshal.PtrToStructure<LinguaPredictionResult>(nativeResult.Predictions + i * structSize);
                    }

                    return result
                        .Take(count)
                        .Where(x => x.Confidence > 0)
                        .OrderByDescending(x => x.Confidence)
                        .Select(x => new LinguaPrediction(x))
                        .ToArray();
                }
                finally
                {
                    LinguaDetectorWrapper.LinguaPredictionResultDestroy(nativeResult.Predictions, nativeResult.PredictionsCount);
                }
            }
            finally
            {
                ReleaseDetector();
            }
        }

        /// <summary>
        /// Produces a prediction for 'text'
        /// </summary>
        /// <param name="text">Some text in natural language</param>
        /// <returns>List of language predictions</returns>
        /// <exception cref="LinguaDetectorException"></exception>
        public IEnumerable<LinguaPredictionRange> PredictMixedLanguages(string text)
        {
            var textBytes = LinguaDetectorWrapper.EncodeText(text);
            var detector = AcquireDetector();
            try
            {
                var status = LinguaDetectorWrapper.LinguaDetectMixed(
                    detector: detector,
                    text: textBytes,
                    textLength: (UIntPtr)textBytes.Length,
                    result: out var nativeResult
                );

                if (status == LinguaStatus.DetectFailure)
                {
                    return Array.Empty<LinguaPredictionRange>();
                }

                CheckNativeStatus(status);

                try
                {
                    var result = new LinguaPredictionRangeResult[nativeResult.PredictionsCount];
                    var structSize = Marshal.SizeOf(typeof(LinguaPredictionRangeResult));

                    for (var i = 0; i < nativeResult.PredictionsCount; i++)
                    {
                        result[i] = Marshal.PtrToStructure<LinguaPredictionRangeResult>(nativeResult.Predictions + i * structSize);
                    }

                    return result
                        .OrderByDescending(x => x.Confidence)
                        .Select(x => new LinguaPredictionRange(x, textBytes))
                        .ToArray();
                }
                finally
                {
                    LinguaDetectorWrapper.LinguaPredictionRangeResultDestroy(nativeResult.Predictions, nativeResult.PredictionsCount);
                }
            }
            finally
            {
                ReleaseDetector();
            }
        }

        /// <summary>
        /// Converts <see cref="LinguaLanguage"/> to ISO 639-1 or ISO 639-3 string.
        /// </summary>
        /// <param name="language"></param>
        /// <returns>Language code according to ISO 639-1 or ISO 639-3</returns>
        /// <exception cref="LinguaDetectorException"></exception>
        public string GetLanguageCode(LinguaLanguage language, LinguaLanguageCode code)
        {
            CheckDisposed();

            if (!Enum.IsDefined(typeof(LinguaLanguage), language))
            {
                throw new LinguaDetectorException($"Language code '{language}' is not found");
            }

            if (!Enum.IsDefined(typeof(LinguaLanguageCode), code))
            {
                throw new LinguaDetectorException($"Language code type '{code}' is not found");
            }

            return LinguaDetectorWrapper.LinguaLangCode(language, code);
        }

        /// <summary>
        /// Gets all languages supported by Lingua
        /// </summary>
        /// <returns>Collection of strings</returns>
        public IEnumerable<LinguaLanguage> GetLanguages()
        {
            CheckDisposed();
            return _labels.Value;
        }

        private void CheckDisposed()
        {
            lock (_lifetimeLock)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(nameof(LinguaDetector), "This instance has already been disposed");
                }
            }
        }

        private IntPtr AcquireDetector()
        {
            lock (_lifetimeLock)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(nameof(LinguaDetector), "This instance has already been disposed");
                }

                _activeOperations++;
                return _detector;
            }
        }

        private void ReleaseDetector()
        {
            lock (_lifetimeLock)
            {
                _activeOperations--;
                if (_activeOperations == 0)
                {
                    Monitor.PulseAll(_lifetimeLock);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            IntPtr detector;

            lock (_lifetimeLock)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                while (_activeOperations > 0)
                {
                    Monitor.Wait(_lifetimeLock);
                }

                if (disposing)
                {
                    // Dispose managed resources if any
                }

                detector = _detector;
                _detector = IntPtr.Zero;
            }

            if (detector != IntPtr.Zero)
            {
                LinguaDetectorWrapper.LinguaLanguageDetectorDestroy(detector);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~LinguaDetector()
        {
            try
            {
                Dispose(false);
            }
            catch
            {
            }
        }

        private static void CheckNativeStatus(LinguaStatus status)
        {
            if (status != LinguaStatus.OK)
            {
                throw new LinguaDetectorException($"Failed to detect language: {status}");
            }
        }
    }

}
