using System.Runtime.InteropServices;

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

            var resultPtr = WhatlangDetectorWrapper.whatlang_detect(
                text: text, 
                info: out var resultCount
            );

            return new[] 
            {
                resultCount,
            };

            /*try
            {
                var result = new WhatlangPredictionResult[resultCount];
                var structSize = Marshal.SizeOf(typeof(WhatlangPredictionResult));

                for (var i = 0; i < resultCount; i++)
                {
                    result[i] = Marshal.PtrToStructure<WhatlangPredictionResult>(resultPtr + i * structSize);
                }

                return result.OrderByDescending(x => x.Probability).ToArray();
            }
            finally
            {
                WhatlangDetectorWrapper.FreeResults(resultPtr, resultCount);
            }*/
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
