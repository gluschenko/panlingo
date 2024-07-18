﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Panlingo.LanguageIdentification.FastText.Internal;

namespace Panlingo.LanguageIdentification.FastText
{
    public class FastTextDetector : IDisposable
    {
        private IntPtr _fastText;
        private readonly SemaphoreSlim _semaphore;

        public FastTextDetector()
        {
            _fastText = FastTextDetectorWrapper.CreateFastText();
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public string ModelPath { get; private set; } = string.Empty;

        public void LoadModel(string path)
        {
            _semaphore.Wait();
            try
            {
                var errptr = IntPtr.Zero;
                FastTextDetectorWrapper.FastTextLoadModel(_fastText, path, ref errptr);
                CheckError(errptr);

                ModelPath = path;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public IEnumerable<FastTextLabelResult> GetLabels()
        {
            var labelsPtr = FastTextDetectorWrapper.FastTextGetLabels(_fastText);

            if (labelsPtr == IntPtr.Zero)
            {
                return Array.Empty<FastTextLabelResult>();
            }

            var labelsStruct = Marshal.PtrToStructure<FastTextLabels>(labelsPtr);
            var result = new List<FastTextLabelResult>();

            for (ulong i = 0; i < labelsStruct.Length; i++)
            {
                var labelPtr = Marshal.ReadIntPtr(labelsStruct.Labels, (int)i * IntPtr.Size);
                var label = DecodeString(labelPtr);
                var freq = Marshal.ReadInt64(labelsStruct.Freqs, (int)i * sizeof(long));
                result.Add(new FastTextLabelResult(label: label, frequency: freq));
            }

            FastTextDetectorWrapper.DestroyLabels(labelsPtr);
            return result;
        }

        public IEnumerable<FastTextPredictionResult> Predict(string text, int k, float threshold = 0.0f)
        {
            var errptr = IntPtr.Zero;
            var predictionPtr = FastTextDetectorWrapper.FastTextPredict(
                handle: _fastText, text: text,
                k: k,
                threshold: threshold,
                ref errptr
            );

            CheckError(errptr);

            if (predictionPtr == IntPtr.Zero)
            {
                return Array.Empty<FastTextPredictionResult>();
            }

            var predictions = Marshal.PtrToStructure<FastTextPredictions>(predictionPtr);
            var result = new List<FastTextPredictionResult>();

            for (ulong i = 0; i < predictions.Length; i++)
            {
                IntPtr elementPtr = new IntPtr(predictions.Predictions.ToInt64() + (long)(i * (uint)Marshal.SizeOf<FastTextPrediction>()));
                var prediction = Marshal.PtrToStructure<FastTextPrediction>(elementPtr);
                var label = DecodeString(prediction.Label);

                result.Add(new FastTextPredictionResult(
                    label: label,
                    probability: prediction.Prob
                ));
            }

            FastTextDetectorWrapper.DestroyPredictions(predictionPtr);
            return result;
        }

        public int GetModelDimensions()
        {
            return FastTextDetectorWrapper.FastTextGetModelDimensions(_fastText);
        }

        public void Dispose()
        {
            try
            {
                _semaphore.Wait();

                if (_fastText != IntPtr.Zero)
                {
                    FastTextDetectorWrapper.DestroyFastText(_fastText);
                    _fastText = IntPtr.Zero;
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void CheckError(IntPtr errorPtr)
        {
            if (errorPtr != IntPtr.Zero)
            {
                ThrowNativeException(errorPtr);
            }
        }

        private void ThrowNativeException(IntPtr errorPtr)
        {
            var error = DecodeString(errorPtr);
            FastTextDetectorWrapper.DestroyString(errorPtr);
            throw new NativeLibraryException(error);
        }

        private string DecodeString(IntPtr ptr)
        {
            return Marshal.PtrToStringUTF8(ptr) ?? throw new NullReferenceException("Failed to decode non-nullable string");
        }
    }
}
