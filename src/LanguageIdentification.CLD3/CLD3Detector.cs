using System.Runtime.InteropServices;

namespace LanguageIdentification.CLD3;

public class CLD3Detector : IDisposable
{
    private readonly nint _identifier;
    private readonly SemaphoreSlim _semaphore;

    public CLD3Detector(int minNumBytes, int maxNumBytes)
    {
        _identifier = CLD3DetectorWrapper.CreateIdentifier(minNumBytes, maxNumBytes);
        _semaphore = new(1, 1);
    }

    public void Dispose()
    {
        try
        {
            _semaphore.Wait();

            GC.SuppressFinalize(this);
            CLD3DetectorWrapper.FreeIdentifier(_identifier);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public CLD3PredictionResult FindLanguage(string text)
    {
        try
        {
            _semaphore.Wait();

            return CLD3DetectorWrapper.FindLanguage(_identifier, text);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public CLD3PredictionResult[] FindLanguageNMostFreqLangs(
        string text,
        int numLangs
    )
    {
        try
        {
            _semaphore.Wait();

            var resultPtr = CLD3DetectorWrapper.FindTopNMostFreqLangs(
                identifier: _identifier,
                text: text,
                numLangs: numLangs,
                resultCount: out var resultCount
            );

            try
            {
                var result = new CLD3PredictionResult[resultCount];
                var structSize = Marshal.SizeOf(typeof(CLD3PredictionResult));

                for (var i = 0; i < resultCount; i++)
                {
                    result[i] = Marshal.PtrToStructure<CLD3PredictionResult>(resultPtr + i * structSize);
                }

                return result;
            }
            finally
            {
                CLD3DetectorWrapper.FreeResults(resultPtr, resultCount);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
