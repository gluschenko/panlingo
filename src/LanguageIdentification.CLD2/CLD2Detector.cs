using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Panlingo.LanguageIdentification.CLD2.Internal;

namespace Panlingo.LanguageIdentification.CLD2
{
    /// <summary>
    /// .NET wrapper for CLD2
    /// </summary>
    public class CLD2Detector : IDisposable
    {
        public CLD2Detector()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new NotSupportedException(
                    $"{nameof(CLD2Detector)} is not yet supported on {RuntimeInformation.RuntimeIdentifier}"
                );
            }
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

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

