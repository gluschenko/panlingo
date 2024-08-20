﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Panlingo.LanguageIdentification.Lingua.Internal;

namespace Panlingo.LanguageIdentification.Lingua
{
    /// <summary>
    /// .NET wrapper for Lingua
    /// </summary>
    public class LinguaDetector : IDisposable
    {
        private IntPtr _detector;

        internal LinguaDetector(LinguaDetectorBuilder builder)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new NotSupportedException(
                    $"{nameof(LinguaDetector)} is not yet supported on {RuntimeInformation.RuntimeIdentifier}"
                );
            }

            _detector = LinguaDetectorWrapper.LinguaLanguageDetectorCreate(builder.GetNativePointer());
            if (_detector == IntPtr.Zero)
            {
                throw new LinguaDetectorException("Failed to create LanguageDetector");
            }
        }

        /// <summary>
        /// Produces a prediction for 'text'
        /// </summary>
        /// <param name="text">Some text in natural language</param>
        /// <returns>Language prediction</returns>
        /// <exception cref="LinguaDetectorException"></exception>
        public LinguaPrediction? PredictLanguage(string text)
        {
            var status = LinguaDetectorWrapper.LinguaDetectSingle(
                detector: _detector,
                text: text,
                result: out var result
            );

            if (status == LinguaStatus.DetectFailure)
            {
                return null;
            }

            if (status == LinguaStatus.BadTextPtr || status == LinguaStatus.BadOutputPtr)
            {
                throw new LinguaDetectorException($"Failed to detect language: {status}");
            }

            return new LinguaPrediction(result);
        }

        /// <summary>
        /// Produces a prediction for 'text'
        /// </summary>
        /// <param name="text">Some text in natural language</param>
        /// <returns>Language prediction</returns>
        /// <exception cref="LinguaDetectorException"></exception>
        public IEnumerable<LinguaPrediction> PredictLanguages(string text)
        {
            var status = LinguaDetectorWrapper.LinguaDetectMultiple(
                detector: _detector,
                text: text,
                result: out var nativeResult
            );

            if (status == LinguaStatus.DetectFailure)
            {
                return Array.Empty<LinguaPrediction>();
            }

            if (status == LinguaStatus.BadTextPtr || status == LinguaStatus.BadOutputPtr)
            {
                throw new LinguaDetectorException($"Failed to detect language: {status}");
            }

            try
            {
                var result = new LinguaPredictionResult[nativeResult.PredictionsCount];
                var structSize = Marshal.SizeOf(typeof(LinguaPredictionResult));

                for (var i = 0; i < nativeResult.PredictionsCount; i++)
                {
                    result[i] = Marshal.PtrToStructure<LinguaPredictionResult>(nativeResult.Predictions + i * structSize);
                }

                return result
                    .OrderByDescending(x => x.Confidence)
                    .Select(x => new LinguaPrediction(x))
                    .ToArray();
            }
            finally
            {
                LinguaDetectorWrapper.LinguaPredictionResultDestroy(nativeResult.Predictions);
            }
        }

        public string GetLanguageCode(LinguaLanguage language, LinguaLanguageCode code)
        {
            var stringBuider = new StringBuilder(10);

            try
            {
                var status = LinguaDetectorWrapper.LinguaLangCode(
                    lang: language, 
                    code: code, 
                    buffer: stringBuider, 
                    bufferSize: (UIntPtr)stringBuider.Capacity
                );

                if (status < 0)
                {
                    throw new LinguaDetectorException($"Language code '{language}' is not found");
                }

                var result = stringBuider.ToString();
                return result;
            }
            finally
            {
                stringBuider.Clear();
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (_detector != IntPtr.Zero)
            {
                LinguaDetectorWrapper.LinguaLanguageDetectorDestroy(_detector);
                _detector = IntPtr.Zero;
            }
        }
    }

}
