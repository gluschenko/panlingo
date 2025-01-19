using System;
using System.Runtime.InteropServices;
using Panlingo.LanguageIdentification.Lingua.Internal;

namespace Panlingo.LanguageIdentification.Lingua
{
    /// <summary>
    /// .NET wrapper for Lingua
    /// </summary>
    public class LinguaDetectorBuilder : IDisposable
    {
        private readonly LinguaLanguage[] _languages;
        private IntPtr _builder;
        private bool _disposed = false;

        public LinguaDetectorBuilder(LinguaLanguage[] languages)
        {
            if (!LinguaDetector.IsSupported())
            {
                throw new NotSupportedException(
                    $"{nameof(LinguaDetector)} is not yet supported on {RuntimeInformation.RuntimeIdentifier} ({RuntimeInformation.OSArchitecture})"
                );
            }

            _builder = LinguaDetectorWrapper.LinguaLanguageDetectorBuilderCreate(languages, (UIntPtr)languages.Length);
            if (_builder == IntPtr.Zero)
            {
                throw new LinguaDetectorException($"Failed to create {nameof(LinguaDetectorBuilder)}");
            }

            _languages = languages;
        }

        internal IntPtr GetNativePointer()
        {
            return _builder;
        }

        public static LinguaDetectorBuilder FromLanguages(LinguaLanguage[] languages)
        {
            return new LinguaDetectorBuilder(languages);
        }

        public LinguaDetector Build()
        {
            CheckDisposed();

            return new LinguaDetector(this);
        }

        public LinguaDetectorBuilder WithLowAccuracyMode()
        {
            CheckDisposed();

            _builder = LinguaDetectorWrapper.LinguaLanguageDetectorBuilderWithLowAccuracyMode(_builder);
            return this;
        }

        public LinguaDetectorBuilder WithPreloadedLanguageModels()
        {
            CheckDisposed();

            _builder = LinguaDetectorWrapper.LinguaLanguageDetectorBuilderWithPreloadedLanguageModels(_builder);
            return this;
        }

        public LinguaDetectorBuilder WithMinimumRelativeDistance(double distance)
        {
            CheckDisposed();

            if (distance < 0.0 || distance > 0.99)
            {
                throw new ArgumentOutOfRangeException(nameof(distance), distance, "[0.00, 0.99]");
            }

            _builder = LinguaDetectorWrapper.LinguaLanguageDetectorBuilderWithMinimumRelativeDistance(_builder, distance);
            return this;
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(LinguaDetectorBuilder), "This instance has already been disposed");
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

                if (_builder != IntPtr.Zero)
                {
                    LinguaDetectorWrapper.LinguaLanguageDetectorBuilderDestroy(_builder);
                    _builder = IntPtr.Zero;
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~LinguaDetectorBuilder()
        {
            Dispose(false);
        }
    }

}
