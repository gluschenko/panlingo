using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
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
        private readonly MediaPipeOptions _options;
        private readonly Lazy<ImmutableHashSet<string>> _labels;

        private IntPtr _detector;
        private bool _disposed = false;

        private const string LABEL_FILE_NAME = "labels.txt";

        public MediaPipeDetector(MediaPipeOptions options)
        {
            if (!IsSupported())
            {
                throw new NotSupportedException(
                    $"{nameof(MediaPipeDetector)} is not yet supported on {RuntimeInformation.RuntimeIdentifier} ({RuntimeInformation.OSArchitecture})"
                );
            }

            _options = options;

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

            _labels = new Lazy<ImmutableHashSet<string>>(
                () =>
                {
                    var modelData = ReadModelData();

                    using var stream = new MemoryStream(modelData);
                    using var zip = new ZipArchive(stream, ZipArchiveMode.Read);

                    var labelFile = zip.Entries.FirstOrDefault(x =>
                    {
                        return x.Name.Equals(LABEL_FILE_NAME, StringComparison.InvariantCultureIgnoreCase);
                    });

                    if (labelFile is null)
                    {
                        throw new Exception($"File '{LABEL_FILE_NAME}' not found inside model file");
                    }

                    using var labelStream = labelFile.Open();
                    using var labelReader = new StreamReader(labelStream);

                    var result = ImmutableHashSet.CreateBuilder<string>();

                    while (true)
                    {
                        var label = labelReader.ReadLine();
                        if (string.IsNullOrEmpty(label))
                        {
                            break;
                        }

                        result.Add(label);
                    }

                    return result.ToImmutableHashSet<string>();
                }
            );

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

            _ = MediaPipeDetectorWrapper.UseLanguageDetector(
                handle: _detector,
                text: text,
                result: ref nativeResult,
                errorMessage: out var errorMessage
            );
            CheckError(errorMessage);

            try
            {
                var result = new LanguageDetectorPrediction[nativeResult.PredictionsCount];
                var structSize = Marshal.SizeOf<LanguageDetectorPrediction>();

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

        /// <summary>
        /// Returns all labels in current model
        /// </summary>
        /// <returns>Collection of label strings</returns>
        public IEnumerable<string> GetLabels()
        {
            return _labels.Value;
        }

        private byte[] ReadModelData()
        {
            if (_options.ModelStream is not null)
            {
                using var memoryStream = new MemoryStream();
                return memoryStream.ToArray();
            }
            else if (_options.ModelData is not null)
            {
                return _options.ModelData;
            }
            else if (_options.ModelPath is not null)
            {
                if (!File.Exists(_options.ModelPath))
                {
                    throw new FileNotFoundException("File is not found", _options.ModelPath);
                }

                return File.ReadAllBytes(_options.ModelPath);
            }
            else
            {
                throw new InvalidOperationException("Model data not specified");
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
            var error = DecodeString(errorPtr);
            throw new NativeLibraryException(error);
        }

        private static string DecodeString(IntPtr ptr)
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
                    _ = MediaPipeDetectorWrapper.FreeLanguageDetector(_detector, out var errorMessage);
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
