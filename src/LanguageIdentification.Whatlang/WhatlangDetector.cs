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

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
