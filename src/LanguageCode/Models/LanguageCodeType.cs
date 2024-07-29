namespace Panlingo.LanguageCode.Models
{
    public enum LanguageCodeType
    {
        /// <summary>
        /// Two letter code from ISO 639-1
        /// </summary>
        Alpha2,
        /// <summary>
        /// Three letter code from ISO 639-3
        /// </summary>
        Alpha3,
        /// <summary>
        /// Three letter code from ISO 639-2 (terminology)
        /// </summary>
        Alpha3T,
        /// <summary>
        /// Three letter code from ISO 639-2 (bibliographic)
        /// </summary>
        Alpha3B,
        /// <summary>
        /// Langauge name in English
        /// </summary>
        EnglishName,
    }
}
