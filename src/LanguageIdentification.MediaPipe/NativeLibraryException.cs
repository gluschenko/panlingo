using System;

namespace Panlingo.LanguageIdentification.MediaPipe
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
