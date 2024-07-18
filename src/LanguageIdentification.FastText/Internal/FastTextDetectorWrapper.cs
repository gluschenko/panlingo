using System.Runtime.InteropServices;

using Panlingo.LanguageIdentification.FastText.Native;

namespace Panlingo.LanguageIdentification.FastText.Internal
{
    internal static class FastTextDetectorWrapper
    {
        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern nint CreateFastText();

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyFastText(nint handle);

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void FastTextLoadModel(nint handle, string filename, ref nint errptr);

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FastTextGetModelDimensions(nint handle);

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern nint FastTextPredict(nint handle, string text, int k, float threshold, ref nint errptr);

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyPredictions(nint predictions);

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern nint FastTextGetLabels(nint handle);

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyLabels(nint labels);

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern nint FastTextTokenize(nint handle, string text);

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyTokens(nint tokens);

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void FastTextAbort(nint handle);

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyString(nint str);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct FastTextPrediction
    {
        public readonly float Prob;
        public readonly nint Label;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct FastTextPredictions
    {
        public readonly nint Predictions;
        public readonly ulong Length;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct FastTextTokens
    {
        public readonly nint Tokens;
        public readonly ulong Length;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct FastTextLabels
    {
        public readonly nint Labels;
        public readonly nint Freqs;
        public readonly ulong Length;
    }
}
