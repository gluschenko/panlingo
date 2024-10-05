using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Panlingo.LanguageIdentification.CLD3.Internal;

namespace Panlingo.LanguageIdentification.CLD3
{
    /// <summary>
    /// .NET wrapper for CLD3
    /// </summary>
    public class CLD3Detector : IDisposable
    {
        private readonly IntPtr _identifier;
        private readonly SemaphoreSlim _semaphore;

        public CLD3Detector(int minNumBytes, int maxNumBytes)
        {
            if (!IsSupported())
            {
                throw new NotSupportedException(
                    $"{nameof(CLD3Detector)} is not yet supported on {RuntimeInformation.RuntimeIdentifier}"
                );
            }

            _identifier = CLD3DetectorWrapper.CreateCLD3(minNumBytes, maxNumBytes);
            _semaphore = new(1, 1);
        }

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

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            try
            {
                _semaphore.Wait();
                CLD3DetectorWrapper.DestroyCLD3(_identifier);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Produces a prediction for 'text'
        /// </summary>
        /// <param name="text">Some text in natural language</param>
        /// <returns>List of language predictions</returns>
        public CLD3Prediction PredictLanguage(string text)
        {
            var resultPtr = CLD3DetectorWrapper.PredictLanguage(
                identifier: _identifier,
                text: text,
                resultCount: out var resultCount
            );

            try
            {
                var nativeResult = new CLD3PredictionResult[resultCount];
                var structSize = Marshal.SizeOf(typeof(CLD3PredictionResult));

                for (var i = 0; i < resultCount; i++)
                {
                    nativeResult[i] = Marshal.PtrToStructure<CLD3PredictionResult>(resultPtr + i * structSize);
                }

                var firstItem = nativeResult.First();
                return new CLD3Prediction(firstItem);
            }
            finally
            {
                CLD3DetectorWrapper.DestroyPredictionResult(resultPtr, resultCount);
            }
        }

        /// <summary>
        /// Produces a prediction for 'text'
        /// </summary>
        /// <param name="text">Some text in natural language</param>
        /// <param name="count">Number of predictions</param>
        /// <returns>List of language predictions</returns>
        public IEnumerable<CLD3Prediction> PredictLanguages(
            string text,
            int count
        )
        {
            var resultPtr = CLD3DetectorWrapper.PredictLanguages(
                identifier: _identifier,
                text: text,
                numLangs: count,
                resultCount: out var resultCount
            );

            try
            {
                var nativeResult = new CLD3PredictionResult[resultCount];
                var structSize = Marshal.SizeOf(typeof(CLD3PredictionResult));

                for (var i = 0; i < resultCount; i++)
                {
                    nativeResult[i] = Marshal.PtrToStructure<CLD3PredictionResult>(resultPtr + i * structSize);
                }

                return nativeResult
                    .OrderByDescending(x => x.Probability)
                    .Select(x => new CLD3Prediction(x))
                    .ToArray();
            }
            finally
            {
                CLD3DetectorWrapper.DestroyPredictionResult(resultPtr, resultCount);
            }
        }
    }

}
