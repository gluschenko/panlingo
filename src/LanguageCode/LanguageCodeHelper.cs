using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Panlingo.LanguageCode.Models;

namespace Panlingo.LanguageCode
{
    public static class LanguageCodeHelper
    {
        private static LanguageCodeResolver _defaultNormalizationOptions = new LanguageCodeResolver()
            .ToLowerAndTrim()
            .ConvertFromIETF();

        private readonly static Dictionary<LanguageCodeEntity, Func<LanguageDescriptor, string>> _langaugeVisitors;

        static LanguageCodeHelper()
        {
            _langaugeVisitors = new Dictionary<LanguageCodeEntity, Func<LanguageDescriptor, string>>
            {
                [LanguageCodeEntity.Alpha2] = x => x.Part1,
                [LanguageCodeEntity.Alpha3] = x => x.Id,
                [LanguageCodeEntity.Alpha3B] = x => x.Part2b,
                [LanguageCodeEntity.Alpha3T] = x => x.Part2t,
                [LanguageCodeEntity.EnglishName] = x => x.RefName
            };
        }

        /// <summary>
        /// Resoves code with resolver options
        /// </summary>
        /// <param name="code">Any ISO langauge code from ISO 639-1, ISO 639-2 or ISO 639-3</param>
        /// <param name="options">Resolver options</param>
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
        /// Tries to resolve code
        /// </summary>
        /// <param name="code">Any ISO langauge code from ISO 639-1, ISO 639-2 or ISO 639-3</param>
        /// <param name="value"></param>
        /// <param name="options">Resolver options</param>
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

        /// <summary>
        /// Resoves codes collection with resolver options
        /// </summary>
        /// <param name="codes">Any ISO langauge code from ISO 639-1, ISO 639-2 or ISO 639-3</param>
        /// <param name="options">Resolver options</param>
        /// <returns></returns>
        public static IDictionary<string, string> Resolve(
            IEnumerable<string> codes,
            LanguageCodeResolver? options = null
        )
        {
            options ??= _defaultNormalizationOptions;

            var result = new Dictionary<string, string>();

            foreach (var code in codes)
            {
                result[code] = options.Apply(code);
            }

            return result;
        }

        /// <summary>
        /// Tries to resolve the codes collection
        /// </summary>
        /// <param name="codes">Any ISO langauge code from ISO 639-1, ISO 639-2 or ISO 639-3</param>
        /// <param name="value"></param>
        /// <param name="options">Resolver options</param>
        /// <returns></returns>
        public static bool TryResolve(
            IEnumerable<string> codes,
            [MaybeNullWhen(false)] out IDictionary<string, string> value,
            LanguageCodeResolver? options = null
        )
        {
            try
            {
                value = Resolve(codes: codes, options: options);
                return true;
            }
            catch (LanguageCodeException)
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// Gets two letter ISO code by any langauge code
        /// </summary>
        /// <param name="code">Any ISO langauge code from ISO 639-1, ISO 639-2 or ISO 639-3</param>
        /// <returns></returns>
        /// <exception cref="LanguageCodeException"></exception>
        public static string GetTwoLetterISOCode(string code)
        {
            if (!TryGetTwoLetterISOCode(code, out var value))
            {
                throw new LanguageCodeException(code, $"Language code is unknown");
            }

            return value;
        }

        /// <summary>
        /// Gets three letter ISO code by any langauge code
        /// </summary>
        /// <param name="code">Any ISO langauge code from ISO 639-1, ISO 639-2 or ISO 639-3</param>
        /// <returns></returns>
        /// <exception cref="LanguageCodeException"></exception>
        public static string GetThreeLetterISOCode(string code)
        {
            if (!TryGetThreeLetterISOCode(code, out var value))
            {
                throw new LanguageCodeException(code, $"Language code is unknown");
            }

            return value;
        }

        /// <summary>
        /// Gets an English name of any langauge code
        /// </summary>
        /// <param name="code">Any ISO langauge code from ISO 639-1, ISO 639-2 or ISO 639-3</param>
        /// <returns></returns>
        /// <exception cref="LanguageCodeException"></exception>
        public static string GetLanguageEnglishName(string code)
        {
            if (!TryGetLanguageEnglishName(code, out var value))
            {
                throw new LanguageCodeException(code, $"Language code is unknown");
            }

            return value;
        }

        /// <summary>
        /// Gets two letter ISO code by any langauge code
        /// </summary>
        /// <param name="code">Any ISO langauge code from ISO 639-1, ISO 639-2 or ISO 639-3</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetTwoLetterISOCode(
            string code, 
            [MaybeNullWhen(false)] out string value
        )
        {
            if (TryGetEntity(code: code, entity: LanguageCodeEntity.Alpha2, out var x))
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

        /// <summary>
        /// Gets three letter ISO code by any langauge code
        /// </summary>
        /// <param name="code">Any ISO langauge code from ISO 639-1, ISO 639-2 or ISO 639-3</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetThreeLetterISOCode(
            string code, 
            [MaybeNullWhen(false)] out string value
        )
        {
            if (TryGetEntity(code: code, entity: LanguageCodeEntity.Alpha3, out var x))
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

        /// <summary>
        /// Gets an English name of any langauge code
        /// </summary>
        /// <param name="code">Any ISO langauge code from ISO 639-1, ISO 639-2 or ISO 639-3</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetLanguageEnglishName(
            string code, 
            [MaybeNullWhen(false)] out string value
        )
        {
            if (TryGetEntity(code: code, entity: LanguageCodeEntity.EnglishName, out var x))
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

        /// <summary>
        /// Gets an entity of langauge
        /// </summary>
        /// <param name="code">Any ISO langauge code from ISO 639-1, ISO 639-2 or ISO 639-3</param>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="LanguageCodeException"></exception>
        public static string GetEntity(
            string code, 
            LanguageCodeEntity entity
        )
        {
            if (!TryGetEntity(code, entity, out var value))
            {
                throw new LanguageCodeException(code, $"Entity '{entity}' is not found for this code");
            }

            return value;
        }

        /// <summary>
        /// Gets an entity of langauge
        /// </summary>
        /// <param name="code">Any ISO langauge code from ISO 639-1, ISO 639-2 or ISO 639-3</param>
        /// <param name="entity"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
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

            if (_langaugeVisitors.TryGetValue(entity, out var visitor))
            {
                var visitorValue = visitor(item);

                if (!string.IsNullOrWhiteSpace(visitorValue))
                {
                    value = visitorValue;
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
    }
}
