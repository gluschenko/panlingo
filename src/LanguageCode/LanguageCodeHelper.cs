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
            if (TryGetEntity(code, LanguageCodeEntity.Alpha2, out var x))
            {
                value = x;
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
            if (TryGetEntity(code, LanguageCodeEntity.Alpha3, out var x))
            {
                value = x;
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
            if (TryGetEntity(code, LanguageCodeEntity.EnglishName, out var x))
            {
                value = x;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public static string GetEntity(
            string code, 
            LanguageCodeEntity entity
        )
        {
            if (TryGetEntity(code, entity, out var value))
            {
                return value;
            }

            throw new LanguageCodeException(code, $"Entity '{entity}' is not found for this code");
        }

        public static bool TryGetEntity(
            string code, 
            LanguageCodeEntity entity, 
            [MaybeNullWhen(false)] out string value
        )
        {
            if (!_langauges.TryGetValue(code, out var item))
            {
                value = null;
                return false;
            }

            if (entity == LanguageCodeEntity.Alpha2)
            {
                if (!string.IsNullOrWhiteSpace(item.Part1))
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
            else if (entity == LanguageCodeEntity.Alpha3)
            {
                if (!string.IsNullOrWhiteSpace(item.Id))
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
            else if (entity == LanguageCodeEntity.Alpha3B)
            {
                if (!string.IsNullOrWhiteSpace(item.Part2b))
                {
                    value = item.Part2b;
                    return true;
                }
                else
                {
                    value = null;
                    return false;
                }
            }
            else if (entity == LanguageCodeEntity.Alpha3T)
            {
                if (!string.IsNullOrWhiteSpace(item.Part2t))
                {
                    value = item.Part2t;
                    return true;
                }
                else
                {
                    value = null;
                    return false;
                }
            }
            else
            {
                throw new NotImplementedException($"Type '{entity}' is not implemented yet");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string Normalize(
            string code,
            NormalizationOptions? options = null
        )
        {
            options ??= _defaultNormalizationOptions;
            return options.Apply(code);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <returns></returns>
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
            ///     // Obsolete Moldovan to actual Romanian
            ///     if (x == "mo")
            ///     {
            ///         return "ro";
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

            public NormalizationOptions ConvertTo(LanguageCodeEntity entity)
            {
                _convert = x => 
                {
                    if (TryGetEntity(x, entity, out var value))
                    {
                        return value;
                    }
                    else
                    {
                        x = ResolveUnknown(x);
                        return GetEntity(x, entity);
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
