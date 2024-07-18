using System.Runtime.InteropServices;

namespace Panlingo.LanguageIdentification.Whatlang
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal readonly struct WhatlangPredictionResult
    {
        public readonly WhatlangLanguage Lang;

        public readonly WhatLangScript Script;

        [MarshalAs(UnmanagedType.R8)]
        public readonly double Confidence;

        [MarshalAs(UnmanagedType.I1)]
        public readonly bool IsReliable;
    }
}
