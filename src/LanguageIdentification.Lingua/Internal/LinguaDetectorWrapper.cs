using System;
using System.Runtime.InteropServices;
using System.Text;

using Panlingo.LanguageIdentification.Lingua.Native;

namespace Panlingo.LanguageIdentification.Lingua.Internal
{
    internal static class LinguaDetectorWrapper
    {
        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_language_detector_builder_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LinguaLanguageDetectorBuilderCreate(LinguaLanguage[] languages, UIntPtr languageCount);

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_language_detector_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LinguaLanguageDetectorCreate(IntPtr builder);

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_language_detector_builder_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LinguaLanguageDetectorBuilderDestroy(IntPtr builder);

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_language_detector_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LinguaLanguageDetectorDestroy(IntPtr detector);

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_prediction_result_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LinguaPredictionResultDestroy(IntPtr detector);

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_detect_single", CallingConvention = CallingConvention.Cdecl)]
        public static extern LinguaStatus LinguaDetectSingle(
            IntPtr detector,
            string text,
            out LinguaPredictionResult result
        );

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_detect_multiple", CallingConvention = CallingConvention.Cdecl)]
        public static extern LinguaStatus LinguaDetectMultiple(
            IntPtr detector,
            string text,
            out LinguaPredictionListResult result
        );

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_language_code", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LinguaLangCode(
            LinguaLanguage lang,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer,
            UIntPtr bufferSize
        );
    }
}
