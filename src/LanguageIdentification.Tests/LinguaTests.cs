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
    public void LinguaAcceptsNullText()
    {
        Skip.IfNot(LinguaDetector.IsSupported());

        using var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>());
        using var lingua = linguaBuilder.Build();

        var predictions = lingua.PredictLanguages(null!).ToArray();
        var ranges = lingua.PredictMixedLanguages(null!).ToArray();

        Assert.NotNull(predictions);
        Assert.NotNull(ranges);
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
    public void LinguaReturnsEmptyForNegativeCount()
    {
        Skip.IfNot(LinguaDetector.IsSupported());

        using var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>());
        using var lingua = linguaBuilder.Build();

        var predictions = lingua.PredictLanguages(Constants.PHRASE_ENG_1, -1);

        Assert.Empty(predictions);
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

    [SkippableFact]
    public void LinguaReturnsEmptyForZeroCount()
    {
        Skip.IfNot(LinguaDetector.IsSupported());

        using var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>());
        using var lingua = linguaBuilder.Build();

        var predictions = lingua.PredictLanguages(Constants.PHRASE_ENG_1, 0);

        Assert.Empty(predictions);
    }

    [SkippableFact]
    public void LinguaThrowsAfterDispose()
    {
        Skip.IfNot(LinguaDetector.IsSupported());

        using var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>());
        var lingua = linguaBuilder.Build();
        lingua.Dispose();

        Assert.Throws<ObjectDisposedException>(() => lingua.PredictLanguages(Constants.PHRASE_ENG_1).ToArray());
        Assert.Throws<ObjectDisposedException>(() => lingua.PredictMixedLanguages(Constants.PHRASE_ENG_1).ToArray());
        Assert.Throws<ObjectDisposedException>(() => lingua.GetLanguageCode(LinguaLanguage.English, LinguaLanguageCode.Alpha2));
        Assert.Throws<ObjectDisposedException>(() => lingua.GetLanguages().ToArray());
    }

    [SkippableFact]
    public void LinguaBuilderThrowsAfterDispose()
    {
        Skip.IfNot(LinguaDetector.IsSupported());

        var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>());
        linguaBuilder.Dispose();

        Assert.Throws<ObjectDisposedException>(() => linguaBuilder.Build());
        Assert.Throws<ObjectDisposedException>(() => linguaBuilder.WithLowAccuracyMode());
        Assert.Throws<ObjectDisposedException>(() => linguaBuilder.WithPreloadedLanguageModels());
        Assert.Throws<ObjectDisposedException>(() => linguaBuilder.WithMinimumRelativeDistance(0.5));
    }

    [SkippableFact]
    public async Task LinguaBuilderBuildAndDisposeRaceDoesNotCrash()
    {
        Skip.IfNot(LinguaDetector.IsSupported());

        var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>());
        var build = Task.Run(() =>
        {
            try
            {
                using var lingua = linguaBuilder.Build();
            }
            catch (ObjectDisposedException)
            {
            }
        });
        var dispose = Task.Run(linguaBuilder.Dispose);

        await Task.WhenAll(build, dispose);
    }

    [SkippableFact]
    public void LinguaBuilderModesBuildUsableDetectors()
    {
        Skip.IfNot(LinguaDetector.IsSupported());

        using var lowAccuracyBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>())
            .WithLowAccuracyMode();
        using var lowAccuracy = lowAccuracyBuilder.Build();

        using var preloadedBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>())
            .WithPreloadedLanguageModels();
        using var preloaded = preloadedBuilder.Build();

        Assert.NotEmpty(lowAccuracy.PredictLanguages(Constants.PHRASE_ENG_1));
        Assert.NotEmpty(preloaded.PredictLanguages(Constants.PHRASE_ENG_1));
    }

    [SkippableFact]
    public async Task LinguaParallelPredictionsDoNotCrash()
    {
        Skip.IfNot(LinguaDetector.IsSupported());

        using var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>());
        using var lingua = linguaBuilder.Build();
        var tasks = Enumerable.Range(0, 32)
            .Select(i => Task.Run(() => lingua.PredictLanguages(i % 2 == 0 ? Constants.PHRASE_NOISY_1 : Constants.PHRASE_MIXED_1).ToArray()));

        var results = await Task.WhenAll(tasks);

        Assert.All(results, predictions =>
        {
            Assert.NotNull(predictions);
            Assert.All(predictions, prediction => Assert.InRange(prediction.Confidence, 0, 1));
        });
    }
}
