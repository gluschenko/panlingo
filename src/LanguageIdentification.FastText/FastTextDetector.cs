using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Panlingo.LanguageIdentification.FastText.Internal;
using Panlingo.LanguageIdentification.FastText.Native;

namespace Panlingo.LanguageIdentification.FastText
{
    /// <summary>
    /// <para>Example:</para>
    /// <code>
    /// using var fastText = new FastTextDetector();
    /// fastText.LoadDefaultModel();
    /// 
    /// var predictions = fastText.Predict(
    ///     text: "Привіт, як справи?", 
    ///     count: 10
    /// );
    /// </code>
    /// 
    /// <para>The using-operator is required to correctly remove unmanaged resources from memory after use.</para>
    /// </summary>
    public class FastTextDetector : IDisposable
    {
        private IntPtr _detector;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private volatile bool _disposed = false;

        /// <summary>
        /// <para>Creates an instance for <see cref="FastTextDetector"/>.</para>
        /// <inheritdoc cref="FastTextDetector"/>
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        public FastTextDetector()
        {
            if (!IsSupported())
            {
                throw new NotSupportedException(
                    $"{nameof(FastTextDetector)} is not yet supported on {RuntimeInformation.RuntimeIdentifier} ({RuntimeInformation.OSArchitecture})"
                );
            }

            _detector = FastTextDetectorWrapper.CreateFastText();
            if (_detector == IntPtr.Zero)
            {
                throw new NativeLibraryException($"Failed to create {nameof(FastTextDetector)}");
            }
        }

        /// <summary>
        /// Checks the suitability of the current platform for use. Key criteria are the operating system and processor architecture
        /// </summary>
        public static bool IsSupported()
        {
            return FastTextNativeLibrary.IsSupported();
        }

        public string ModelPath { get; private set; } = string.Empty;

        /// <summary>
        /// Loads model file located on path. Supports *.bin or *.ftz file formats.
        /// </summary>
        /// <param name="path">Path to *.bin or *.ftz model file</param>
        public void LoadModel(string path)
        {
            CheckDisposed();
            ArgumentNullException.ThrowIfNull(path);

            _semaphore.Wait();
            try
            {
                var errPtr = IntPtr.Zero;
                FastTextDetectorWrapper.FastTextLoadModel(_detector, path, ref errPtr);
                CheckError(errPtr);

                ModelPath = path;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Loads model file from binary stream. Supports *.bin or *.ftz file formats.
        /// </summary>
        /// <param name="stream">Stream of *.bin or *.ftz model file</param>
        public void LoadModel(Stream stream)
        {
            CheckDisposed();

            _semaphore.Wait();
            try
            {
                using var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);

                var modelData = memoryStream.ToArray();
                var modelDataHandle = GCHandle.Alloc(modelData, GCHandleType.Pinned);
                var modelAssetBuffer = modelDataHandle.AddrOfPinnedObject();
                var modelAssetBufferCount = (UIntPtr)modelData.Length;

                try
                {
                    var errPtr = IntPtr.Zero;
                    FastTextDetectorWrapper.FastTextLoadModelData(_detector, modelAssetBuffer, modelAssetBufferCount, ref errPtr);
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

        /// <summary>
        /// <para>Loads self-contained model located in the package.</para>
        /// <para>Original file: https://dl.fbaipublicfiles.com/fasttext/supervised-models/lid.176.ftz</para>
        /// </summary>
        public void LoadDefaultModel()
        {
            CheckDisposed();

            using var memoryStream = new MemoryStream(FastTextResourceProvider.DefaultModel);
            LoadModel(memoryStream);
        }

        /// <summary>
        /// Returns all labels in current model
        /// </summary>
        /// <returns>Collection of label objects</returns>
        public IEnumerable<FastTextLabel> GetLabels()
        {
            CheckDisposed();

            _semaphore.Wait();
            try
            {
                CheckDisposed();
                var labelsPtr = FastTextDetectorWrapper.FastTextGetLabels(_detector);

                if (labelsPtr == IntPtr.Zero)
                {
                    return Array.Empty<FastTextLabel>();
                }

                try
                {
                    var labelsStruct = Marshal.PtrToStructure<FastTextLabels>(labelsPtr);
                    var result = new List<FastTextLabel>();

                    for (ulong i = 0; i < labelsStruct.Length; i++)
                    {
                        var labelPtr = Marshal.ReadIntPtr(labelsStruct.Labels, (int)i * IntPtr.Size);
                        var label = DecodeString(labelPtr);
                        var freq = Marshal.ReadInt64(labelsStruct.Freqs, (int)i * sizeof(long));
                        result.Add(new FastTextLabel(label: label, frequency: freq));
                    }

                    return result;
                }
                finally
                {
                    FastTextDetectorWrapper.DestroyLabels(labelsPtr);
                }
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
        /// <param name="count">Number of predictions</param>
        /// <param name="threshold">An internal accuracy threshold</param>
        /// <returns>List of language predictions</returns>
        public IEnumerable<FastTextPrediction> Predict(string text, int count, float threshold = 0.0f)
        {
            CheckDisposed();
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "Count must be greater than or equal to zero");
            }

            var textBytes = EncodeText(text);
            _semaphore.Wait();
            try
            {
                CheckDisposed();
                var errPtr = IntPtr.Zero;
                var predictionPtr = FastTextDetectorWrapper.FastTextPredict(
                    handle: _detector,
                    text: textBytes,
                    textLength: (UIntPtr)textBytes.Length,
                    k: count,
                    threshold: threshold,
                    ref errPtr
                );

                CheckError(errPtr);

                if (predictionPtr == IntPtr.Zero)
                {
                    return Array.Empty<FastTextPrediction>();
                }

                try
                {
                    var predictions = Marshal.PtrToStructure<FastTextPredictionListNativeResult>(predictionPtr);
                    var result = new List<FastTextPrediction>();

                    var structSize = Marshal.SizeOf<FastTextPredictionNativeResult>();

                    for (var i = 0; i < (int)predictions.Length; i++)
                    {
                        var prediction = Marshal.PtrToStructure<FastTextPredictionNativeResult>(predictions.Predictions + i * structSize);
                        var label = DecodeString(prediction.Label);

                        result.Add(new FastTextPrediction(
                            label: label,
                            probability: prediction.Prob
                        ));
                    }

                    return result
                        .OrderByDescending(x => x.Probability)
                        .ToArray();
                }
                finally
                {
                    FastTextDetectorWrapper.DestroyPredictions(predictionPtr);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public int GetModelDimensions()
        {
            CheckDisposed();

            _semaphore.Wait();
            try
            {
                CheckDisposed();
                return FastTextDetectorWrapper.FastTextGetModelDimensions(_detector);
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

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(FastTextDetector), "This instance has already been disposed");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _semaphore.Wait();
            try
            {
                if (_disposed)
                {
                    return;
                }

                if (disposing)
                {
                    // Dispose managed resources if any
                }

                if (_detector != IntPtr.Zero)
                {
                    FastTextDetectorWrapper.DestroyFastText(_detector);
                    _detector = IntPtr.Zero;
                }

                _disposed = true;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FastTextDetector()
        {
            try
            {
                Dispose(false);
            }
            catch
            {
            }
        }

        private static byte[] EncodeText(string text)
        {
            ArgumentNullException.ThrowIfNull(text);
            return Encoding.UTF8.GetBytes(text);
        }
    }
}
