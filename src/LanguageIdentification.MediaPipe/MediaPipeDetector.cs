using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Panlingo.LanguageIdentification.MediaPipe.Internal;

namespace Panlingo.LanguageIdentification.MediaPipe
{
    /// <summary>
    /// .NET wrapper for MediaPipe
    /// </summary>
    public class MediaPipeDetector : IDisposable
    {
        [Obsolete]
        public MediaPipeDetector(int resultCount = -1, float scoreThreshold = 0.0f, string modelPath = "") : this(
            (modelPath != "" ? MediaPipeOptions.FromFile(modelPath) : MediaPipeOptions.FromDefault())
                .WithResultCount(resultCount)
                .WithScoreThreshold(scoreThreshold)
        )
        { }
        private IntPtr _detector;
        private bool _disposed = false;

        public MediaPipeDetector(MediaPipeOptions options)
        {
            if (!IsSupported())
            {
                throw new NotSupportedException(
                    $"{nameof(MediaPipeDetector)} is not yet supported on {RuntimeInformation.RuntimeIdentifier}"
                );
            }

            var modelAssetBuffer = IntPtr.Zero;
            uint modelAssetBufferCount = 0;
            GCHandle? modelDataHandle = null;

            if (options.ModelStream is not null)
            {
                using var memoryStream = new MemoryStream();
                options.ModelStream.CopyTo(memoryStream);

                var modelData = memoryStream.ToArray();
                modelDataHandle = GCHandle.Alloc(modelData, GCHandleType.Pinned);
                modelAssetBuffer = modelDataHandle.Value.AddrOfPinnedObject();
                modelAssetBufferCount = (uint)modelData.Length;
            }
            else if (options.ModelData is not null)
            {
                var modelData = options.ModelData;
                modelDataHandle = GCHandle.Alloc(modelData, GCHandleType.Pinned);
                modelAssetBuffer = modelDataHandle.Value.AddrOfPinnedObject();
                modelAssetBufferCount = (uint)modelData.Length;
            }
            else if (options.ModelPath is not null)
            {
                if (!File.Exists(options.ModelPath))
                {
                    throw new FileNotFoundException("File is not found", options.ModelPath);
                }
            }
            else
            {
                throw new InvalidOperationException("Model data not specified");
            }

            try
            {
                var nativeOptions = new LanguageDetectorOptions(
                    baseOptions: new BaseOptions(
                        modelAssetBuffer: modelAssetBuffer,
                        modelAssetBufferCount: modelAssetBufferCount,
                        modelAssetPath: options.ModelPath
                    ),
                    classifierOptions: new ClassifierOptions(
                        resultCount: options.ResultCount,
                        scoreThreshold: options.ScoreThreshold
                    )
                );

                _detector = MediaPipeDetectorWrapper.CreateLanguageDetector(ref nativeOptions, out var errorMessage);

                CheckError(errorMessage);
            }
            finally
            {
                modelDataHandle?.Free();
            }
        }

        public static bool IsSupported()
        {
            return RuntimeInformation.OSArchitecture switch
            {
                Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
                _ => false,
            };
        }

        public IEnumerable<MediaPipePrediction> PredictLanguages(string text)
        {
            CheckDisposed();

            var nativeResult = new LanguageDetectorResult();

            MediaPipeDetectorWrapper.UseLanguageDetector(
                handle: _detector,
                text: text,
                result: ref nativeResult,
                errorMessage: out var errorMessage
            );
            CheckError(errorMessage);

            try
            {
                var result = new LanguageDetectorPrediction[nativeResult.PredictionsCount];
                var structSize = Marshal.SizeOf(typeof(LanguageDetectorPrediction));

                for (var i = 0; i < nativeResult.PredictionsCount; i++)
                {
                    result[i] = Marshal.PtrToStructure<LanguageDetectorPrediction>(nativeResult.Predictions + i * structSize);
                }

                return result
                    .OrderByDescending(x => x.Probability)
                    .Select(x => new MediaPipePrediction(x))
                    .ToArray();
            }
            finally
            {
                MediaPipeDetectorWrapper.FreeLanguageDetectorResult(ref nativeResult);
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
            throw new NativeLibraryException(error);
        }

        private string DecodeString(IntPtr ptr)
        {
            return Marshal.PtrToStringUTF8(ptr) ?? throw new NullReferenceException("Failed to decode non-nullable string");
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(MediaPipeDetector), "This instance has already been disposed");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources if any
                }

                if (_detector != IntPtr.Zero)
                {
                    MediaPipeDetectorWrapper.FreeLanguageDetector(_detector, out var errorMessage);
                    _detector = IntPtr.Zero;
                    CheckError(errorMessage);
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MediaPipeDetector()
        {
            Dispose(false);
        }
    }
}
