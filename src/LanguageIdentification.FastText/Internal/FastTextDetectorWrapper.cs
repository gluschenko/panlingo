using System;
using System.Runtime.InteropServices;

using Panlingo.LanguageIdentification.FastText.Native;

namespace Panlingo.LanguageIdentification.FastText.Internal
{
    internal static class FastTextDetectorWrapper
    {
        [DllImport(FastTextNativeLibrary.Name, EntryPoint = "destroy_string", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyString(IntPtr str);

        [DllImport(FastTextNativeLibrary.Name, EntryPoint = "create_fasttext", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateFastText();

        [DllImport(FastTextNativeLibrary.Name, EntryPoint = "destroy_fasttext", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyFastText(IntPtr handle);

        [DllImport(FastTextNativeLibrary.Name, EntryPoint = "fasttext_load_model", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FastTextLoadModel(IntPtr handle, string filename, ref IntPtr errPtr);

        [DllImport(FastTextNativeLibrary.Name, EntryPoint = "fasttext_load_model_data", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FastTextLoadModelData(IntPtr handle, IntPtr buffer, uint bufferLength, ref IntPtr errPtr);

        [DllImport(FastTextNativeLibrary.Name, EntryPoint = "fasttext_get_model_dimensions", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FastTextGetModelDimensions(IntPtr handle);

        [DllImport(FastTextNativeLibrary.Name, EntryPoint = "fasttext_predict", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FastTextPredict(
            IntPtr handle,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string text,
            int k,
            float threshold,
            ref IntPtr errPtr
        );

        [DllImport(FastTextNativeLibrary.Name, EntryPoint = "destroy_predictions", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyPredictions(IntPtr predictions);

        [DllImport(FastTextNativeLibrary.Name, EntryPoint = "fasttext_get_labels", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FastTextGetLabels(IntPtr handle);

        [DllImport(FastTextNativeLibrary.Name, EntryPoint = "destroy_labels", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyLabels(IntPtr labels);

        [DllImport(FastTextNativeLibrary.Name, EntryPoint = "fasttext_tokenize", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FastTextTokenize(IntPtr handle, string text);

        [DllImport(FastTextNativeLibrary.Name, EntryPoint = "destroy_tokens", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyTokens(IntPtr tokens);

        [DllImport(FastTextNativeLibrary.Name, EntryPoint = "fasttext_abort", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FastTextAbort(IntPtr handle);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct FastTextPredictionNativeResult
    {
        public readonly float Prob;
        public readonly IntPtr Label;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct FastTextPredictionListNativeResult
    {
        public readonly IntPtr Predictions;
        public readonly ulong Length;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct FastTextTokenListNativeResult
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
