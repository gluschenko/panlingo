using System;
using System.Collections.Generic;
using System.Drawing;
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
        private readonly IEnumerable<LinguaLanguage> _languages;
        private IntPtr _builder;
        private IntPtr _detector;

        public LinguaDetector(LinguaLanguage[] languages)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new NotSupportedException(
                    $"{nameof(LinguaDetector)} is not yet supported on {RuntimeInformation.RuntimeIdentifier}"
                );
            }

            _languages = languages;

            _builder = LinguaDetectorWrapper.LinguaLanguageDetectorBuilderCreate(languages, (UIntPtr)languages.Length);
            if (_builder == IntPtr.Zero)
            {
                throw new LinguaDetectorException("Failed to create LanguageDetectorBuilder");
            }

            _detector = LinguaDetectorWrapper.LinguaLanguageDetectorCreate(_builder);
            if (_detector == IntPtr.Zero)
            {
                LinguaDetectorWrapper.LinguaLanguageDetectorBuilderDestroy(_builder);
                throw new LinguaDetectorException("Failed to create LanguageDetector");
            }
        }

        /// <summary>
        /// Produces a prediction for 'text'
        /// </summary>
        /// <param name="text">Some text in natural language</param>
        /// <returns>Language prediction</returns>
        /// <exception cref="LinguaDetectorException"></exception>
        public LinguaPrediction? PredictLanguage(string text)
        {
            var status = LinguaDetectorWrapper.LinguaDetectSingle(
                detector: _detector,
                text: text,
                result: out var result
            );

            if (status == LinguaStatus.DetectFailure)
            {
                return null;
            }

            if (status == LinguaStatus.BadTextPtr || status == LinguaStatus.BadOutputPtr)
            {
                throw new LinguaDetectorException($"Failed to detect language: {status}");
            }

            return new LinguaPrediction(result);
        }

        /// <summary>
        /// Produces a prediction for 'text'
        /// </summary>
        /// <param name="text">Some text in natural language</param>
        /// <returns>Language prediction</returns>
        /// <exception cref="LinguaDetectorException"></exception>
        public IEnumerable<LinguaPrediction> PredictLanguages(string text)
        {
            var status = LinguaDetectorWrapper.LinguaDetectMultiple(
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

                /*var bytesNum = (int)nativeResult.PredictionsCount * structSize * 4;
                var bytes = new byte[bytesNum];
                Marshal.Copy(
                    source: nativeResult.Predictions,
                    destination: bytes,
                    startIndex: 0,
                    length: bytesNum
                );*/

                return result
                    .OrderByDescending(x => x.Confidence)
                    .Select(x => new LinguaPrediction(x))
                    .ToArray();
            }
            finally
            {
                LinguaDetectorWrapper.LinguaPredictionResultDestroy(nativeResult.Predictions);
            }
        }

        public string GetLanguageCode(LinguaLanguage language)
        {
            var stringBuider = new StringBuilder(10);

            try
            {
                var code = LinguaDetectorWrapper.LinguaLangCode(language, stringBuider, (UIntPtr)stringBuider.Capacity);
                if (code < 0)
                {
                    throw new Exception($"Language code '{language}' is not found");
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

            if (_builder != IntPtr.Zero)
            {
                LinguaDetectorWrapper.LinguaLanguageDetectorBuilderDestroy(_builder);
                _builder = IntPtr.Zero;
            }
        }
    }

}
