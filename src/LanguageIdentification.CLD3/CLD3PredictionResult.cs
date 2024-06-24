using System.Runtime.InteropServices;

namespace LanguageIdentification.CLD3;

#pragma warning disable IDE1006 // Naming Styles
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct CLD3PredictionResult
{
    [MarshalAs(UnmanagedType.LPStr)]
    public string Language;

    [MarshalAs(UnmanagedType.R8)]
    public double Probability;

    [MarshalAs(UnmanagedType.I1)]
    public bool IsReliable;

    [MarshalAs(UnmanagedType.R8)]
    public double Proportion;
}
#pragma warning restore IDE1006 // Naming Styles
