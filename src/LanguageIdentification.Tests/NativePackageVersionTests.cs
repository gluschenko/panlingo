using System.Reflection;
using Cld2 = Panlingo.LanguageIdentification.CLD2.CLD2Detector;
using Cld2Native = Panlingo.LanguageIdentification.CLD2.Native.CLD2NativeLibrary;
using Cld3 = Panlingo.LanguageIdentification.CLD3.CLD3Detector;
using Cld3Native = Panlingo.LanguageIdentification.CLD3.Native.CLD3NativeLibrary;
using FastText = Panlingo.LanguageIdentification.FastText.FastTextDetector;
using FastTextNative = Panlingo.LanguageIdentification.FastText.Native.FastTextNativeLibrary;
using Lingua = Panlingo.LanguageIdentification.Lingua.LinguaDetector;
using LinguaNative = Panlingo.LanguageIdentification.Lingua.Native.LinguaNativeLibrary;
using MediaPipe = Panlingo.LanguageIdentification.MediaPipe.MediaPipeDetector;
using MediaPipeNative = Panlingo.LanguageIdentification.MediaPipe.Native.MediaPipeNativeLibrary;
using Whatlang = Panlingo.LanguageIdentification.Whatlang.WhatlangDetector;
using WhatlangNative = Panlingo.LanguageIdentification.Whatlang.Native.WhatlangNativeLibrary;

namespace Panlingo.LanguageIdentification.Tests;

public class NativePackageVersionTests
{
    [Theory]
    [MemberData(nameof(PackagePairs))]
    public void NativePackageVersionMatchesManagedPackage(Assembly managedAssembly, Assembly nativeAssembly)
    {
        Assert.Equal(GetPackageVersion(managedAssembly), GetPackageVersion(nativeAssembly));
    }

    public static IEnumerable<object[]> PackagePairs()
    {
        yield return new object[] { typeof(Cld2).Assembly, typeof(Cld2Native).Assembly };
        yield return new object[] { typeof(Cld3).Assembly, typeof(Cld3Native).Assembly };
        yield return new object[] { typeof(FastText).Assembly, typeof(FastTextNative).Assembly };
        yield return new object[] { typeof(Lingua).Assembly, typeof(LinguaNative).Assembly };
        yield return new object[] { typeof(MediaPipe).Assembly, typeof(MediaPipeNative).Assembly };
        yield return new object[] { typeof(Whatlang).Assembly, typeof(WhatlangNative).Assembly };
    }

    private static string GetPackageVersion(Assembly assembly)
    {
        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? assembly.GetName().Version?.ToString()
            ?? string.Empty;

        var metadataIndex = version.IndexOf('+');
        return metadataIndex >= 0 ? version.Substring(0, metadataIndex) : version;
    }
}
