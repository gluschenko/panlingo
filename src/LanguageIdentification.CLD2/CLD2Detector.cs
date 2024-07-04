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

            var textLength = text.Length;
            var textPointer = Marshal.StringToHGlobalUni(text);

            var resultPtr = CLD2DetectorWrapper.PredictLanguage(
                data: textPointer, 
                length: textLength,
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

                return result;
            }
            finally
            {
                CLD2DetectorWrapper.FreeResults(resultPtr, resultCount);
                Marshal.FreeHGlobal(textPointer);
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
