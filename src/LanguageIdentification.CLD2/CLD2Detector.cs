using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using Panlingo.LanguageIdentification.CLD2.Internal;

namespace Panlingo.LanguageIdentification.CLD2
{
    /// <summary>
    /// <para>Example:</para>
    /// <code>
    /// using var cld2 = new CLD2Detector();
    /// var predictions = cld2.PredictLanguage("Привіт, як справи?");
    /// </code>
    /// 
    /// <para>The using-operator is required to correctly remove unmanaged resources from memory after use.</para>
    /// </summary>
    public class CLD2Detector : IDisposable
    {
        private readonly Lazy<ImmutableHashSet<string>> _labels;

        /// <summary>
        /// <para>Creates an instance for <see cref="CLD2Detector"/>.</para>
        /// <inheritdoc cref="CLD2Detector"/>
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        public CLD2Detector()
        {
            if (!IsSupported())
            {
                throw new NotSupportedException(
                    $"{nameof(CLD2Detector)} is not yet supported on {RuntimeInformation.RuntimeIdentifier} ({RuntimeInformation.OSArchitecture})"
                );
            }

            _labels = new Lazy<ImmutableHashSet<string>>(
                () =>
                {
                    var result = ImmutableHashSet.CreateRange<string>(new string[]
                    {
                        "af", "sq", "ar", "hy", "az", "eu", "be", "bn", "bh", "bg",
                        "ca", "ceb", "chr", "hr", "cs", "zh", "zh-Hant", "da", "dv", "nl",
                        "en", "et", "fi", "fr", "gl", "lg", "ka", "de", "el", "gu",
                        "ht", "iw", "hi", "hmn", "hu", "is", "id", "iu", "ga", "it",
                        "jw", "ja", "kn", "km", "rw", "ko", "lo", "lv", "lif", "lt",
                        "mk", "ms", "ml", "mt", "mr", "ne", "no", "or", "fa", "pl",
                        "pt", "pa", "ro", "ru", "gd", "sr", "si", "sk", "sl", "es",
                        "sw", "sv", "syr", "tl", "ta", "te", "th", "tr", "uk", "ur",
                        "vi", "cy", "yi",
                    });

                    return result;
                }
            );
        }

        /// <summary>
        /// Checks the suitability of the current platform for use. Key criteria are the operating system and processor architecture
        /// </summary>
        public static bool IsSupported()
        {
            return RuntimeInformation.OSArchitecture switch
            {
                Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
                Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => true,
                Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
                Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
                _ => false,
            };
        }

        /// <summary>
        /// Produces a prediction for 'text'
        /// </summary>
        /// <param name="text">Some text in natural language</param>
        /// <returns>List of language predictions</returns>
        public IEnumerable<CLD2Prediction> PredictLanguage(string text)
        {
            var resultPtr = CLD2DetectorWrapper.PredictLanguage(
                text: text,
                resultCount: out var resultCount
            );

            try
            {
                var result = new CLD2PredictionResult[resultCount];
                var structSize = Marshal.SizeOf(typeof(CLD2PredictionResult));

                for (var i = 0; i < resultCount; i++)
                {
                    result[i] = Marshal.PtrToStructure<CLD2PredictionResult>(resultPtr + i * structSize);
                }

                return result
                    .OrderByDescending(x => x.Probability)
                    .Select(x => new CLD2Prediction(x))
                    .ToArray();
            }
            finally
            {
                CLD2DetectorWrapper.FreeResults(resultPtr, resultCount);
            }
        }

        /// <summary>
        /// Gets all languages supported by CLD2
        /// </summary>
        /// <returns>Collection of strings</returns>
        public IEnumerable<string> GetLanguages()
        {
            return _labels.Value;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

