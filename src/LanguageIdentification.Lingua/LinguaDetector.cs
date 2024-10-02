using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Panlingo.LanguageIdentification.Lingua.Internal;

namespace Panlingo.LanguageIdentification.Lingua
{
    /// <summary>
    /// .NET wrapper for Lingua
    /// </summary>
    public class LinguaDetector : IDisposable
    {
        private IntPtr _detector;

        internal LinguaDetector(LinguaDetectorBuilder builder)
        {
            if (!IsSupported())
            {
                throw new NotSupportedException(
                    $"{nameof(LinguaDetector)} is not yet supported on {RuntimeInformation.RuntimeIdentifier}"
                );
            }

            _detector = LinguaDetectorWrapper.LinguaLanguageDetectorCreate(builder.GetNativePointer());
            if (_detector == IntPtr.Zero)
            {
                throw new LinguaDetectorException($"Failed to create {nameof(LinguaDetector)}");
            }
        }

        public static bool IsSupported()
        {
            return RuntimeInformation.OSArchitecture switch
            {
                Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
                _ => false,
            };
        }

        /// <summary>
        /// Produces a prediction for 'text'
        /// </summary>
        /// <param name="text">Some text in natural language</param>
        /// <returns>Language prediction</returns>
        /// <exception cref="LinguaDetectorException"></exception>
        public IEnumerable<LinguaPrediction> PredictLanguages(string text, int count = 10)
        {
            var status = LinguaDetectorWrapper.LinguaDetectSingle(
                detector: _detector,
                text: text,
                result: out var nativeResult
            );

            if (status == LinguaStatus.DetectFailure)
            {
                return Array.Empty<LinguaPrediction>();
            }

            if (status == LinguaStatus.BadTextPtr || status == LinguaStatus.BadOutputPtr)
            {
                throw new LinguaDetectorException($"Failed to detect language: {status}");
            }

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
                LinguaDetectorWrapper.LinguaPredictionResultDestroy(nativeResult.Predictions);
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
            var status = LinguaDetectorWrapper.LinguaDetectMixed(
                detector: _detector,
                text: text,
                result: out var nativeResult
            );

            if (status == LinguaStatus.DetectFailure)
            {
                return Array.Empty<LinguaPredictionRange>();
            }

            if (status == LinguaStatus.BadTextPtr || status == LinguaStatus.BadOutputPtr)
            {
                throw new LinguaDetectorException($"Failed to detect language: {status}");
            }

            try
            {
                var result = new LinguaPredictionRangeResult[nativeResult.PredictionsCount];
                var structSize = Marshal.SizeOf(typeof(LinguaPredictionRangeResult));

                for (var i = 0; i < nativeResult.PredictionsCount; i++)
                {
                    result[i] = Marshal.PtrToStructure<LinguaPredictionRangeResult>(nativeResult.Predictions + i * structSize);
                }

                var textBytes = Encoding.UTF8.GetBytes(text);

                return result
                    .OrderByDescending(x => x.Confidence)
                    .Select(x => new LinguaPredictionRange(x, textBytes))
                    .ToArray();
            }
            finally
            {
                LinguaDetectorWrapper.LinguaPredictionRangeResultDestroy(nativeResult.Predictions);
            }
        }

        public string GetLanguageCode(LinguaLanguage language, LinguaLanguageCode code)
        {
            var stringBuider = new StringBuilder(10);

            try
            {
                var status = LinguaDetectorWrapper.LinguaLangCode(
                    lang: language,
                    code: code,
                    buffer: stringBuider,
                    bufferSize: (UIntPtr)stringBuider.Capacity
                );

                if (status < 0)
                {
                    throw new LinguaDetectorException($"Language code '{language}' is not found");
                }

                var result = stringBuider.ToString();
                return result;
            }
            finally
            {
                stringBuider.Clear();
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (_detector != IntPtr.Zero)
            {
                LinguaDetectorWrapper.LinguaLanguageDetectorDestroy(_detector);
                _detector = IntPtr.Zero;
            }
        }
    }

}
