using System.Runtime.InteropServices;

namespace LanguageIdentification.FastText.Native
{
    public class FastTextNativeLibrary
    {
        public const string Name = "libfasttext.so";

        public static void LoadNativeLibrary()
        {
            NativeLibrary.Load(Name);
        }
    }
}

