using System;
using System.Runtime.InteropServices;

namespace Panlingo.LanguageIdentification.Lingua.Internal
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal readonly struct LinguaPredictionResult
    {
        public readonly LinguaLanguage Language;

        [MarshalAs(UnmanagedType.R8)]
        public readonly double Confidence;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal readonly struct LanguageDetectorListResult
    {
        public readonly IntPtr Predictions;
        public readonly uint PredictionsCount;
    }
}
