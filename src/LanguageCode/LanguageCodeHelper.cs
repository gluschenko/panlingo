using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Panlingo.LanguageCode.Models;

namespace Panlingo.LanguageCode
{
    public static class LanguageCodeHelper
    {
        private static readonly Dictionary<string, string> _twoLetterCodes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly Dictionary<string, string> _threeLetterCodes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly Dictionary<string, string> _englishNames = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly Dictionary<string, string> _legacyCodes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        private static NormalizationOptions _defaultNormalizationOptions;

        static LanguageCodeHelper()
        {
            _defaultNormalizationOptions = new NormalizationOptions().ConvertFromIETF().ConvertFromDeprecatedCode();

            static void Set(
                Dictionary<string, string> target,
                SetThreeLanguageDescriptor item,
                string value
            )
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    // ru, en, uk
                    if (!string.IsNullOrWhiteSpace(item.Part1))
                    {
                        target[item.Part1] = value;
                    }
                    // rus, eng, ukr
                    if (!string.IsNullOrWhiteSpace(item.Part2b))
                    {
                        target[item.Part2b] = value;
                    }

                    if (!string.IsNullOrWhiteSpace(item.Part2t))
                    {
                        target[item.Part2t] = value;
                    }
                }
            }

            foreach (var item in ISOGeneratorResourceProvider.ISOGeneratorResources.SetThreeLanguageDescriptorList)
            {
                Set(
                    target: _twoLetterCodes,
                    item: item,
                    value: item.Part1
                );

                Set(
                    target: _threeLetterCodes,
                    item: item,
                    value: item.Part2b
                );

                Set(
                    target: _englishNames,
                    item: item,
                    value: item.RefName
                );
            }
        }

        public static string GetTwoLetterISOCode(string code)
        {
            return TryGetTwoLetterISOCode(code, out var value)
                ? value
                : throw new LanguageCodeException(code, $"Language code is unknown");
        }

        public static string GetThreeLetterISOCode(string code)
        {
            return TryGetThreeLetterISOCode(code, out var value)
                ? value
                : throw new LanguageCodeException(code, $"Language code is unknown");
        }

        public static string GetLanguageEnglishName(string code)
        {
            return TryGetLanguageEnglishName(code, out var value)
                ? value
                : throw new LanguageCodeException(code, $"Language code is unknown");
        }

        public static bool TryGetTwoLetterISOCode(string code, [MaybeNullWhen(false)] out string value)
        {
            return _twoLetterCodes.TryGetValue(code, out value);
        }

        public static bool TryGetThreeLetterISOCode(string code, [MaybeNullWhen(false)] out string value)
        {
            return _threeLetterCodes.TryGetValue(code, out value);
        }

        public static bool TryGetLanguageEnglishName(string code, [MaybeNullWhen(false)] out string value)
        {
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

        public static string Normalize(
            string code,
            NormalizationOptions? options = null
        )
        {
            options ??= new NormalizationOptions();
            return options.Apply(code);
        }


        public class NormalizationOptions
        {
            private List<Func<string, string>> _rules;
            private Func<string, string>? _resolveUnknown;
            private Func<string, string>? _convert;

            public NormalizationOptions()
            {
                _rules = new List<Func<string, string>>();
            }

            public NormalizationOptions ToLowerAndTrim()
            {
                _rules.Add(
                    x => x.ToLower().Trim()
                );

                return this;
            }

            /// <summary>
            /// Examples:
            /// <code>iw -> he</code>
            /// <code>mo -> ro</code>
            /// <code>mol -> ron</code>
            /// </summary>
            /// <returns></returns>
            public NormalizationOptions ConvertFromDeprecatedCode()
            {
                _rules.Add(
                    x =>
                    {
                        if (_legacyCodes.TryGetValue(x, out var value))
                        {
                            return value;
                        }

                        return x;
                    }
                );

                return this;
            }

            /// <summary>
            /// Examples:
            /// <code>ru-RU => ru</code>
            /// <code>uk-UA => uk</code>
            /// <code>en-US => en</code>
            /// </summary>
            /// <returns></returns>
            public NormalizationOptions ConvertFromIETF()
            {
                _rules.Add(
                    x =>
                    {
                        if (x.Contains('-'))
                        {
                            var words = x.Split('-');

                            if (words.Length > 0)
                            {
                                return words[0];
                            }
                        }

                        return x;
                    }
                );

                return this;
            }

            /// <summary>
            /// Allows you to manually resolve unknown or conflicting codes.
            /// 
            /// Example:
            /// <code>
            /// .ResolveUnknownCode((x) => 
            /// {
            ///     // Obsolete Serbo-Croatian to actual Serbian
            ///     if (x == "sh")
            ///     {
            ///         return "sr";
            ///     }
            ///     
            ///     return x;
            /// });
            /// </code>
            /// </summary>
            /// <returns></returns>
            public NormalizationOptions ResolveUnknownCode(Func<string, string> resolver)
            {
                _resolveUnknown = resolver;
                return this;
            }

            public NormalizationOptions ConvertTo(LanguageCodeType type)
            {
                _convert = x => 
                {
                    if (type == LanguageCodeType.Alpha2)
                    {
                        if (TryGetTwoLetterISOCode(x, out var value))
                        {
                            return value;
                        }
                        else
                        {
                            return ResolveUnknown(x);
                        }
                    }

                    if (type == LanguageCodeType.Alpha3)
                    {
                        if (TryGetThreeLetterISOCode(x, out var value))
                        {
                            return value;
                        }
                        else
                        {
                            return ResolveUnknown(x);
                        }
                    }

                    throw new NotImplementedException($"Type {type} is not implemented yet");
                };

                return this;
            }

            public string Apply(string code)
            {
                foreach (var rule in _rules)
                {
                    code = rule(code);
                }

                if (_convert != null)
                {
                    code = _convert(code);
                }

                return code;
            }

            private string ResolveUnknown(string code)
            {
                if (_resolveUnknown != null)
                {
                    var previousCode = code;
                    code = _resolveUnknown(code);

                    if (code.Equals(previousCode, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new LanguageCodeException(code, $"Language code is unknown");
                    }
                }

                throw new LanguageCodeException(code, $"Language code is unknown");
            }
        }
    }
}
