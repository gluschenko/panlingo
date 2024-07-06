using System.Text;

namespace LanguageIdentification.Whatlang;

public class WhatlangDetector : IDisposable
{
    private readonly SemaphoreSlim _semaphore;

    public WhatlangDetector()
    {
        _semaphore = new(1, 1);
    }

    public IEnumerable<WhatlangPredictionResult> PredictLanguage(string text)
    {
        try
        {
            _semaphore.Wait();

            var status = WhatlangDetectorWrapper.WhatlangDetect(
                text: text, 
                info: out var resultCount
            );

            if (status == WhatLangStatus.DetectFailure)
            {
                return Array.Empty<WhatlangPredictionResult>();
            }

            if (status == WhatLangStatus.BadTextPtr || status == WhatLangStatus.BadOutputPtr)
            {
                throw new Exception($"Failed to detect langauge: {status}");
            }

            return new[] 
            {
                resultCount,
            };
        }
        finally
        {
            _semaphore.Release();
        }
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
