using System.Runtime.InteropServices;

namespace Panlingo.LanguageIdentification.CLD2.Native
{
    public static class CLD2NativeLibrary
    {
        /// <summary>
        /// Name of native binary
        /// </summary>
        public const string Name = "libcld2";

        public static bool IsSupported()
        {

            return RuntimeInformation.OSArchitecture switch
            {
                Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
#if ALL_TARGETS
                Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => true,
                Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
                Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
#endif
                _ => false,
            };
        }
    }
}
