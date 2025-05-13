using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Text;
using Panlingo.LanguageIdentification.Whatlang.Internal;
using Panlingo.LanguageIdentification.Whatlang.Native;

namespace Panlingo.LanguageIdentification.Whatlang
{
    /// <summary>
    /// <para>Example:</para>
    /// <code>
    /// using var whatlang = new WhatlangDetector();
    /// var prediction = whatlang.PredictLanguage("Привіт, як справи?");
    /// </code>
    /// 
    /// <para>The using-operator is required to correctly remove unmanaged resources from memory after use.</para>
    /// </summary>
    public class WhatlangDetector : IDisposable
    {
        private readonly Lazy<ImmutableHashSet<WhatlangLanguage>> _labels;

        /// <summary>
        /// <para>Creates an instance for <see cref="WhatlangDetector"/>.</para>
        /// <inheritdoc cref="WhatlangDetector"/>
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        public WhatlangDetector()
        {
            if (!IsSupported())
            {
                throw new NotSupportedException(
                    $"{nameof(WhatlangDetector)} is not yet supported on {RuntimeInformation.RuntimeIdentifier} ({RuntimeInformation.OSArchitecture})"
                );
            }

            _labels = new Lazy<ImmutableHashSet<WhatlangLanguage>>(
                () =>
                {
                    var result = ImmutableHashSet.CreateRange(Enum.GetValues<WhatlangLanguage>());
                    return result;
                }
            );
        }

        /// <summary>
        /// Checks the suitability of the current platform for use. Key criteria are the operating system and processor architecture
        /// </summary>
        public static bool IsSupported()
        {
            return WhatlangNativeLibrary.IsSupported();
        }

        /// <summary>
        /// Produces a prediction for 'text'
        /// </summary>
        /// <param name="text">Some text in natural language</param>
        /// <returns>Language prediction</returns>
        /// <exception cref="WhatlangDetectorException"></exception>
        public WhatlangPrediction? PredictLanguage(string text)
        {
            var status = WhatlangDetectorWrapper.WhatlangDetect(
                text: text,
                result: out var result
            );

            if (status == WhatlangStatus.DetectFailure)
            {
                return null;
            }

            if (status == WhatlangStatus.BadTextPtr || status == WhatlangStatus.BadOutputPtr)
            {
                throw new WhatlangDetectorException($"Failed to detect language: {status}");
            }

            return new WhatlangPrediction(result);
        }

        /// <summary>
        /// Makes script detection for 'text'
        /// </summary>
        /// <param name="text">Some text in natural language</param>
        /// <exception cref="WhatlangDetectorException"></exception>
        public WhatlangScript? PredictScript(string text)
        {
            var status = WhatlangDetectorWrapper.WhatlangDetectScript(
                text: text,
                result: out var result
            );

            if (status == WhatlangStatus.DetectFailure)
            {
                return null;
            }

            if (status == WhatlangStatus.BadTextPtr || status == WhatlangStatus.BadOutputPtr)
            {
                throw new WhatlangDetectorException($"Failed to detect language: {status}");
            }

            return result;
        }

        /// <summary>
        /// Converts <see cref="WhatlangLanguage"/> to ISO 639-3 string.
        /// </summary>
        /// <param name="language"></param>
        /// <returns>Language code according to ISO 639-3</returns>
        /// <exception cref="WhatlangDetectorException"></exception>
        public string GetLanguageCode(WhatlangLanguage language)
        {
            var stringBuilder = new StringBuilder(100);

            try
            {
                var code = WhatlangDetectorWrapper.WhatlangLangCode(language, stringBuilder, (UIntPtr)stringBuilder.Capacity);
                if (code < 0)
                {
                    throw new WhatlangDetectorException($"Language code '{language}' is not found");
                }

                var result = stringBuilder.ToString();
                return result;
            }
            finally
            {
                stringBuilder.Clear();
            }
        }

        /// <summary>
        /// Converts <see cref="WhatlangLanguage"/> to an native name of language.
        /// </summary>
        /// <param name="language"></param>
        /// <returns>English name of language</returns>
        /// <exception cref="WhatlangDetectorException"></exception>
        public string GetLanguageName(WhatlangLanguage language)
        {
            var stringBuilder = new StringBuilder(100);

            try
            {
                var code = WhatlangDetectorWrapper.WhatlangLangName(language, stringBuilder, (UIntPtr)stringBuilder.Capacity);
                if (code < 0)
                {
                    throw new WhatlangDetectorException($"Language code '{language}' is not found");
                }

                var result = stringBuilder.ToString();
                return result;
            }
            finally
            {
                stringBuilder.Clear();
            }
        }

        /// <summary>
        /// Converts <see cref="WhatlangScript"/> to an English script name.
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        /// <exception cref="WhatlangDetectorException"></exception>
        public string GetScriptName(WhatlangScript script)
        {
            var stringBuilder = new StringBuilder(100);

            try
            {
                var code = WhatlangDetectorWrapper.WhatlangScriptName(script, stringBuilder, (UIntPtr)stringBuilder.Capacity);
                if (code < 0)
                {
                    throw new WhatlangDetectorException($"Language script '{script}' is not found");
                }

                var result = stringBuilder.ToString();
                return result;
            }
            finally
            {
                stringBuilder.Clear();
            }
        }

        /// <summary>
        /// Converts <see cref="WhatlangLanguage"/> to an English name of language.
        /// </summary>
        /// <param name="language"></param>
        /// <returns>English name of language</returns>
        /// <exception cref="WhatlangDetectorException"></exception>
        public string GetLanguageEnglishName(WhatlangLanguage language)
        {
            var stringBuilder = new StringBuilder(100);

            try
            {
                var code = WhatlangDetectorWrapper.WhatlangLangEngName(language, stringBuilder, (UIntPtr)stringBuilder.Capacity);
                if (code < 0)
                {
                    throw new WhatlangDetectorException($"Language code '{language}' is not found");
                }

                var result = stringBuilder.ToString();
                return result;
            }
            finally
            {
                stringBuilder.Clear();
            }
        }

        /// <summary>
        /// Gets all languages supported by Whatlang
        /// </summary>
        /// <returns>Collection of strings</returns>
        public IEnumerable<WhatlangLanguage> GetLanguages()
        {
            return _labels.Value;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

}
