using System;
using System.Diagnostics.CodeAnalysis;
using Panlingo.LanguageCode.Models;

namespace Panlingo.LanguageCode
{
    public static class LanguageCodeHelper
    {
        private static LanguageCodeResolver _defaultNormalizationOptions = new LanguageCodeResolver()
            .ToLowerAndTrim()
            .ConvertFromIETF()
            .ConvertFromDeprecatedCode();

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

        public static bool TryGetTwoLetterISOCode(
            string code, 
            [MaybeNullWhen(false)] out string value
        )
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

        public static bool TryGetThreeLetterISOCode(
            string code, 
            [MaybeNullWhen(false)] out string value
        )
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

        public static bool TryGetLanguageEnglishName(
            string code, 
            [MaybeNullWhen(false)] out string value
        )
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
            if (!LanguageCodeSearchIndex.Langauges.TryGetValue(code, out var item))
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
            else if (entity == LanguageCodeEntity.EnglishName)
            {
                if (!string.IsNullOrWhiteSpace(item.RefName))
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
        public static string Resolve(
            string code,
            LanguageCodeResolver? options = null
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
        public static bool TryResolve(
            string code,
            [MaybeNullWhen(false)] out string value,
            LanguageCodeResolver? options = null
        )
        {
            try
            {
                value = Resolve(code: code, options: options);
                return true;
            }
            catch (LanguageCodeException)
            {
                value = null;
                return false;
            }
        }
    }
}
