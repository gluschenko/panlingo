using System.Runtime.InteropServices;

namespace LanguageIdentification.FastText.Native
{
    public class FastTextNativeLibrary
    {
        public static void LoadNativeLibrary()
        {
            NativeLibrary.Load("libfasttext.so");
        }
    }
}

