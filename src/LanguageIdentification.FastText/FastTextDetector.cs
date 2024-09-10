using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Panlingo.LanguageIdentification.FastText.Internal;

namespace Panlingo.LanguageIdentification.FastText
{
    /// <summary>
    /// .NET wrapper for FastText
    /// </summary>
    public class FastTextDetector : IDisposable
    {
        private IntPtr _fastText;
        private readonly SemaphoreSlim _semaphore;

        public FastTextDetector()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new NotSupportedException(
                    $"{nameof(FastTextDetector)} is not yet supported on {RuntimeInformation.RuntimeIdentifier}"
                );
            }

            _fastText = FastTextDetectorWrapper.CreateFastText();
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public string ModelPath { get; private set; } = string.Empty;

        public void LoadModel(string path)
        {
            _semaphore.Wait();
            try
            {
                var errPtr = IntPtr.Zero;
                FastTextDetectorWrapper.FastTextLoadModel(_fastText, path, ref errPtr);
                CheckError(errPtr);

                ModelPath = path;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void LoadModel(Stream stream)
        {
            _semaphore.Wait();
            try
            {
                using var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);

                var modelData = memoryStream.ToArray();
                var modelDataHandle = GCHandle.Alloc(modelData, GCHandleType.Pinned);
                var modelAssetBuffer = modelDataHandle.AddrOfPinnedObject();
                var modelAssetBufferCount = (uint)modelData.Length;

                try
                {
                    var errPtr = IntPtr.Zero;
                    FastTextDetectorWrapper.FastTextLoadModelData(_fastText, modelAssetBuffer, modelAssetBufferCount, ref errPtr);
                    CheckError(errPtr);
                }
                finally
                {
                    modelDataHandle.Free();
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void LoadDefaultModel()
        {
            using var memoryStream = new MemoryStream(FastTextResourceProvider.DefaultModel);
            LoadModel(memoryStream);
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

        /// <summary>
        /// Produces a prediction for 'text'
        /// </summary>
        /// <param name="text">Some text in natural language</param>
        /// <param name="count">Number of predictions</param>
        /// <param name="threshold">An internal accuracy threshold</param>
        /// <returns>List of language predictions</returns>
        public IEnumerable<FastTextPrediction> Predict(string text, int count, float threshold = 0.0f)
        {
            var errPtr = IntPtr.Zero;
            var predictionPtr = FastTextDetectorWrapper.FastTextPredict(
                handle: _fastText,
                text: text,
                k: count,
                threshold: threshold,
                ref errPtr
            );

            CheckError(errPtr);

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
            GC.SuppressFinalize(this);

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

        private static void CheckError(IntPtr errorPtr)
        {
            if (errorPtr != IntPtr.Zero)
            {
                ThrowNativeException(errorPtr);
            }
        }

        private static void ThrowNativeException(IntPtr errorPtr)
        {
            try
            {
                var error = DecodeString(errorPtr);
                throw new NativeLibraryException(error);
            }
            finally
            {
                FastTextDetectorWrapper.DestroyString(errorPtr);
            }
        }

        private static string DecodeString(IntPtr ptr)
        {
            return Marshal.PtrToStringUTF8(ptr) ?? throw new NullReferenceException("Failed to decode non-nullable string");
        }
    }
}
