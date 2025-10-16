namespace Panlingo.LanguageIdentification.Tests.Helpers
{
    internal class Constants
    {
        public const double EPSILON = 0.0001;

        public const string PHRASE_ENG_1 = "Hello, how are you?";
        public const string PHRASE_UKR_1 = "Привіт, як справи?";
        public const string PHRASE_RUS_1 = "Привет, как дела?";

        public const string PHRASE_MIXED_1 = PHRASE_ENG_1 + " " + PHRASE_RUS_1 + " " + PHRASE_UKR_1;

        public const string MALFORMED_BYTES_0 = "";

        // NUL-bytes is string
        public const string MALFORMED_BYTES_1 = "hello\0world";
        public const string MALFORMED_BYTES_2 = "\0";

        // continuation / non-ascii single bytes represented as Unicode control range
        public const string MALFORMED_BYTES_3 = "\u0080";
        public const string MALFORMED_BYTES_4 = "\u00BF";

        // Truncated Multibyte/Unpaired Surrogates
        public const string MALFORMED_BYTES_5 = "\uD800";
        public const string MALFORMED_BYTES_6 = "\uDC00";

        // Overlong / invalid sequences
        public const string MALFORMED_BYTES_7 = "\uFFFF";
        public const string MALFORMED_BYTES_8 = "\uFFFE";

        // BOM / ZERO WIDTH / BiDi controls
        public const string MALFORMED_BYTES_9 = "\uFEFF" + "Hello";
        public const string MALFORMED_BYTES_10 = "A" + "\u200D" + "B";
        public const string MALFORMED_BYTES_11 = "A" + "\u200C" + "B";
        public const string MALFORMED_BYTES_12 = "abc" + "\u202E" + "def";
    }
}
