using System.Runtime.InteropServices;

namespace Panlingo.LanguageIdentification.MediaPipe.Native
{
    public class MediaPipeNativeLibrary
    {
        /// <summary>
        /// Name of native binary
        /// </summary>
        public const string Name = "mediapipe_language_detector";
        /// <summary>
        /// Name of model
        /// </summary>
        public const string ModelName = "mediapipe_language_detector.tflite";

        public static bool IsSupported()
        {
            return RuntimeInformation.OSArchitecture switch
            {
                Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
#if ALL_TARGETS
                Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
                Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => true,
                Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
#endif
                _ => false,
            };
        }
    }
}

