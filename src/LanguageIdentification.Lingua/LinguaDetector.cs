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
        private volatile bool _disposed = false;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

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
            CheckDisposed();
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "Count must be greater than or equal to zero");
            }

            var textBytes = LinguaDetectorWrapper.EncodeText(text);
            _semaphore.Wait();
            try
            {
                CheckDisposed();
                var status = LinguaDetectorWrapper.LinguaDetectSingle(
                    detector: _detector,
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
                _semaphore.Release();
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
            CheckDisposed();

            var textBytes = LinguaDetectorWrapper.EncodeText(text);
            _semaphore.Wait();
            try
            {
                CheckDisposed();
                var status = LinguaDetectorWrapper.LinguaDetectMixed(
                    detector: _detector,
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
                _semaphore.Release();
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
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(LinguaDetector), "This instance has already been disposed");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _semaphore.Wait();
            try
            {
                if (_disposed)
                {
                    return;
                }

                if (disposing)
                {
                    // Dispose managed resources if any
                }

                if (_detector != IntPtr.Zero)
                {
                    LinguaDetectorWrapper.LinguaLanguageDetectorDestroy(_detector);
                    _detector = IntPtr.Zero;
                }

                _disposed = true;
            }
            finally
            {
                _semaphore.Release();
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
