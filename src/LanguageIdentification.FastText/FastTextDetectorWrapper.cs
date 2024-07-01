using System.Runtime.InteropServices;
using LanguageIdentification.FastText.Native;

namespace LanguageIdentification.FastText
{
    internal static class FastTextDetectorWrapper
    {
        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateFastText();

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyFastText(IntPtr handle);

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void FastTextLoadModel(IntPtr handle, string filename, ref IntPtr errptr);

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FastTextGetModelDimensions(IntPtr handle);

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FastTextPredict(IntPtr handle, string text, int k, float threshold, ref IntPtr errptr);

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyPredictions(IntPtr predictions);

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FastTextGetLabels(IntPtr handle);

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyLabels(IntPtr labels);

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FastTextTokenize(IntPtr handle, string text);

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyTokens(IntPtr tokens);

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void FastTextAbort(IntPtr handle);

        [DllImport(FastTextNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyString(IntPtr str);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct FastTextPrediction
    {
        public readonly float Prob;
        public readonly IntPtr Label;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct FastTextPredictions
    {
        public readonly IntPtr Predictions;
        public readonly ulong Length;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct FastTextTokens
    {
        public readonly IntPtr Tokens;
        public readonly ulong Length;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct FastTextLabels
    {
        public readonly IntPtr Labels;
        public readonly IntPtr Freqs;
        public readonly ulong Length;
    }
}
