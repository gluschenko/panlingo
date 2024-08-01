using System;
using System.Collections.Generic;
using System.Linq;
using Panlingo.LanguageCode.Models;

namespace Panlingo.LanguageCode
{
    public sealed class LanguageCodeResolver
    {
        private Dictionary<LanguageCodeRule, Func<string, string>> _rules;
        private Func<string, string>? _resolveUnknown;
        private Func<string, string>? _convert;

        public LanguageCodeResolver()
        {
            _rules = new Dictionary<LanguageCodeRule, Func<string, string>>();
        }

        /// <summary>
        /// Examples:
        /// <code>RU -> ru</code>
        /// <code>eNg -> eng</code>
        /// </summary>
        /// <returns></returns>
        public LanguageCodeResolver ToLowerAndTrim()
        {
            _rules[LanguageCodeRule.ToLowerAndTrim] = x => x.ToLower().Trim();

            return this;
        }

        /// <summary>
        /// Examples:
        /// <code>ru-RU => ru</code>
        /// <code>uk-UA => uk</code>
        /// <code>en-US => en</code>
        /// </summary>
        /// <returns></returns>
        public LanguageCodeResolver ConvertFromIETF()
        {
            _rules[LanguageCodeRule.ConvertFromIETF] = x =>
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
            };

            return this;
        }

        /// <summary>
        /// Examples:
        /// <code>azb -> aze</code>
        /// <code>azj -> aze</code>
        /// <code>cmn -> zho</code>
        /// <code>nan -> zho</code>
        /// <code>hji -> msa</code>
        /// <code>ind -> msa</code>
        /// </summary>
        /// <returns></returns>
        public LanguageCodeResolver ReduceToMacrolanguage()
        {
            _rules[LanguageCodeRule.ReduceToMacrolanguage] = x =>
            {
                if (LanguageCodeSearchIndex.MacrolanguageCodes.TryGetValue(x, out var value))
                {
                    return value;
                }

                return x;
            };

            return this;
        }

        /// <summary>
        /// Examples:
        /// <code>iw -> he</code>
        /// <code>mo -> ro</code>
        /// <code>mol -> ron</code>
        /// </summary>
        /// <param name="onConflict"></param>
        /// <returns></returns>
        public LanguageCodeResolver ConvertFromDeprecatedCode(
            Func<string, IEnumerable<string>, string>? onConflict = null
        )
        {
            _rules[LanguageCodeRule.ConvertFromDeprecatedCode] = x =>
            {
                if (LanguageCodeSearchIndex.LegacyCodes.TryGetValue(x, out var value))
                {
                    if (value.Count() == 1)
                    {
                        return value.First();
                    }
                    else if (onConflict != null)
                    {
                        return onConflict(x, value);
                    }
                }

                return x;
            };

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
        public LanguageCodeResolver ResolveUnknownCode(Func<string, string> resolver)
        {
            _resolveUnknown = resolver;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public LanguageCodeResolver ConvertTo(LanguageCodeEntity entity)
        {
            _convert = x =>
            {
                if (LanguageCodeHelper.TryGetEntity(x, entity, out var value))
                {
                    return value;
                }
                else
                {
                    x = ResolveUnknown(x);
                    return LanguageCodeHelper.GetEntity(x, entity);
                }
            };

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="LanguageCodeException"></exception>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public bool HasRule(LanguageCodeRule rule)
        {
            return _rules.ContainsKey(rule);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public LanguageCodeResolver RemoveRule(LanguageCodeRule rule)
        {
            if (HasRule(rule))
            {
                _rules.Remove(rule);
            }
            else
            {
                throw new InvalidOperationException($"Rule '{rule}' is already removed or not used");
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        internal string Apply(string code)
        {
            foreach (var rule in _rules.OrderBy(x => x.Key))
            {
                code = rule.Value(code);
            }

            if (_convert != null)
            {
                code = _convert(code);
            }

            return code;
        }
    }
}
