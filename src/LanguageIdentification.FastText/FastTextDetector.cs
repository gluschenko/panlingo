using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<FastTextLabel> GetLabels()
        {
            var labelsPtr = FastTextDetectorWrapper.FastTextGetLabels(_fastText);

            if (labelsPtr == IntPtr.Zero)
            {
                return Array.Empty<FastTextLabel>();
            }

            var labelsStruct = Marshal.PtrToStructure<FastTextLabels>(labelsPtr);
            var result = new List<FastTextLabel>();

            for (ulong i = 0; i < labelsStruct.Length; i++)
            {
                var labelPtr = Marshal.ReadIntPtr(labelsStruct.Labels, (int)i * IntPtr.Size);
                var label = DecodeString(labelPtr);
                var freq = Marshal.ReadInt64(labelsStruct.Freqs, (int)i * sizeof(long));
                result.Add(new FastTextLabel(label: label, frequency: freq));
            }

            FastTextDetectorWrapper.DestroyLabels(labelsPtr);

            return result;
        }

        public IEnumerable<FastTextPrediction> Predict(string text, int k, float threshold = 0.0f)
        {
            var errptr = IntPtr.Zero;
            var predictionPtr = FastTextDetectorWrapper.FastTextPredict(
                handle: _fastText, 
                text: text,
                k: k,
                threshold: threshold,
                ref errptr
            );

            CheckError(errptr);

            if (predictionPtr == IntPtr.Zero)
            {
                return Array.Empty<FastTextPrediction>();
            }

            var predictions = Marshal.PtrToStructure<FastTextPredictionListNativeResult>(predictionPtr);
            var result = new List<FastTextPrediction>();

            for (ulong i = 0; i < predictions.Length; i++)
            {
                IntPtr elementPtr = new IntPtr(predictions.Predictions.ToInt64() + (long)(i * (uint)Marshal.SizeOf<FastTextPredictionNativeResult>()));
                var prediction = Marshal.PtrToStructure<FastTextPredictionNativeResult>(elementPtr);
                var label = DecodeString(prediction.Label);

                result.Add(new FastTextPrediction(
                    label: label,
                    probability: prediction.Prob
                ));
            }

            FastTextDetectorWrapper.DestroyPredictions(predictionPtr);

            return result
                .OrderByDescending(x => x.Probability)
                .ToArray();
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
