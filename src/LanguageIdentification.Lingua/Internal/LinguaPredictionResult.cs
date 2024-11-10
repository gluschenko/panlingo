using System;
using System.Runtime.InteropServices;

namespace Panlingo.LanguageIdentification.Lingua.Internal
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal readonly struct LinguaPredictionResult
    {
        [MarshalAs(UnmanagedType.U1)]
        public readonly LinguaLanguage Language;

        [MarshalAs(UnmanagedType.R8)]
        public readonly double Confidence;

        public LinguaPredictionResult(LinguaLanguage language, double confidence)
        {
            Language = language;
            Confidence = confidence;
        }
    }
}
