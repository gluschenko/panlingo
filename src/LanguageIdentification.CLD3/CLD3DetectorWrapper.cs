using System.Runtime.InteropServices;

namespace LanguageIdentification.CLD3;

internal static class CLD3DetectorWrapper
{
    [DllImport("libcld3", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint CreateIdentifier(int minNumBytes, int maxNumBytes);

    [DllImport("libcld3", CallingConvention = CallingConvention.Cdecl)]
    public static extern void FreeIdentifier(nint identifier);

    [DllImport("libcld3", CallingConvention = CallingConvention.Cdecl)]
    public static extern CLD3PredictionResult FindLanguage(nint identifier, string text);

    [DllImport("libcld3", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint FindTopNMostFreqLangs(nint identifier, string text, int numLangs, out int resultCount);

    [DllImport("libcld3", CallingConvention = CallingConvention.Cdecl)]
    public static extern void FreeResults(nint results, int count);
}
