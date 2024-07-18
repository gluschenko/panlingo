using System;
using System.Runtime.InteropServices;
using Panlingo.LanguageIdentification.CLD3.Native;

namespace Panlingo.LanguageIdentification.CLD3.Internal
{
    internal static class CLD3DetectorWrapper
    {
        [DllImport(CLD3NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateIdentifier(int minNumBytes, int maxNumBytes);

        [DllImport(CLD3NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeIdentifier(IntPtr identifier);

        [DllImport(CLD3NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern CLD3PredictionResult FindLanguage(IntPtr identifier, string text);

        [DllImport(CLD3NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FindLanguages(IntPtr identifier, string text, int numLangs, out int resultCount);

        [DllImport(CLD3NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeResults(IntPtr results, int count);
    }

}
