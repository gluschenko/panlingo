using System.Runtime.InteropServices;

namespace LanguageIdentification.CLD3;

// Inspired by:
// https://github.com/NikulovE/cld3.net/tree/master
// https://github.com/uranium62/cld3-net/tree/master
// https://github.com/google/cld3
// + GPT-4o
public class CLD3Detector : IDisposable
{
    private readonly nint _identifier;

    public CLD3Detector(int minNumBytes, int maxNumBytes)
    {
        _identifier = CLD3DetectorWrapper.CreateIdentifier(minNumBytes, maxNumBytes);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        CLD3DetectorWrapper.FreeIdentifier(_identifier);
    }

    public CLD3DetectorWrapper.Result FindLanguage(string text)
    {
        return CLD3DetectorWrapper.FindLanguage(_identifier, text);
    }

    public CLD3DetectorWrapper.Result[] FindLanguageNMostFreqLangs(
        string text,
        int numLangs
    )
    {
        var resultPtr = CLD3DetectorWrapper.FindTopNMostFreqLangs(
            identifier: _identifier,
            text: text,
            numLangs: numLangs,
            resultCount: out var resultCount
        );

        try
        {
            var result = new CLD3DetectorWrapper.Result[resultCount];
            var structSize = Marshal.SizeOf(typeof(CLD3DetectorWrapper.Result));

            for (var i = 0; i < resultCount; i++)
            {
                result[i] = Marshal.PtrToStructure<CLD3DetectorWrapper.Result>(resultPtr + i * structSize);
            }

            return result;
        }
        finally
        {
            CLD3DetectorWrapper.FreeResults(resultPtr, resultCount);
        }
    }
}

internal static class CLD3DetectorWrapper
{
#pragma warning disable IDE1006 // Naming Styles
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct Result
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string Language;

        [MarshalAs(UnmanagedType.R8)]
        public double Probability;

        [MarshalAs(UnmanagedType.I1)]
        public bool IsReliable;

        [MarshalAs(UnmanagedType.R8)]
        public double Proportion;
    }
#pragma warning restore IDE1006 // Naming Styles

    [DllImport("libcld3", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint CreateIdentifier(int minNumBytes, int maxNumBytes);

    [DllImport("libcld3", CallingConvention = CallingConvention.Cdecl)]
    public static extern void FreeIdentifier(nint identifier);

    [DllImport("libcld3", CallingConvention = CallingConvention.Cdecl)]
    public static extern Result FindLanguage(nint identifier, string text);

    [DllImport("libcld3", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint FindTopNMostFreqLangs(nint identifier, string text, int numLangs, out int resultCount);

    [DllImport("libcld3", CallingConvention = CallingConvention.Cdecl)]
    public static extern void FreeResults(nint results, int count);

    public static Result[] FindTopNMostFreqLangsWrapper(nint identifier, string text, int numLangs)
    {
        int resultCount;
        var resultsPtr = FindTopNMostFreqLangs(identifier, text, numLangs, out resultCount);

        var results = new Result[resultCount];
        var structSize = Marshal.SizeOf(typeof(Result));

        for (var i = 0; i < resultCount; i++)
        {
            results[i] = Marshal.PtrToStructure<Result>(resultsPtr + i * structSize);
        }

        FreeResults(resultsPtr, resultCount);
        return results;
    }
}
