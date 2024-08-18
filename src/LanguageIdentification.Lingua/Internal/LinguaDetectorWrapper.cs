using System;
using System.Runtime.InteropServices;
using System.Text;

using Panlingo.LanguageIdentification.Lingua.Native;

namespace Panlingo.LanguageIdentification.Lingua.Internal
{
    internal static class LinguaDetectorWrapper
    {
        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_language_detector_builder_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lingua_language_detector_builder_create(LinguaLanguage[] languages, UIntPtr languageCount);

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_language_detector_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lingua_language_detector_create(IntPtr builder);

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_language_detector_builder_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void lingua_language_detector_builder_destroy(IntPtr builder);

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_language_detector_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void lingua_language_detector_destroy(IntPtr detector);

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_detect_single", CallingConvention = CallingConvention.Cdecl)]
        public static extern LinguaStatus LinguaDetectSingle(
            IntPtr detector,
            string text,
            out LinguaPredictionResult result
        );

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_language_code", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LinguaLangCode(
            LinguaLanguage lang,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer,
            UIntPtr bufferSize
        );
    }
}
