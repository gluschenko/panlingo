using System.Runtime.InteropServices;
using Panlingo.LanguageIdentification.CLD2.Native;

namespace Panlingo.LanguageIdentification.CLD2
{
    internal static class CLD2DetectorWrapper
    {
        [DllImport(CLD2NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern nint PredictLanguage(string text, out int resultCount);

        [DllImport(CLD2NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeResults(nint results, int count);
    }
}
