using System.Runtime.InteropServices;

namespace Panlingo.LanguageIdentification.Whatlang.Native
{
    public static class WhatlangNativeLibrary
    {
        /// <summary>
        /// Name of native binary
        /// </summary>
        public const string Name = "whatlang";

        public static bool IsSupported()
        {
            return RuntimeInformation.OSArchitecture switch
            {
                Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
#if ALL_TARGETS
                Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
                Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => true,
                Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => true,
                Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
                Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
#endif
                _ => false,
            };
        }
    }
}
