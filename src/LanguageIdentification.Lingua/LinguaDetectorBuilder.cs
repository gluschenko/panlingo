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

        public LinguaDetectorBuilder(LinguaLanguage[] languages)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new NotSupportedException(
                    $"{nameof(LinguaDetector)} is not yet supported on {RuntimeInformation.RuntimeIdentifier}"
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

        public static LinguaDetectorBuilder FromLangauges(LinguaLanguage[] languages)
        {
            return new LinguaDetectorBuilder(languages);
        }

        public LinguaDetector Build()
        {
            return new LinguaDetector(this);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (_builder != IntPtr.Zero)
            {
                LinguaDetectorWrapper.LinguaLanguageDetectorBuilderDestroy(_builder);
                _builder = IntPtr.Zero;
            }
        }
    }

}
