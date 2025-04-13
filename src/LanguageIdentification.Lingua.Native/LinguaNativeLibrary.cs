using System.Runtime.InteropServices;

namespace Panlingo.LanguageIdentification.Lingua.Native
{
    public static class LinguaNativeLibrary
    {
        /// <summary>
        /// Name of native binary
        /// </summary>
        public const string Name = "lingua";

        public static bool IsSupported()
        {
            return RuntimeInformation.OSArchitecture switch
            {
                Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
#if ALL_TARGETS
                Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => true,
                Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
#endif
                _ => false,
            };
        }
    }
}
