using System.Runtime.InteropServices;
using Panlingo.LanguageIdentification.CLD3;
using Panlingo.LanguageIdentification.Tests.Helpers;

namespace Panlingo.LanguageIdentification.Tests;

public class CLD3Tests
{
    [Fact]
    public void CLD3CheckPlatformSupport()
    {
        var isSupported = RuntimeInformation.OSArchitecture switch
        {
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => true,
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
            Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
            _ => false,
        };

        Assert.Equal(isSupported, CLD3Detector.IsSupported());
    }

    [SkippableTheory]
    [InlineData("en", Constants.PHRASE_ENG_1, 0.9985)]
    [InlineData("uk", Constants.PHRASE_UKR_1, 0.9992)]
    [InlineData("ru", Constants.PHRASE_RUS_1, 0.9770)]
    public void CLD3SingleLanguage(string languageCode, string text, double score)
    {
        Skip.IfNot(CLD3Detector.IsSupported());

        using var cld3 = new CLD3Detector(0, 512);

        var prediction = cld3.PredictLanguage(text: text);
        var predictions = cld3.PredictLanguages(text: text, count: 3);

        if (prediction is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, prediction.Language);
        Assert.Equal(score, prediction.Probability, Constants.EPSILON);
    }

    [SkippableTheory]
    [InlineData("en", Constants.PHRASE_ENG_1, 0.9985)]
    [InlineData("uk", Constants.PHRASE_UKR_1, 0.9992)]
    [InlineData("ru", Constants.PHRASE_RUS_1, 0.9770)]
    public void CLD3MixedLanguage(string languageCode, string text, double score)
    {
        Skip.IfNot(CLD3Detector.IsSupported());

        using var cld3 = new CLD3Detector(0, 512);

        var predictions = cld3.PredictLanguages(text: text, count: 3);
        var mainLanguage = predictions.FirstOrDefault();

        if (mainLanguage is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, mainLanguage.Language);
        Assert.Equal(score, mainLanguage.Probability, Constants.EPSILON);
    }

    [SkippableFact]
    public void CLD3GetLanguages()
    {
        Skip.IfNot(CLD3Detector.IsSupported());

        using var cld3 = new CLD3Detector(0, 512);

        var labels = cld3.GetLanguages();
        Assert.Equal(107, labels.Count());
        Assert.Contains("uz", labels);
        Assert.Contains("uk", labels);
        Assert.Contains("en", labels);
        Assert.Contains("zh-Latn", labels);
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
    public void CLD3MalformedBytes(string text)
    {
        Skip.IfNot(CLD3Detector.IsSupported());

        using var cld3 = new CLD3Detector(0, 512);

        var predictions = cld3.PredictLanguages(text: text, count: 3);
    }
}
