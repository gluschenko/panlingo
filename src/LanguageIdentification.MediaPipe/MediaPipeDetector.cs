using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Panlingo.LanguageIdentification.MediaPipe.Internal;
using Panlingo.LanguageIdentification.MediaPipe.Native;

namespace Panlingo.LanguageIdentification.MediaPipe
{
    /// <summary>
    /// .NET wrapper for MediaPipe
    /// </summary>
    public class MediaPipeDetector : IDisposable
    {
        private IntPtr _mediaPipe;
        private readonly SemaphoreSlim _semaphore;

        public MediaPipeDetector(int resultCount = -1, float scoreThreshold = 0.0f, string modelPath = "")
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new NotSupportedException(
                    $"{nameof(MediaPipeDetector)} is not yet supported on {RuntimeInformation.RuntimeIdentifier}"
                );
            }

            if (string.IsNullOrWhiteSpace(modelPath))
            {
                modelPath = Path.GetFullPath(MediaPipeNativeLibrary.ModelName);

                foreach (var file in Directory.EnumerateFiles(".", "*", SearchOption.AllDirectories))
                {
                    if (Path.GetFileName(file) == MediaPipeNativeLibrary.ModelName)
                    {
                        modelPath = Path.GetFullPath(file);
                    }
                }
            }

            var options = new LanguageDetectorOptions(
                baseOptions: new BaseOptions(
                    modelAssetBuffer: null,
                    modelAssetBufferCount: 0,
                    modelAssetPath: modelPath
                ),
                classifierOptions: new ClassifierOptions(
                    resultCount: resultCount,
                    scoreThreshold: scoreThreshold
                )
            );

            _mediaPipe = MediaPipeDetectorWrapper.CreateLanguageDetector(ref options, out var errorMessage);
            _semaphore = new SemaphoreSlim(1, 1);

            CheckError(errorMessage);
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
