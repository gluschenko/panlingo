using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Panlingo.LangaugeCode;
using Panlingo.LangaugeCode.Core.Models;

namespace Panlingo.LanguageCode
{
    public static class LanguageCodeHelper
    {
        private static readonly Dictionary<string, string> _twoLetterCodes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly Dictionary<string, string> _threeLetterCodes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly Dictionary<string, string> _englishNames = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly Dictionary<string, string> _legacyCodes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        static LanguageCodeHelper()
        {
            static void Set(
                Dictionary<string, string> target,
                SetThreeLanguageDescriptor culture,
                string value
            )
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    // ru, en, uk
                    if (!string.IsNullOrWhiteSpace(culture.Part1))
                    {
                        target[culture.Part1] = value;
                    }
                    // rus, eng, ukr
                    if (!string.IsNullOrWhiteSpace(culture.Part2b))
                    {
                        target[culture.Part2b] = value;
                    }

                    if (!string.IsNullOrWhiteSpace(culture.Part2t))
                    {
                        target[culture.Part2t] = value;
                    }
                }
            }

            var cultures = ISOGeneratorResources.SetThreeLanguageDescriptorList;

            foreach (var culture in cultures)
            {
                Set(
                    target: _twoLetterCodes,
                    culture: culture,
                    value: culture.Part1
                );

                Set(
                    target: _threeLetterCodes,
                    culture: culture,
                    value: culture.Part2b
                );

                Set(
                    target: _englishNames,
                    culture: culture,
                    value: culture.RefName
                );
            }
        }

        public static string GetTwoLetterISOCode(string code)
        {
            return TryGetTwoLetterISOCode(code, out var value)
                ? value
                : throw new Exception($"Language code is unknown: {code}");
        }

        public static string GetThreeLetterISOCode(string code)
        {
            return TryGetThreeLetterISOCode(code, out var value)
                ? value
                : throw new Exception($"Language code is unknown: {code}");
        }

        public static string GetLanguageEnglishName(string code)
        {
            return TryGetLanguageEnglishName(code, out var value)
                ? value
                : throw new Exception($"Language code is unknown: {code}");
        }

        public static bool TryGetTwoLetterISOCode(string code, [MaybeNullWhen(false)] out string value)
        {
            code = NormalizeCode(code);
            return _twoLetterCodes.TryGetValue(code, out value);
        }

        public static bool TryGetThreeLetterISOCode(string code, [MaybeNullWhen(false)] out string value)
        {
            code = NormalizeCode(code);
            return _threeLetterCodes.TryGetValue(code, out value);
        }

        public static bool TryGetLanguageEnglishName(string code, [MaybeNullWhen(false)] out string value)
        {
            code = NormalizeCode(code);
            return _englishNames.TryGetValue(code, out value);
        }

        public static string NormalizeCode(string code)
        {
            if (_legacyCodes.TryGetValue(code, out var value))
            {
                code = value;
            }

            return code;
        }
    }

    [Flags]
    public enum LangaugeCodeNormalizationOptions
    {
        ConvertDeprecatedCodes = 1 << 0,
        StripIETF = 1 << 1,
        ToLowerCase = 1 << 2,
    }
}
