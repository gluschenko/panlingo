using System;
using System.Runtime.InteropServices;
using Panlingo.LanguageIdentification.MediaPipe.Native;

namespace Panlingo.LanguageIdentification.MediaPipe.Internal
{
    internal static class MediaPipeDetectorWrapper
    {
        [DllImport(MediaPipeNativeLibrary.Name, EntryPoint = "language_detector_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateLangaugeDetector(
            LanguageDetectorOptions options,
            ref IntPtr errorMessage
        );

        [DllImport(MediaPipeNativeLibrary.Name, EntryPoint = "language_detector_detect", CallingConvention = CallingConvention.Cdecl)]
        public static extern int UseLangaugeDetector(
            IntPtr handle,
            string text,
            ref LanguageDetectorResult result,
            ref IntPtr errorMessage
        );

        [DllImport(MediaPipeNativeLibrary.Name, EntryPoint = "language_detector_close_result", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeLangaugeDetectorResult(IntPtr handle);

        [DllImport(MediaPipeNativeLibrary.Name, EntryPoint = "language_detector_close", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeLangaugeDetector(IntPtr handle);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal readonly struct LanguageDetectorPrediction
    {
        public readonly IntPtr LanguageCode;

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

        public BaseOptions(string unmanagedBuffer, int modelAssetBufferCount)
        {
            ModelAssetBuffer = unmanagedBuffer;
            ModelAssetBufferCount = (uint)modelAssetBufferCount;
            ModelAssetPath = null;
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

        public ClassifierOptions(int maxResults)
        {
            DisplayNamesLocale = null;
            MaxResults = maxResults;
            ScoreThreshold = 0.1f;

            CategoryAllowlist = IntPtr.Zero;
            CategoryAllowlistCount = 0;

            CategoryDenylist = IntPtr.Zero;
            CategoryDenylistCount = 0;
        }
    }
}
