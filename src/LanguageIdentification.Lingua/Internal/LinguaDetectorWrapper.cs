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
            byte[] text,
            UIntPtr textLength,
            out LinguaPredictionListResult result
        );

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_detect_mixed", CallingConvention = CallingConvention.Cdecl)]
        public static extern LinguaStatus LinguaDetectMixed(
            IntPtr detector,
            byte[] text,
            UIntPtr textLength,
            out LinguaPredictionRangeListResult result
        );

        [DllImport(LinguaNativeLibrary.Name, EntryPoint = "lingua_language_code", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr LinguaLangCode(
            byte lang,
            byte code,
            IntPtr buffer,
            UIntPtr bufferSize
        );

        public static string LinguaLangCode(LinguaLanguage lang, LinguaLanguageCode code)
        {
            return ReadNativeString((buffer, size) => LinguaLangCode((byte)lang, (byte)code, buffer, size));
        }

        public static byte[] EncodeText(string text)
        {
            return text is null ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(text);
        }

        private static string ReadNativeString(Func<IntPtr, UIntPtr, UIntPtr> nativeCall)
        {
            var requiredLength = checked((int)nativeCall(IntPtr.Zero, UIntPtr.Zero));
            var buffer = Marshal.AllocHGlobal(requiredLength + 1);

            try
            {
                var actualLength = checked((int)nativeCall(buffer, (UIntPtr)(requiredLength + 1)));
                if (actualLength > requiredLength)
                {
                    throw new LinguaDetectorException("Native string buffer size changed during read");
                }

                var bytes = new byte[actualLength];
                Marshal.Copy(buffer, bytes, 0, actualLength);
                return Encoding.UTF8.GetString(bytes);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
}
