using System;

namespace Panlingo.LanguageCode.Models
{
    public sealed class LanguageCodeException : Exception
    {
        public string? Code { get; private set; }

        public LanguageCodeException() : base()
        {
        }

        public LanguageCodeException(string message) : base(message)
        {
        }

        public LanguageCodeException(string code, string message) : this($"[{code ?? "NULL"}] {message}")
        {
            Code = code;
        }
    }
}
