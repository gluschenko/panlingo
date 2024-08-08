using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
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

        public MediaPipeDetector()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new NotSupportedException(
                    $"{nameof(MediaPipeDetector)} is not yet supported on {RuntimeInformation.RuntimeIdentifier}"
                );
            }

            var path = Path.GetFullPath("language_detector.tflite");
            // var modelBytes = File.ReadAllBytes("language_detector.tflite");
            // var modelText = Encoding.ASCII.GetString(modelBytes);

            var options = new LanguageDetectorOptions(
                baseOptions: new BaseOptions(
                    modelAssetBuffer: null, 
                    modelAssetBufferCount: 0, 
                    modelAssetPath: path
                ), 
                classifierOptions: new ClassifierOptions(maxResults: 10)
            );

            try
            {
                var errorMessage = IntPtr.Zero;
                _mediaPipe = MediaPipeDetectorWrapper.CreateLangaugeDetector(options, ref errorMessage);
                _semaphore = new SemaphoreSlim(1, 1);

                CheckError(errorMessage);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                _semaphore.Wait();

                if (_mediaPipe != IntPtr.Zero)
                {
                    MediaPipeDetectorWrapper.FreeLangaugeDetector(_mediaPipe);
                    _mediaPipe = IntPtr.Zero;
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
