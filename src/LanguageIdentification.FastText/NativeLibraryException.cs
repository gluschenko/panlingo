using System;

namespace Panlingo.LanguageIdentification.FastText
{
    public class NativeLibraryException : Exception
    {
        public NativeLibraryException()
        {
        }

        public NativeLibraryException(string message) : base(message)
        {
        }
    }
}
