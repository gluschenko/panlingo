using System.Runtime.InteropServices;
using System.Text;
using LanguageIdentification.Whatlang.Native;

namespace LanguageIdentification.Whatlang;

internal static class WhatlangDetectorWrapper
{
    [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_detect", CallingConvention = CallingConvention.Cdecl)]
    public static extern WhatLangStatus WhatlangDetect(string text, out WhatlangPredictionResult info);

    [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_detectn", CallingConvention = CallingConvention.Cdecl)]
    public static extern WhatLangStatus WhatlangDetectN(string text, int len, out WhatlangPredictionResult info);

    [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_lang_code", CallingConvention = CallingConvention.Cdecl)]
    public static extern int WhatlangLangCode(WhatLangLang lang, [MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer, UIntPtr bufferSize);
    
    [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_lang_eng_name", CallingConvention = CallingConvention.Cdecl)]
    public static extern int WhatlangLangEngName(WhatLangLang lang, [MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer, UIntPtr bufferSize);
    
    [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_lang_name", CallingConvention = CallingConvention.Cdecl)]
    public static extern int WhatlangLangName(WhatLangLang lang, [MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer, UIntPtr bufferSize);
    
    [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_script_name", CallingConvention = CallingConvention.Cdecl)]
    public static extern int WhatlangScriptName(WhatLangScript script, [MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer, UIntPtr bufferSize);
}

