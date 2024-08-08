using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Panlingo.LanguageCode.Models;

namespace Panlingo.LanguageCode
{
    public sealed class LanguageCodeResolver
    {
        /// <summary>
        /// Callback that returns one of candidates
        /// </summary>
        /// <param name="sourceCode">Initial code</param>
        /// <param name="candidates">Potential candidates to resolve</param>
        /// <returns>One of candidates</returns>
        public delegate string ConflictCallback(string sourceCode, IEnumerable<string> candidates);
        public delegate string ResolveCallback(string sourceCode);

        private delegate bool TryConvertCallback(
            string code, 
            [MaybeNullWhen(false)] out string newCode, 
            [MaybeNullWhen(true)] out string reason
        );

        private Dictionary<LanguageCodeRule, ResolveCallback> _rules;
        private ResolveCallback? _resolveUnknown;
        private TryConvertCallback? _tryConvert;

        public LanguageCodeResolver()
        {
            _rules = new Dictionary<LanguageCodeRule, ResolveCallback>();
        }

        /// <summary>
        /// Examples:
        /// <code>RU -> ru</code>
        /// <code>Eng -> eng</code>
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
        /// <param name="onConflict"><see cref="ConflictCallback"/></param>
        /// <returns></returns>
        public LanguageCodeResolver ConvertFromDeprecatedCode(
            ConflictCallback? onConflict = null
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
        public LanguageCodeResolver ResolveUnknownCode(ResolveCallback resolver)
        {
            _resolveUnknown = resolver;
            return this;
        }

        /// <summary>
        /// Selects the entity of language
        /// </summary>
        /// <param name="entity">Entity of language (e.g. language code or language name)</param>
        /// <returns></returns>
        public LanguageCodeResolver Select(LanguageCodeEntity entity)
        {
            bool TryConvertInternal(
                string code,
#if NET5_0_OR_GREATER
                [MaybeNullWhen(false)] out string newCode,
#else
                out string newCode,
#endif
#if NET5_0_OR_GREATER
                [MaybeNullWhen(true)] out string reason
#else
                out string reason
#endif
            )
            {
                if (LanguageCodeHelper.TryGetEntity(code, entity, out var value))
                {
                    newCode = value;
                    reason = null!;
                    return true;
                }
                else
                {
                    if (TryResolveUnknown(code, out var value2))
                    {
                        code = value2;
                    }
                    else
                    {
                        newCode = null!;
                        reason = "Language code is unknown";
                        return false;
                    }

                    if (LanguageCodeHelper.TryGetEntity(code, entity, out value))
                    {
                        newCode = value;
                        reason = null!;
                        return true;
                    }
                    else
                    {
                        newCode = null!;
                        reason = $"Entity '{entity}' is not found for this code";
                        return false;
                    }
                }
            }

            _tryConvert = TryConvertInternal;
            return this;
        }

        private bool TryResolveUnknown(string code, [MaybeNullWhen(false)] out string newCode)
        {
            if (_resolveUnknown != null)
            {
                var previousCode = code;
                code = _resolveUnknown(code);

                // If there is no changes after custom resolver
                if (code.Equals(previousCode, StringComparison.OrdinalIgnoreCase))
                {
                    newCode = null;
                    return false;
                }

                newCode = code;
                return true;
            }

            newCode = null;
            return false;
        }

        public bool HasRule(LanguageCodeRule rule)
        {
            return _rules.ContainsKey(rule);
        }

        /// <summary>
        /// Removes a previously added rule
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

        internal bool TryApply(
            string code, 
            [MaybeNullWhen(false)] out string newCode, 
            [MaybeNullWhen(true)] out string reason
        )
        {
            foreach (var rule in _rules.OrderBy(x => x.Key))
            {
                code = rule.Value(code);
            }

            if (_tryConvert != null)
            {
                if (_tryConvert(code, out var value, out reason))
                {
                    newCode = value;
                    return true;
                }
                else
                {
                    newCode = null;
                    return false;
                }
            }
            else
            {
                newCode = code;
                reason = null;
                return true;
            }
        }
    }
}
