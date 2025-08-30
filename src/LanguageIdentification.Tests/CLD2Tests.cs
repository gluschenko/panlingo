using System.Runtime.InteropServices;
using Panlingo.LanguageIdentification.CLD2;
using Panlingo.LanguageIdentification.Tests.Helpers;

namespace Panlingo.LanguageIdentification.Tests;

public class CLD2Tests
{
    [Fact]
    public void CLD2CheckPlatformSupport()
    {
        var isSupported = RuntimeInformation.OSArchitecture switch
        {
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => true,
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
            Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
            _ => false,
        };

        Assert.Equal(isSupported, CLD2Detector.IsSupported());
    }

    [SkippableTheory]
    [InlineData("en", Constants.PHRASE_ENG_1, 0.9999)]
    [InlineData("uk", Constants.PHRASE_UKR_1, 0.9999)]
    [InlineData("un", Constants.PHRASE_RUS_1, 0)]
    public void CLD2SingleLanguage(string languageCode, string text, double score)
    {
        Skip.IfNot(CLD2Detector.IsSupported());

        using var cld2 = new CLD2Detector();

        var predictions = cld2.PredictLanguage(text);
        var mainLanguage = predictions.FirstOrDefault();

        if (mainLanguage is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, mainLanguage.Language);
        Assert.Equal(score, mainLanguage.Probability, Constants.EPSILON);
    }

    [SkippableFact]
    public void CLD2GetLanguages()
    {
        Skip.IfNot(CLD2Detector.IsSupported());

        using var cld2 = new CLD2Detector();

        var labels = cld2.GetLanguages();
        Assert.Equal(83, labels.Count());
        Assert.Contains("iw", labels);
        Assert.Contains("uk", labels);
        Assert.Contains("en", labels);
        Assert.Contains("zh-Hant", labels);
    }

    [SkippableTheory]
    [InlineData(Constants.MALFORMED_BYTES_0)]
    [InlineData(Constants.MALFORMED_BYTES_1)]
    [InlineData(Constants.MALFORMED_BYTES_2)]
    [InlineData(Constants.MALFORMED_BYTES_3)]
    [InlineData(Constants.MALFORMED_BYTES_4)]
    [InlineData(Constants.MALFORMED_BYTES_5)]
    [InlineData(Constants.MALFORMED_BYTES_6)]
    [InlineData(Constants.MALFORMED_BYTES_7)]
    [InlineData(Constants.MALFORMED_BYTES_8)]
    [InlineData(Constants.MALFORMED_BYTES_9)]
    [InlineData(Constants.MALFORMED_BYTES_10)]
    [InlineData(Constants.MALFORMED_BYTES_11)]
    [InlineData(Constants.MALFORMED_BYTES_12)]
    public void CLD2MalformedBytes(string text)
    {
        Skip.IfNot(CLD2Detector.IsSupported());

        using var cld2 = new CLD2Detector();

        var predictions = cld2.PredictLanguage(text);
    }
}
