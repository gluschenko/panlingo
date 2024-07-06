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
    public static extern int WhatlangLangCode(WhatLangLang lang, out string buffer);
    
    [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_lang_eng_name", CallingConvention = CallingConvention.Cdecl)]
    public static extern int WhatlangLangEngName(WhatLangLang lang, out string buffer);
    
    [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_lang_name", CallingConvention = CallingConvention.Cdecl)]
    public static extern int WhatlangLangName(WhatLangLang lang, out string buffer);
    
    [DllImport(WhatlangNativeLibrary.Name, EntryPoint = "whatlang_script_name", CallingConvention = CallingConvention.Cdecl)]
    public static extern int WhatlangScriptName(WhatLangScript script, out string buffer);
}

public enum WhatLangStatus : byte
{
    OK = 0,
    DetectFailure = 1,
    BadTextPtr = 2,
    BadOutputPtr = 3,
}

public enum WhatLangLang : byte
{
    /// Esperanto (Esperanto)
    Epo = 0,
    /// English (English)
    Eng = 1,
    /// Русский (Russian)
    Rus = 2,
    /// 普通话 (Mandarin)
    Cmn = 3,
    /// Español (Spanish)
    Spa = 4,
    /// Português (Portuguese)
    Por = 5,
    /// Italiano (Italian)
    Ita = 6,
    /// বাংলা (Bengali)
    Ben = 7,
    /// Français (French)
    Fra = 8,
    /// Deutsch (German)
    Deu = 9,
    /// Українська (Ukrainian)
    Ukr = 10,
    /// ქართული (Georgian)
    Kat = 11,
    /// العربية (Arabic)
    Ara = 12,
    /// हिन्दी (Hindi)
    Hin = 13,
    /// 日本語 (Japanese)
    Jpn = 14,
    /// עברית (Hebrew)
    Heb = 15,
    /// ייִדיש (Yiddish)
    Yid = 16,
    /// Polski (Polish)
    Pol = 17,
    /// አማርኛ (Amharic)
    Amh = 18,
    /// Basa Jawa (Javanese)
    Jav = 19,
    /// 한국어 (Korean)
    Kor = 20,
    /// Bokmål (Bokmal)
    Nob = 21,
    /// Dansk (Danish)
    Dan = 22,
    /// Svenska (Swedish)
    Swe = 23,
    /// Suomi (Finnish)
    Fin = 24,
    /// Türkçe (Turkish)
    Tur = 25,
    /// Nederlands (Dutch)
    Nld = 26,
    /// Magyar (Hungarian)
    Hun = 27,
    /// Čeština (Czech)
    Ces = 28,
    /// Ελληνικά (Greek)
    Ell = 29,
    /// Български (Bulgarian)
    Bul = 30,
    /// Беларуская (Belarusian)
    Bel = 31,
    /// मराठी (Marathi)
    Mar = 32,
    /// ಕನ್ನಡ (Kannada)
    Kan = 33,
    /// Română (Romanian)
    Ron = 34,
    /// Slovenščina (Slovene)
    Slv = 35,
    /// Hrvatski (Croatian)
    Hrv = 36,
    /// Српски (Serbian)
    Srp = 37,
    /// Македонски (Macedonian)
    Mkd = 38,
    /// Lietuvių (Lithuanian)
    Lit = 39,
    /// Latviešu (Latvian)
    Lav = 40,
    /// Eesti (Estonian)
    Est = 41,
    /// தமிழ் (Tamil)
    Tam = 42,
    /// Tiếng Việt (Vietnamese)
    Vie = 43,
    /// اُردُو (Urdu)
    Urd = 44,
    /// ภาษาไทย (Thai)
    Tha = 45,
    /// ગુજરાતી (Gujarati)
    Guj = 46,
    /// Oʻzbekcha (Uzbek)
    Uzb = 47,
    /// ਪੰਜਾਬੀ (Punjabi)
    Pan = 48,
    /// Azərbaycanca (Azerbaijani)
    Aze = 49,
    /// Bahasa Indonesia (Indonesian)
    Ind = 50,
    /// తెలుగు (Telugu)
    Tel = 51,
    /// فارسی (Persian)
    Pes = 52,
    /// മലയാളം (Malayalam)
    Mal = 53,
    /// ଓଡ଼ିଆ (Oriya)
    Ori = 54,
    /// မြန်မာစာ (Burmese)
    Mya = 55,
    /// नेपाली (Nepali)
    Nep = 56,
    /// සිංහල (Sinhalese)
    Sin = 57,
    /// ភាសាខ្មែរ (Khmer)
    Khm = 58,
    /// Türkmençe (Turkmen)
    Tuk = 59,
    /// Akan (Akan)
    Aka = 60,
    /// IsiZulu (Zulu)
    Zul = 61,
    /// ChiShona (Shona)
    Sna = 62,
    /// Afrikaans (Afrikaans)
    Afr = 63,
    /// Lingua Latina (Latin)
    Lat = 64,
    /// Slovenčina (Slovak)
    Slk = 65,
    /// Català (Catalan)
    Cat = 66,
    /// Tagalog (Tagalog)
    Tgl = 67,
    /// Հայերեն (Armenian)
    Hye = 68,
}

public enum WhatLangScript : byte
{
    ARABIC = 0, 
    BENGALI = 1, 
    CYRILLIC = 2, 
    DEVANAGARI = 3, 
    ETHIOPIC = 4,
    GEORGIAN = 5, 
    GREEK = 6, 
    GUJARATI = 7,
    GURMUKHI = 8,
    HANGUL = 9,
    HEBREW = 10, 
    HIRAGANA = 11, 
    KANNADA = 12, 
    KATAKANA = 13, 
    KHMER = 14,
    LATIN = 15,
    MALAYALAM = 16, 
    MANDARIN = 17, 
    MYANMAR = 18, 
    ORIYA = 19,
    SINHALA = 20, 
    TAMIL = 21, 
    TELUGU = 22, 
    THAI = 23,
}

