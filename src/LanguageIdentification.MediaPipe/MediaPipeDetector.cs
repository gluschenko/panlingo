using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Panlingo.LanguageIdentification.MediaPipe.Internal;
using Panlingo.LanguageIdentification.MediaPipe.Native;

namespace Panlingo.LanguageIdentification.MediaPipe
{
    /// <summary>
    /// <para>Example:</para>
    /// <code>
    /// using var mediaPipe = new MediaPipeDetector(
    ///     options: MediaPipeOptions.FromDefault()
    /// );
    /// 
    /// var predictions = mediaPipe.PredictLanguages("Привіт, як справи?");
    /// </code>
    /// 
    /// <para>The using-operator is required to correctly remove unmanaged resources from memory after use.</para>
    /// </summary>
    public class MediaPipeDetector : IDisposable
    {
        private readonly MediaPipeOptions _options;
        private readonly Lazy<ImmutableHashSet<string>> _labels;
        private readonly byte[]? _modelData;

        private IntPtr _detector;
        private bool _disposed = false;
        private int _activeOperations = 0;
        private readonly object _lifetimeLock = new object();

        private const string LABEL_FILE_NAME = "labels.txt";

        /// <summary>
        /// <para>Creates an instance for <see cref="MediaPipeDetector"/>.</para>
        /// <inheritdoc cref="MediaPipeDetector"/>
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
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

                _modelData = memoryStream.ToArray();
                var modelData = _modelData;
                modelDataHandle = GCHandle.Alloc(modelData, GCHandleType.Pinned);
                modelAssetBuffer = modelDataHandle.Value.AddrOfPinnedObject();
                modelAssetBufferCount = (uint)modelData.Length;
            }
            else if (options.ModelData is not null)
            {
                _modelData = options.ModelData;
                var modelData = _modelData;
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

            // The *.tflite is actually a zip archive. We need to read ‘labels.txt’ inside to get a list of labels.
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

                _detector = MediaPipeDetectorWrapper.CreateLanguageDetector(ref nativeOptions, IntPtr.Zero);
                if (_detector == IntPtr.Zero)
                {
                    throw new NativeLibraryException($"Failed to create {nameof(MediaPipeDetector)}");
                }
            }
            finally
            {
                modelDataHandle?.Free();
            }
        }

        /// <summary>
        /// Checks the suitability of the current platform for use. Key criteria are the operating system and processor architecture
        /// </summary>
        public static bool IsSupported()
        {
            return MediaPipeNativeLibrary.IsSupported();
        }

        public IEnumerable<MediaPipePrediction> PredictLanguages(string text)
        {
            CheckDisposed();
            var detector = AcquireDetector();

            var nativeResult = new LanguageDetectorResult();

            try
            {
                var status = MediaPipeDetectorWrapper.UseLanguageDetector(
                    handle: detector,
                    text: text,
                    result: ref nativeResult,
                    errorMessage: IntPtr.Zero
                );
                if (status != 0)
                {
                    throw new NativeLibraryException($"Failed to run {nameof(MediaPipeDetector)}");
                }

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
            finally
            {
                ReleaseDetector();
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
            if (_modelData is not null)
            {
                return _modelData;
            }

            if (_options.ModelPath is not null)
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

        private void CheckDisposed()
        {
            lock (_lifetimeLock)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(nameof(MediaPipeDetector), "This instance has already been disposed");
                }
            }
        }

        private IntPtr AcquireDetector()
        {
            lock (_lifetimeLock)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(nameof(MediaPipeDetector), "This instance has already been disposed");
                }

                _activeOperations++;
                return _detector;
            }
        }

        private void ReleaseDetector()
        {
            lock (_lifetimeLock)
            {
                _activeOperations--;
                if (_activeOperations == 0)
                {
                    Monitor.PulseAll(_lifetimeLock);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            IntPtr detector;

            lock (_lifetimeLock)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                while (_activeOperations > 0)
                {
                    Monitor.Wait(_lifetimeLock);
                }

                if (disposing)
                {
                    // Dispose managed resources if any
                }

                detector = _detector;
                _detector = IntPtr.Zero;
            }

            if (detector != IntPtr.Zero)
            {
                _ = MediaPipeDetectorWrapper.FreeLanguageDetector(detector, IntPtr.Zero);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MediaPipeDetector()
        {
            try
            {
                Dispose(false);
            }
            catch
            {
            }
        }
    }
}
