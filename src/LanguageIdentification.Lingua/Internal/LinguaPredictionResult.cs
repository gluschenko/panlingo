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

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal readonly struct LinguaPredictionRangeResult
    {
        [MarshalAs(UnmanagedType.U1)]
        public readonly LinguaLanguage Language;

        [MarshalAs(UnmanagedType.R8)]
        public readonly double Confidence;

        public readonly uint StartIndex;

        public readonly uint EndIndex;

        public readonly uint WordCount;

        public static explicit operator LinguaPredictionResult(LinguaPredictionRangeResult item)
        {
            return new LinguaPredictionResult(item.Language, item.Confidence);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal readonly struct LinguaPredictionListResult
    {
        public readonly IntPtr Predictions;
        public readonly uint PredictionsCount;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal readonly struct LinguaPredictionRangeListResult
    {
        public readonly IntPtr Predictions;
        public readonly uint PredictionsCount;
    }
}
