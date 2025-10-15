using System;
using System.Runtime.InteropServices;
using System.Text;

using Panlingo.LanguageIdentification.Lingua.Native;

namespace Panlingo.LanguageIdentification.Lingua.Internal
{
    internal static class LinguaDetectorWrapper
    {
        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_language_detector_builder_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LinguaLanguageDetectorBuilderCreate(
            LinguaLanguage[] languages, 
            UIntPtr languageCount
        );

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_language_detector_builder_with_low_accuracy_mode", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LinguaLanguageDetectorBuilderWithLowAccuracyMode(IntPtr builder);

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_language_detector_builder_with_preloaded_language_models", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LinguaLanguageDetectorBuilderWithPreloadedLanguageModels(IntPtr builder);

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_language_detector_builder_with_minimum_relative_distance", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LinguaLanguageDetectorBuilderWithMinimumRelativeDistance(IntPtr builder, double distance);

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_language_detector_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LinguaLanguageDetectorCreate(IntPtr builder);

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_language_detector_builder_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LinguaLanguageDetectorBuilderDestroy(IntPtr builder);

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_language_detector_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LinguaLanguageDetectorDestroy(IntPtr detector);

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_prediction_result_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LinguaPredictionResultDestroy(IntPtr result, uint count);

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_prediction_range_result_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LinguaPredictionRangeResultDestroy(IntPtr result, uint count);

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_detect_single", CallingConvention = CallingConvention.Cdecl)]
        public static extern LinguaStatus LinguaDetectSingle(
            IntPtr detector,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string text,
            out LinguaPredictionListResult result
        );

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_detect_mixed", CallingConvention = CallingConvention.Cdecl)]
        public static extern LinguaStatus LinguaDetectMixed(
            IntPtr detector,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string text,
            out LinguaPredictionRangeListResult result
        );

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_language_code", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LinguaLangCode(
            LinguaLanguage lang,
            LinguaLanguageCode code,
            [MarshalAs(UnmanagedType.LPUTF8Str)] StringBuilder buffer,
            UIntPtr bufferSize
        );
    }
}
