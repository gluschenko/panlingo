using System;
using System.Runtime.InteropServices;
using System.Text;
using Panlingo.LanguageIdentification.Whatlang.Internal;

namespace Panlingo.LanguageIdentification.Whatlang
{
    /// <summary>
    /// .NET wrapper for Whatlang
    /// </summary>
    public class WhatlangDetector : IDisposable
    {
        public WhatlangDetector()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new NotSupportedException(
                    $"{nameof(WhatlangDetector)} is not yet supported on {RuntimeInformation.RuntimeIdentifier}"
                );
            }
        }

        /// <summary>
        /// Produces a prediction for 'text'
        /// </summary>
        /// <param name="text">Some text in natural language</param>
        /// <returns>Language prediction</returns>
        /// <exception cref="WhatlangDetectorException"></exception>
        public WhatlangPrediction? PredictLanguage(string text)
        {
            var status = WhatlangDetectorWrapper.WhatlangDetect(
                text: text,
                info: out var result
            );

            if (status == WhatlangStatus.DetectFailure)
            {
                return null;
            }

            if (status == WhatlangStatus.BadTextPtr || status == WhatlangStatus.BadOutputPtr)
            {
                throw new WhatlangDetectorException($"Failed to detect langauge: {status}");
            }

            return new WhatlangPrediction(result);
        }

        public string GetLanguageCode(WhatlangLanguage language)
        {
            var stringBuider = new StringBuilder(100);

            try
            {
                var code = WhatlangDetectorWrapper.WhatlangLangCode(language, stringBuider, (UIntPtr)stringBuider.Capacity);
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

        public string GetLanguageName(WhatlangLanguage language)
        {
            var stringBuider = new StringBuilder(100);

            try
            {
                var code = WhatlangDetectorWrapper.WhatlangLangName(language, stringBuider, (UIntPtr)stringBuider.Capacity);
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

        public string GetScriptName(WhatlangScript script)
        {
            var stringBuider = new StringBuilder(100);

            try
            {
                var code = WhatlangDetectorWrapper.WhatlangScriptName(script, stringBuider, (UIntPtr)stringBuider.Capacity);
                if (code < 0)
                {
                    throw new Exception($"Language script '{script}' is not found");
                }

                var result = stringBuider.ToString();
                return result;
            }
            finally
            {
                stringBuider.Clear();
            }
        }

        public string GetLanguageEnglishName(WhatlangLanguage language)
        {
            var stringBuider = new StringBuilder(100);

            try
            {
                var code = WhatlangDetectorWrapper.WhatlangLangEngName(language, stringBuider, (UIntPtr)stringBuider.Capacity);
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
        }
    }

}
