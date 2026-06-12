using System;
using System.Runtime.InteropServices;
using System.Text;

using Panlingo.LanguageIdentification.Whatlang.Native;

namespace Panlingo.LanguageIdentification.Whatlang.Internal
{
    internal static class WhatlangDetectorWrapper
    {
        [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_detect", CallingConvention = CallingConvention.Cdecl)]
        public static extern WhatlangStatus WhatlangDetect(
            byte[] text,
            UIntPtr textLength,
            out WhatlangPredictionResult result
        );

        [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_detect_script", CallingConvention = CallingConvention.Cdecl)]
        public static extern WhatlangStatus WhatlangDetectScript(
            byte[] text,
            UIntPtr textLength,
            out WhatlangScript result
        );

        [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_lang_code", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr WhatlangLangCode(
            byte lang,
            IntPtr buffer,
            UIntPtr bufferSize
        );

        [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_lang_eng_name", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr WhatlangLangEngName(
            byte lang,
            IntPtr buffer,
            UIntPtr bufferSize
        );

        [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_lang_name", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr WhatlangLangName(
            byte lang,
            IntPtr buffer,
            UIntPtr bufferSize
        );

        [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_script_name", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr WhatlangScriptName(
            byte script,
            IntPtr buffer,
            UIntPtr bufferSize
        );

        public static string WhatlangLangCode(WhatlangLanguage lang)
        {
            return ReadNativeString((buffer, size) => WhatlangLangCode((byte)lang, buffer, size));
        }

        public static string WhatlangLangEngName(WhatlangLanguage lang)
        {
            return ReadNativeString((buffer, size) => WhatlangLangEngName((byte)lang, buffer, size));
        }

        public static string WhatlangLangName(WhatlangLanguage lang)
        {
            return ReadNativeString((buffer, size) => WhatlangLangName((byte)lang, buffer, size));
        }

        public static string WhatlangScriptName(WhatlangScript script)
        {
            return ReadNativeString((buffer, size) => WhatlangScriptName((byte)script, buffer, size));
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
                    throw new WhatlangDetectorException("Native string buffer size changed during read");
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
