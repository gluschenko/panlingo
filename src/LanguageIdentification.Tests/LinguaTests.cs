﻿using System.Runtime.InteropServices;
using Panlingo.LanguageIdentification.Lingua;
using Panlingo.LanguageIdentification.Tests.Helpers;

namespace Panlingo.LanguageIdentification.Tests;

public class LinguaTests
{
    [Fact]
    public void LinguaCheckPlatformSupport()
    {
        var isSupported = RuntimeInformation.OSArchitecture switch
        {
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => true,
            Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
            _ => false,
        };

        Assert.Equal(isSupported, LinguaDetector.IsSupported());
    }

    [SkippableTheory]
    [InlineData(LinguaLanguage.English, Constants.PHRASE_ENG_1, 0.1666)]
    [InlineData(LinguaLanguage.Ukrainian, Constants.PHRASE_UKR_1, 0.8228)]
    [InlineData(LinguaLanguage.Russian, Constants.PHRASE_RUS_1, 0.3502)]
    public void LinguaSingleLanguage(LinguaLanguage languageCode, string text, double score)
    {
        Skip.IfNot(LinguaDetector.IsSupported());

        using var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>());
        using var lingua = linguaBuilder.Build();

        var predictions = lingua.PredictLanguages(text: text);
        var mainLanguage = predictions.FirstOrDefault();

        if (mainLanguage is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, mainLanguage.Language);
        Assert.Equal(score, mainLanguage.Confidence, Constants.EPSILON);
    }

    [SkippableTheory]
    [InlineData(LinguaLanguage.English, Constants.PHRASE_ENG_1, 0.1666)]
    [InlineData(LinguaLanguage.Ukrainian, Constants.PHRASE_UKR_1, 0.8228)]
    [InlineData(LinguaLanguage.Russian, Constants.PHRASE_RUS_1, 0.3502)]
    public void LinguaMixedLanguage(LinguaLanguage languageCode, string text, double score)
    {
        Skip.IfNot(LinguaDetector.IsSupported());

        using var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>());
        using var lingua = linguaBuilder.Build();

        var predictions = lingua.PredictMixedLanguages(text: text);
        var mainLanguage = predictions.FirstOrDefault();

        if (mainLanguage is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, mainLanguage.Language);
        Assert.Equal(score, mainLanguage.Confidence, Constants.EPSILON);
    }

    [SkippableTheory]
    [InlineData(LinguaLanguage.Ukrainian, LinguaLanguageCode.Alpha2, "uk")]
    [InlineData(LinguaLanguage.Ukrainian, LinguaLanguageCode.Alpha3, "ukr")]
    [InlineData(LinguaLanguage.Hebrew, LinguaLanguageCode.Alpha2, "he")]
    [InlineData(LinguaLanguage.Hebrew, LinguaLanguageCode.Alpha3, "heb")]
    [InlineData(LinguaLanguage.Serbian, LinguaLanguageCode.Alpha2, "sr")]
    [InlineData(LinguaLanguage.Serbian, LinguaLanguageCode.Alpha3, "srp")]
    public void LinguaGetLanguageCode(LinguaLanguage language, LinguaLanguageCode type, string code)
    {
        Skip.IfNot(LinguaDetector.IsSupported());

        using var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>());
        using var lingua = linguaBuilder.Build();

        var languageCode = lingua.GetLanguageCode(language, type);
        Assert.Equal(code, languageCode);
    }

    [SkippableFact]
    public void LinguaBuilderReuse()
    {
        Skip.IfNot(LinguaDetector.IsSupported());

        using var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>());
        using var lingua1 = linguaBuilder.Build();
        using var lingua2 = linguaBuilder.Build();

        Assert.NotEqual(lingua1, lingua2);
    }

    [SkippableFact]
    public void LinguaGetLanguages()
    {
        Skip.IfNot(LinguaDetector.IsSupported());

        using var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>());
        using var lingua = linguaBuilder.Build();

        var labels = lingua.GetLanguages();
        Assert.Equal(75, labels.Count());
        Assert.Contains(LinguaLanguage.Hebrew, labels);
        Assert.Contains(LinguaLanguage.Ukrainian, labels);
        Assert.Contains(LinguaLanguage.English, labels);
        Assert.Contains(LinguaLanguage.Chinese, labels);
    }
}
