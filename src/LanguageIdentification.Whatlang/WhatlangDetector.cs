using System;
using System.Runtime.InteropServices;
using System.Text;
using Panlingo.LanguageIdentification.Whatlang.Internal;

namespace Panlingo.LanguageIdentification.Whatlang
{
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
                throw new Exception($"Failed to detect langauge: {status}");
            }

            return new WhatlangPrediction(result);
        }

        public string GetLangCode(WhatlangLanguage lang)
        {
            var stringBuider = new StringBuilder(100);

            try
            {
                var code = WhatlangDetectorWrapper.WhatlangLangCode(lang, stringBuider, (UIntPtr)stringBuider.Capacity);
                if (code < 0)
                {
                    throw new Exception($"Language code '{lang}' is not found");
                }

                var result = stringBuider.ToString();
                return result;
            }
            finally
            {
                stringBuider.Clear();
            }
        }

        public string GetLangName(WhatlangLanguage lang)
        {
            var stringBuider = new StringBuilder(100);

            try
            {
                var code = WhatlangDetectorWrapper.WhatlangLangName(lang, stringBuider, (UIntPtr)stringBuider.Capacity);
                if (code < 0)
                {
                    throw new Exception($"Language code '{lang}' is not found");
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

        public string GetLangEngName(WhatlangLanguage lang)
        {
            var stringBuider = new StringBuilder(100);

            try
            {
                var code = WhatlangDetectorWrapper.WhatlangLangEngName(lang, stringBuider, (UIntPtr)stringBuider.Capacity);
                if (code < 0)
                {
                    throw new Exception($"Language code '{lang}' is not found");
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
