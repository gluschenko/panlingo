﻿using System.Runtime.InteropServices;
using System.Text;

using Panlingo.LanguageIdentification.Whatlang.Native;

namespace Panlingo.LanguageIdentification.Whatlang.Internal
{
    internal static class WhatlangDetectorWrapper
    {
        [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_detect", CallingConvention = CallingConvention.Cdecl)]
        public static extern WhatlangStatus WhatlangDetect(
            string text,
            out WhatlangPredictionResult info
        );

        [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_detectn", CallingConvention = CallingConvention.Cdecl)]
        public static extern WhatlangStatus WhatlangDetectN(
            string text,
            int len,
            out WhatlangPredictionResult info
        );

        [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_lang_code", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WhatlangLangCode(
            WhatlangLanguage lang,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer,
            nuint bufferSize
        );

        [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_lang_eng_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WhatlangLangEngName(
            WhatlangLanguage lang,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer,
            nuint bufferSize
        );

        [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_lang_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WhatlangLangName(
            WhatlangLanguage lang,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer,
            nuint bufferSize
        );

        [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_script_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WhatlangScriptName(
            WhatlangScript script,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer,
            nuint bufferSize
        );
    }


}