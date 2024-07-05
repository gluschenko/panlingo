﻿using System.Runtime.InteropServices;
using LanguageIdentification.Whatlang.Native;

namespace LanguageIdentification.Whatlang;

internal static class WhatlangDetectorWrapper
{
    [DllImport(WhatlangNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern nint PredictLanguage(string text, out int resultCount);

    [DllImport(WhatlangNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern void FreeResults(nint results, int count);
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
    AKA = 0,
    AMH, ARB, AZJ, BEL, BEN, BHO, BUL, CEB,
    CES, CMN, DAN, DEU, ELL, ENG, EPO, EST, FIN,
    FRA, GUJ, HAT, HAU, HEB, HIN, HRV, HUN, IBO,
    ILO, IND, ITA, JAV, JPN, KAN, KAT, KHM, KIN,
    KOR, KUR, LAV, LIT, MAI, MAL, MAR, MKD, MLG,
    MYA, NEP, NLD, NNO, NOB, NYA, ORI, ORM, PAN,
    PES, POL, POR, RON, RUN, RUS, SIN, SKR, SLV,
    SNA, SOM, SPA, SRP, SWE, TAM, TEL, TGL, THA,
    TIR, TUK, TUR, UIG, UKR, URD, UZB, VIE, YDD,
    YOR, ZUL
}

public enum WhatLangScript : byte
{
    ARABIC = 0, BENGALI, CYRILLIC, DEVANAGARI, ETHIOPIC,
    GEORGIAN, GREEK, GUJARATI, GURMUKHI, HANGUL,
    HEBREW, HIRAGANA, KANNADA, KATAKANA, KHMER,
    LATIN, MALAYALAM, MANDARIN, MYANMAR, ORIYA,
    SINHALA, TAMIL, TELUGU, THAI
}
