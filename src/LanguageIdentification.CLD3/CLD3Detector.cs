using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using Panlingo.LanguageIdentification.CLD3.Internal;

namespace Panlingo.LanguageIdentification.CLD3
{
    /// <summary>
    /// .NET wrapper for CLD3
    /// </summary>
    public class CLD3Detector : IDisposable
    {
        private readonly Lazy<ImmutableHashSet<string>> _labels;

        private IntPtr _detector;
        private bool _disposed = false;

        public CLD3Detector(int minNumBytes, int maxNumBytes)
        {
            if (!IsSupported())
            {
                throw new NotSupportedException(
                    $"{nameof(CLD3Detector)} is not yet supported on {RuntimeInformation.RuntimeIdentifier} ({RuntimeInformation.OSArchitecture})"
                );
            }

            _detector = CLD3DetectorWrapper.CreateCLD3(minNumBytes, maxNumBytes);

            _labels = new Lazy<ImmutableHashSet<string>>(
                () =>
                {
                    var result = ImmutableHashSet.CreateRange<string>(new string[]
                    {
                        "af", "am", "ar", "bg", "bg-Latn", "bn", "bs", "ca", "ceb", "co", 
                        "cs", "cy", "da", "de", "el", "el-Latn", "en", "eo", "es", "et", 
                        "eu", "fa", "fi", "fil",  "fr", "fy", "ga", "gd", "gl", "gu", 
                        "ha", "haw", "hi", "hi-Latn", "hmn", "hr", "ht", "hu", "hy", "id", 
                        "ig", "is", "it", "iw", "ja", "ja-Latn", "jv", "ka", "kk", "km", 
                        "kn", "ko", "ku", "ky", "la", "lb", "lo", "lt", "lv", "mg", 
                        "mi", "mk", "ml", "mn", "mr", "ms", "mt", "my", "ne", "nl", 
                        "no", "ny", "pa", "pl", "ps", "pt", "ro", "ru", "ru-Latn", "sd", 
                        "si", "sk", "sl", "sm", "sn", "so", "sq", "sr", "st", "su", 
                        "sv", "sw", "ta", "te", "tg", "th", "tr", "uk", "ur", "uz", 
                        "vi", "xh", "yi", "yo", "zh", "zh-Latn", "zu",
                    });

                    return result;
                }
            );
        }

        public static bool IsSupported()
        {
            return RuntimeInformation.OSArchitecture switch
            {
                Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
                Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => true,
                Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
                Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
                _ => false,
            };
        }

        /// <summary>
        /// Produces a prediction for 'text'
        /// </summary>
        /// <param name="text">Some text in natural language</param>
        /// <returns>List of language predictions</returns>
        public CLD3Prediction PredictLanguage(string text)
        {
            CheckDisposed();

            var resultPtr = CLD3DetectorWrapper.PredictLanguage(
                identifier: _detector,
                text: text,
                resultCount: out var resultCount
            );

            try
            {
                var nativeResult = new CLD3PredictionResult[resultCount];
                var structSize = Marshal.SizeOf(typeof(CLD3PredictionResult));

                for (var i = 0; i < resultCount; i++)
                {
                    nativeResult[i] = Marshal.PtrToStructure<CLD3PredictionResult>(resultPtr + i * structSize);
                }

                var firstItem = nativeResult.First();
                return new CLD3Prediction(firstItem);
            }
            finally
            {
                CLD3DetectorWrapper.DestroyPredictionResult(resultPtr, resultCount);
            }
        }

        /// <summary>
        /// Produces a prediction for 'text'
        /// </summary>
        /// <param name="text">Some text in natural language</param>
        /// <param name="count">Number of predictions</param>
        /// <returns>List of language predictions</returns>
        public IEnumerable<CLD3Prediction> PredictLanguages(
            string text,
            int count
        )
        {
            CheckDisposed();

            var resultPtr = CLD3DetectorWrapper.PredictLanguages(
                identifier: _detector,
                text: text,
                numLangs: count,
                resultCount: out var resultCount
            );

            try
            {
                var nativeResult = new CLD3PredictionResult[resultCount];
                var structSize = Marshal.SizeOf(typeof(CLD3PredictionResult));

                for (var i = 0; i < resultCount; i++)
                {
                    nativeResult[i] = Marshal.PtrToStructure<CLD3PredictionResult>(resultPtr + i * structSize);
                }

                return nativeResult
                    .OrderByDescending(x => x.Probability)
                    .Select(x => new CLD3Prediction(x))
                    .ToArray();
            }
            finally
            {
                CLD3DetectorWrapper.DestroyPredictionResult(resultPtr, resultCount);
            }
        }

        /// <summary>
        /// Gets all languages supported by CLD3
        /// </summary>
        /// <returns>Collection of strings</returns>
        public IEnumerable<string> GetLanguages()
        {
            return _labels.Value;
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(CLD3Detector), "This instance has already been disposed");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources if any
                }

                if (_detector != IntPtr.Zero)
                {
                    CLD3DetectorWrapper.DestroyCLD3(_detector);
                    _detector = IntPtr.Zero;
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CLD3Detector()
        {
            Dispose(false);
        }
    }
}
