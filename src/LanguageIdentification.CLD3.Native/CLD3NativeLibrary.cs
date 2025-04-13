using System.Runtime.InteropServices;

namespace Panlingo.LanguageIdentification.CLD3.Native
{
    public static class CLD3NativeLibrary
    {
        /// <summary>
        /// Name of native binary
        /// </summary>
        public const string Name = "libcld3";

        public static bool IsSupported()
        {
            return RuntimeInformation.OSArchitecture switch
            {
                Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
#if ALL_TARGETS
                Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => true,
                // TODO: Find out why CLD3 is crashing on macOS
                // Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
                // Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
#endif
                _ => false,
            };
        }
    }
}
