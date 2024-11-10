using System.Runtime.InteropServices;

namespace Panlingo.LanguageIdentification.Lingua.Internal
{
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
}
