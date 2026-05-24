using System.Runtime.InteropServices;
using System.Text;
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
    public void LinguaMalformedBytes(string text)
    {
        Skip.IfNot(LinguaDetector.IsSupported());

        using var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>());
        using var lingua = linguaBuilder.Build();

        var predictions = lingua.PredictMixedLanguages(text: text);
    }

    [SkippableFact]
    public void LinguaRejectsNullText()
    {
        Skip.IfNot(LinguaDetector.IsSupported());

        using var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>());
        using var lingua = linguaBuilder.Build();

        Assert.Throws<ArgumentNullException>(() => lingua.PredictLanguages(null!));
        Assert.Throws<ArgumentNullException>(() => lingua.PredictMixedLanguages(null!));
    }

    [SkippableFact]
    public void LinguaRejectsInvalidEnums()
    {
        Skip.IfNot(LinguaDetector.IsSupported());

        Assert.Throws<LinguaDetectorException>(() => new LinguaDetectorBuilder(new[] { (LinguaLanguage)255 }));

        using var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>());
        using var lingua = linguaBuilder.Build();

        Assert.Throws<LinguaDetectorException>(() => lingua.GetLanguageCode((LinguaLanguage)255, LinguaLanguageCode.Alpha2));
        Assert.Throws<LinguaDetectorException>(() => lingua.GetLanguageCode(LinguaLanguage.English, (LinguaLanguageCode)255));
    }

    [SkippableFact]
    public void LinguaRejectsNegativeCount()
    {
        Skip.IfNot(LinguaDetector.IsSupported());

        using var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>());
        using var lingua = linguaBuilder.Build();

        Assert.Throws<ArgumentOutOfRangeException>(() => lingua.PredictLanguages(Constants.PHRASE_ENG_1, -1));
    }

    [SkippableFact]
    public void LinguaAcceptsEmbeddedNulText()
    {
        Skip.IfNot(LinguaDetector.IsSupported());

        using var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>());
        using var lingua = linguaBuilder.Build();

        var predictions = lingua.PredictMixedLanguages(Constants.MALFORMED_BYTES_1);

        Assert.NotNull(predictions);
    }

    [SkippableFact]
    public async Task LinguaPredictAndDisposeRaceDoesNotCrash()
    {
        Skip.IfNot(LinguaDetector.IsSupported());

        using var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>());
        var lingua = linguaBuilder.Build();
        var predict = Task.Run(() =>
        {
            try
            {
                _ = lingua.PredictLanguages(Constants.PHRASE_ENG_1).ToArray();
            }
            catch (ObjectDisposedException)
            {
            }
        });
        var dispose = Task.Run(lingua.Dispose);

        await Task.WhenAll(predict, dispose);
    }
}
