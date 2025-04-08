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
            [MarshalAs(UnmanagedType.LPUTF8Str)] string text,
            out WhatlangPredictionResult result
        );

        [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_detect_lang", CallingConvention = CallingConvention.Cdecl)]
        public static extern WhatlangStatus WhatlangDetectLang(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string text,
            out WhatlangLanguage result
        );

        [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_detect_script", CallingConvention = CallingConvention.Cdecl)]
        public static extern WhatlangStatus WhatlangDetectScript(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string text,
            out WhatlangScript result
        );

        [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_lang_code", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WhatlangLangCode(
            WhatlangLanguage lang,
            [MarshalAs(UnmanagedType.LPUTF8Str)] StringBuilder buffer,
            UIntPtr bufferSize
        );

        [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_lang_eng_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WhatlangLangEngName(
            WhatlangLanguage lang,
            [MarshalAs(UnmanagedType.LPUTF8Str)] StringBuilder buffer,
            UIntPtr bufferSize
        );

        [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_lang_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WhatlangLangName(
            WhatlangLanguage lang,
            [MarshalAs(UnmanagedType.LPUTF8Str)] StringBuilder buffer,
            UIntPtr bufferSize
        );

        [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_script_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WhatlangScriptName(
            WhatlangScript script,
            [MarshalAs(UnmanagedType.LPUTF8Str)] StringBuilder buffer,
            UIntPtr bufferSize
        );
    }


}
