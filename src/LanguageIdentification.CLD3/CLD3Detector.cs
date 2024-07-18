using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Panlingo.LanguageIdentification.CLD3.Internal;

namespace Panlingo.LanguageIdentification.CLD3
{
    public class CLD3Detector : IDisposable
    {
        private readonly IntPtr _identifier;
        private readonly SemaphoreSlim _semaphore;

        public CLD3Detector(int minNumBytes, int maxNumBytes)
        {
            _identifier = CLD3DetectorWrapper.CreateIdentifier(minNumBytes, maxNumBytes);
            _semaphore = new(1, 1);
        }

        public void Dispose()
        {
            try
            {
                _semaphore.Wait();
                GC.SuppressFinalize(this);
                CLD3DetectorWrapper.FreeIdentifier(_identifier);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public CLD3Prediction PredictLanguage(string text)
        {
            var result = CLD3DetectorWrapper.FindLanguage(_identifier, text);
            return new CLD3Prediction(result);
        }

        public IEnumerable<CLD3Prediction> PredictLangauges(
            string text,
            int numLangs
        )
        {
            var resultPtr = CLD3DetectorWrapper.FindLanguages(
                identifier: _identifier,
                text: text,
                numLangs: numLangs,
                resultCount: out var resultCount
            );

            try
            {
                var result = new CLD3PredictionResult[resultCount];
                var structSize = Marshal.SizeOf(typeof(CLD3PredictionResult));

                for (var i = 0; i < resultCount; i++)
                {
                    result[i] = Marshal.PtrToStructure<CLD3PredictionResult>(resultPtr + i * structSize);
                }

                return result
                    .OrderByDescending(x => x.Probability)
                    .Select(x => new CLD3Prediction(x))
                    .ToArray();
            }
            finally
            {
                CLD3DetectorWrapper.FreeResults(resultPtr, resultCount);
            }
        }
    }

}
