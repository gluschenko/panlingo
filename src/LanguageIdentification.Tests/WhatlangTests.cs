using System.Runtime.InteropServices;
using Panlingo.LanguageIdentification.Tests.Helpers;
using Panlingo.LanguageIdentification.Whatlang;

namespace Panlingo.LanguageIdentification.Tests;

public class WhatlangTests
{
    [Fact]
    public void WhatlangCheckPlatformSupport()
    {
        var isSupported = RuntimeInformation.OSArchitecture switch
        {
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => true,
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
            Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
            _ => false,
        };

        Assert.Equal(isSupported, WhatlangDetector.IsSupported());
    }

    [SkippableTheory]
    [InlineData(WhatlangLanguage.Ron, Constants.PHRASE_ENG_1, 0.0274)]
    [InlineData(WhatlangLanguage.Ukr, Constants.PHRASE_UKR_1, 0.9999)]
    [InlineData(WhatlangLanguage.Rus, Constants.PHRASE_RUS_1, 0.2308)]
    public void WhatlangSingleLanguage(WhatlangLanguage languageCode, string text, double score)
    {
        Skip.IfNot(WhatlangDetector.IsSupported());

        using var whatlang = new WhatlangDetector();

        var prediction = whatlang.PredictLanguage(text: text);

        if (prediction is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, prediction.Language);
        Assert.Equal(score, prediction.Confidence, Constants.EPSILON);
    }

    [SkippableTheory]
    [InlineData(WhatlangScript.Latn, Constants.PHRASE_ENG_1)]
    [InlineData(WhatlangScript.Cyrl, Constants.PHRASE_UKR_1)]
    [InlineData(WhatlangScript.Cyrl, Constants.PHRASE_RUS_1)]
    public void WhatlangSingleScript(WhatlangScript script, string text)
    {
        Skip.IfNot(WhatlangDetector.IsSupported());

        using var whatlang = new WhatlangDetector();

        var prediction = whatlang.PredictScript(text: text);
        Assert.Equal(script, prediction);
    }

    [SkippableTheory]
    [InlineData(WhatlangLanguage.Ukr, "ukr")]
    [InlineData(WhatlangLanguage.Uzb, "uzb")]
    [InlineData(WhatlangLanguage.Heb, "heb")]
    [InlineData(WhatlangLanguage.Srp, "srp")]
    public void WhatlangGetLanguageCode(WhatlangLanguage language, string code)
    {
        Skip.IfNot(WhatlangDetector.IsSupported());

        using var whatlang = new WhatlangDetector();

        var languageCode = whatlang.GetLanguageCode(language);
        Assert.Equal(code, languageCode);
    }

    [SkippableFact]
    public void WhatlangGetLanguages()
    {
        Skip.IfNot(WhatlangDetector.IsSupported());

        using var whatlang = new WhatlangDetector();

        var labels = whatlang.GetLanguages();
        Assert.Equal(69, labels.Count());
        Assert.Contains(WhatlangLanguage.Heb, labels);
        Assert.Contains(WhatlangLanguage.Ukr, labels);
        Assert.Contains(WhatlangLanguage.Eng, labels);
        Assert.Contains(WhatlangLanguage.Cmn, labels);
    }
}
