using System.Runtime.InteropServices;

namespace LanguageIdentification.Whatlang.Native
{
    public static class WhatlangNativeLibrary
    {
        public const string Name = "libwhatlang.so";

        public static void LoadNativeLibrary()
        {
            NativeLibrary.Load(Name);
        }
    }
}
