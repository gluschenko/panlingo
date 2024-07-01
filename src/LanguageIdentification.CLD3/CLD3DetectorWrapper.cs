using System.Runtime.InteropServices;
using LanguageIdentification.CLD3.Native;

namespace LanguageIdentification.CLD3;

internal static class CLD3DetectorWrapper
{
    [DllImport(CLD3NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern nint CreateIdentifier(int minNumBytes, int maxNumBytes);

    [DllImport(CLD3NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern void FreeIdentifier(nint identifier);

    [DllImport(CLD3NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern CLD3PredictionResult FindLanguage(nint identifier, string text);

    [DllImport(CLD3NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern nint FindTopNMostFreqLangs(nint identifier, string text, int numLangs, out int resultCount);

    [DllImport(CLD3NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern void FreeResults(nint results, int count);
}
