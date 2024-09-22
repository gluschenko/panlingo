using System;
using System.Runtime.InteropServices;
using Panlingo.LanguageIdentification.CLD3.Native;

namespace Panlingo.LanguageIdentification.CLD3.Internal
{
    internal static class CLD3DetectorWrapper
    {
        [DllImport(CLD3NativeLibrary.Name, EntryPoint = "create_cld3", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateCLD3(int minNumBytes, int maxNumBytes);

        [DllImport(CLD3NativeLibrary.Name, EntryPoint = "destroy_cld3", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyCLD3(IntPtr identifier);

        [DllImport(CLD3NativeLibrary.Name, EntryPoint = "cld3_find_language", CallingConvention = CallingConvention.Cdecl)]
        public static extern CLD3PredictionResult FindLanguage(IntPtr identifier, string text);

        [DllImport(CLD3NativeLibrary.Name, EntryPoint = "cld3_find_languages", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FindLanguages(IntPtr identifier, string text, int numLangs, out int resultCount);

        [DllImport(CLD3NativeLibrary.Name, EntryPoint = "cld3_destroy_prediction_result", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyPredictionResult(IntPtr results, int count);
    }

}
