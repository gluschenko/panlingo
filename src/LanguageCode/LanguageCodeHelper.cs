using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Panlingo.LanguageCode.Models;

namespace Panlingo.LanguageCode
{
    public static class LanguageCodeHelper
    {
        private static readonly Dictionary<string, SetThreeLanguageDescriptor> _langauges = 
            new Dictionary<string, SetThreeLanguageDescriptor>(StringComparer.InvariantCultureIgnoreCase);

        private static readonly Dictionary<string, string> _legacyCodes = 
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        private static NormalizationOptions _defaultNormalizationOptions = new NormalizationOptions()
            .ToLowerAndTrim()
            .ConvertFromIETF()
            .ConvertFromDeprecatedCode();

        static LanguageCodeHelper()
        {
            foreach (var item in ISOGeneratorResourceProvider.ISOGeneratorResources.SetThreeLanguageDescriptorList)
            {
                if (!string.IsNullOrWhiteSpace(item.Id))
                {
                    _langauges[item.Id] = item;
                }

                if (!string.IsNullOrWhiteSpace(item.Part1))
                {
                    _langauges[item.Part1] = item;
                }

                if (!string.IsNullOrWhiteSpace(item.Part2t))
                {
                    _langauges[item.Part2t] = item;
                }

                if (!string.IsNullOrWhiteSpace(item.Part2b))
                {
                    _langauges[item.Part2b] = item;
                }
            }

            foreach (var item in ISOGeneratorResourceProvider.ISOGeneratorResources.SetTwoLanguageDeprecationDescriptorList)
            {
                if (!string.IsNullOrWhiteSpace(item.CodeAlpha2) && !string.IsNullOrWhiteSpace(item.CodeAlpha2Deprecated))
                {
                    _legacyCodes[item.CodeAlpha2Deprecated] = item.CodeAlpha2;
                }

                if (!string.IsNullOrWhiteSpace(item.CodeAlpha3) && !string.IsNullOrWhiteSpace(item.CodeAlpha3Deprecated))
                {
                    _legacyCodes[item.CodeAlpha3Deprecated] = item.CodeAlpha3;
                }
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
            if (_langauges.TryGetValue(code, out var item) && !string.IsNullOrWhiteSpace(item.Part1))
            {
                value = item.Part1;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public static bool TryGetThreeLetterISOCode(string code, [MaybeNullWhen(false)] out string value)
        {
            if (_langauges.TryGetValue(code, out var item) && !string.IsNullOrWhiteSpace(item.Id))
            {
                value = item.Id;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public static bool TryGetLanguageEnglishName(string code, [MaybeNullWhen(false)] out string value)
        {
            if (_langauges.TryGetValue(code, out var item) && !string.IsNullOrWhiteSpace(item.RefName))
            {
                value = item.RefName;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public static string Normalize(
            string code,
            NormalizationOptions? options = null
        )
        {
            options ??= _defaultNormalizationOptions;
            return options.Apply(code);
        }

        public static bool TryNormalize(
            string code,
            [MaybeNullWhen(false)] out string value,
            NormalizationOptions? options = null
        )
        {
            try
            {
                value = Normalize(code: code, options: options);
                return true;
            }
            catch (LanguageCodeException)
            {
                value = null;
                return false;
            }
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

            public NormalizationOptions ReduceMacrolanguage()
            {
                // TODO

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
                            x = ResolveUnknown(x);
                            return GetTwoLetterISOCode(x);
                        }
                    }
                    else if (type == LanguageCodeType.Alpha3)
                    {
                        if (TryGetThreeLetterISOCode(x, out var value))
                        {
                            return value;
                        }
                        else
                        {
                            x = ResolveUnknown(x);
                            return GetThreeLetterISOCode(x);
                        }
                    }
                    else
                    {
                        throw new NotImplementedException($"Type {type} is not implemented yet");
                    }
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

                    // If there is no changes after custom resolver
                    if (code.Equals(previousCode, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new LanguageCodeException(code, $"Language code is unknown");
                    }

                    return code;
                }

                throw new LanguageCodeException(code, $"Language code is unknown");
            }
        }
    }
}
