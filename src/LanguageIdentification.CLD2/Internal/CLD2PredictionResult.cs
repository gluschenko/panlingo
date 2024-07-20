using System.Runtime.InteropServices;

namespace Panlingo.LanguageIdentification.CLD2.Internal
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal readonly struct CLD2PredictionResult
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public readonly string Language;

        [MarshalAs(UnmanagedType.LPStr)]
        public readonly string Script;

        [MarshalAs(UnmanagedType.R8)]
        public readonly double Probability;

        [MarshalAs(UnmanagedType.I1)]
        public readonly bool IsReliable;

        [MarshalAs(UnmanagedType.R8)]
        public readonly double Proportion;
    }
}
