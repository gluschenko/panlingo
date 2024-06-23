using System.Runtime.InteropServices;

namespace LanguageIdentification.CLD3.Native
{
    public static class CLD3NativeLibrary
    {
        public static void LoadNativeLibrary()
        {
            NativeLibrary.Load("libcld3.so");
        }
    }
}
