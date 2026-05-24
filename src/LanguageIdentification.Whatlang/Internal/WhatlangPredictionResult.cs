using System.Runtime.InteropServices;

namespace Panlingo.LanguageIdentification.Whatlang.Internal
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal readonly struct WhatlangPredictionResult
    {
        [MarshalAs(UnmanagedType.U1)]
        public readonly WhatlangLanguage Lang;

        [MarshalAs(UnmanagedType.U1)]
        public readonly WhatlangScript Script;

        [MarshalAs(UnmanagedType.R8)]
        public readonly double Confidence;

        [MarshalAs(UnmanagedType.I1)]
        public readonly bool IsReliable;
    }
}
