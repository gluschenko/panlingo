using System;
using System.Runtime.InteropServices;
using Panlingo.LanguageIdentification.MediaPipe.Native;

namespace Panlingo.LanguageIdentification.MediaPipe.Internal
{
    internal static class MediaPipeDetectorWrapper
    {
        [DllImport(MediaPipeNativeLibrary.Name, EntryPoint = "language_detector_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateLanguageDetector(
            ref LanguageDetectorOptions options,
            out IntPtr errorMessage
        );

        [DllImport(MediaPipeNativeLibrary.Name, EntryPoint = "language_detector_detect", CallingConvention = CallingConvention.Cdecl)]
        public static extern int UseLanguageDetector(
            IntPtr handle,
            string text,
            ref LanguageDetectorResult result,
            out IntPtr errorMessage
        );

        [DllImport(MediaPipeNativeLibrary.Name, EntryPoint = "language_detector_close_result", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeLanguageDetectorResult(ref LanguageDetectorResult result);

        [DllImport(MediaPipeNativeLibrary.Name, EntryPoint = "language_detector_close", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FreeLanguageDetector(
            IntPtr handle,
            out IntPtr errorMessage
        );
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal readonly struct LanguageDetectorPrediction
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public readonly string LanguageCode;

        [MarshalAs(UnmanagedType.R4)]
        public readonly float Probability;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal readonly struct LanguageDetectorResult
    {
        public readonly IntPtr Predictions;
        public readonly uint PredictionsCount;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal readonly struct LanguageDetectorOptions
    {
        public readonly BaseOptions BaseOptions;
        public readonly ClassifierOptions ClassifierOptions;

        public LanguageDetectorOptions(BaseOptions baseOptions, ClassifierOptions classifierOptions)
        {
            BaseOptions = baseOptions;
            ClassifierOptions = classifierOptions;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal readonly struct BaseOptions
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public readonly string? ModelAssetBuffer;

        public readonly uint ModelAssetBufferCount;

        [MarshalAs(UnmanagedType.LPStr)]
        public readonly string? ModelAssetPath;

        public BaseOptions(string? modelAssetBuffer, int modelAssetBufferCount, string? modelAssetPath)
        {
            ModelAssetBuffer = modelAssetBuffer;
            ModelAssetBufferCount = (uint)modelAssetBufferCount;
            ModelAssetPath = modelAssetPath;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal readonly struct ClassifierOptions
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public readonly string? DisplayNamesLocale;
        public readonly int MaxResults;
        public readonly float ScoreThreshold;

        public readonly IntPtr CategoryAllowlist; // const char** in C++
        public readonly uint CategoryAllowlistCount;

        public readonly IntPtr CategoryDenylist; // const char** in C++
        public readonly uint CategoryDenylistCount;

        public ClassifierOptions(int resultCount, float scoreThreshold)
        {
            DisplayNamesLocale = null;
            MaxResults = resultCount;
            ScoreThreshold = scoreThreshold;

            CategoryAllowlist = IntPtr.Zero;
            CategoryAllowlistCount = 0;

            CategoryDenylist = IntPtr.Zero;
            CategoryDenylistCount = 0;
        }
    }
}
