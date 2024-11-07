using System;
using System.Runtime.InteropServices;

namespace Panlingo.LanguageIdentification.Lingua.Internal
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal readonly struct LinguaPredictionRangeListResult
    {
        public readonly IntPtr Predictions;
        public readonly uint PredictionsCount;
    }
}
