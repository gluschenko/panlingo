using System.Runtime.InteropServices;

namespace LanguageIdentification.CLD2;

public class CLD2Detector : IDisposable
{
    private readonly SemaphoreSlim _semaphore;

    public CLD2Detector()
    {
        _semaphore = new(1, 1);
    }

    public IEnumerable<CLD2PredictionResult> PredictLanguage(string text)
    {
        try
        {
            _semaphore.Wait();

            var resultPtr = CLD2DetectorWrapper.PredictLanguage(
                text: text, 
                resultCount: out var resultCount
            );

            try
            {
                var result = new CLD2PredictionResult[resultCount];
                var structSize = Marshal.SizeOf(typeof(CLD2PredictionResult));

                for (var i = 0; i < resultCount; i++)
                {
                    result[i] = Marshal.PtrToStructure<CLD2PredictionResult>(resultPtr + i * structSize);
                }

                return result.OrderByDescending(x => x.Probability).ToArray();
            }
            finally
            {
                CLD2DetectorWrapper.FreeResults(resultPtr, resultCount);
            }
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
