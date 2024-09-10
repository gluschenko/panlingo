using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Panlingo.LanguageIdentification.MediaPipe.Internal;

namespace Panlingo.LanguageIdentification.MediaPipe
{
    /// <summary>
    /// .NET wrapper for MediaPipe
    /// </summary>
    public class MediaPipeDetector : IDisposable
    {
        private IntPtr _mediaPipe;
        private readonly SemaphoreSlim _semaphore;

        [Obsolete]
        public MediaPipeDetector(int resultCount = -1, float scoreThreshold = 0.0f, string modelPath = "") : this(
            (modelPath != "" ? MediaPipeOptions.FromFile(modelPath) : MediaPipeOptions.FromDefault())
                .WithResultCount(resultCount)
                .WithScoreThreshold(scoreThreshold)
        ) { }

        public MediaPipeDetector(MediaPipeOptions options)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
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
            else if(options.ModelPath is not null)
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

                _mediaPipe = MediaPipeDetectorWrapper.CreateLanguageDetector(ref nativeOptions, out var errorMessage);

                CheckError(errorMessage);
            }
            finally
            {
                modelDataHandle?.Free();
            }

            _semaphore = new SemaphoreSlim(1, 1);
        }

        public IEnumerable<MediaPipePrediction> PredictLanguages(string text)
        {
            var nativeResult = new LanguageDetectorResult();

            MediaPipeDetectorWrapper.UseLanguageDetector(
                handle: _mediaPipe,
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

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            try
            {
                _semaphore.Wait();

                if (_mediaPipe != IntPtr.Zero)
                {
                    MediaPipeDetectorWrapper.FreeLanguageDetector(_mediaPipe, out var errorMessage);
                    _mediaPipe = IntPtr.Zero;
                    CheckError(errorMessage);
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
            throw new NativeLibraryException(error);
        }

        private string DecodeString(IntPtr ptr)
        {
            return Marshal.PtrToStringUTF8(ptr) ?? throw new NullReferenceException("Failed to decode non-nullable string");
        }
    }
}
