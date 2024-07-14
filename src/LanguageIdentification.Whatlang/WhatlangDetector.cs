using System;
using System.Text;

namespace LanguageIdentification.Whatlang
{
    public class WhatlangDetector : IDisposable
    {
        public WhatlangDetector()
        {

        }

        public WhatlangPredictionResult? PredictLanguage(string text)
        {
            var status = WhatlangDetectorWrapper.WhatlangDetect(
                text: text,
                info: out var resultCount
            );

            if (status == WhatLangStatus.DetectFailure)
            {
                return null;
            }

            if (status == WhatLangStatus.BadTextPtr || status == WhatLangStatus.BadOutputPtr)
            {
                throw new Exception($"Failed to detect langauge: {status}");
            }

            return resultCount;
        }

        public string GetLangCode(WhatLangLang lang)
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

        public string GetLangName(WhatLangLang lang)
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

        public string GetScriptName(WhatLangScript script)
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

        public string GetLangEngName(WhatLangLang lang)
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
