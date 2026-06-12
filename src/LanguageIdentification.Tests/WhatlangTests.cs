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
            Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => true,
            Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => true,
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
    public void WhatlangMalformedBytes(string text)
    {
        Skip.IfNot(WhatlangDetector.IsSupported());

        using var whatlang = new WhatlangDetector();

        var prediction = whatlang.PredictScript(text: text);
    }

    [SkippableFact]
    public void WhatlangAcceptsNullText()
    {
        Skip.IfNot(WhatlangDetector.IsSupported());

        using var whatlang = new WhatlangDetector();

        var language = whatlang.PredictLanguage(null!);
        var script = whatlang.PredictScript(null!);

        Assert.True(language is null || Enum.IsDefined(typeof(WhatlangLanguage), language.Language));
        Assert.True(script is null || Enum.IsDefined(typeof(WhatlangScript), script.Value));
    }

    [SkippableFact]
    public void WhatlangRejectsInvalidEnums()
    {
        Skip.IfNot(WhatlangDetector.IsSupported());

        using var whatlang = new WhatlangDetector();

        Assert.Throws<WhatlangDetectorException>(() => whatlang.GetLanguageCode((WhatlangLanguage)255));
        Assert.Throws<WhatlangDetectorException>(() => whatlang.GetLanguageName((WhatlangLanguage)255));
        Assert.Throws<WhatlangDetectorException>(() => whatlang.GetLanguageEnglishName((WhatlangLanguage)255));
        Assert.Throws<WhatlangDetectorException>(() => whatlang.GetScriptName((WhatlangScript)255));
    }

    [SkippableFact]
    public void WhatlangAcceptsEmbeddedNulText()
    {
        Skip.IfNot(WhatlangDetector.IsSupported());

        using var whatlang = new WhatlangDetector();

        var prediction = whatlang.PredictScript(Constants.MALFORMED_BYTES_1);

        Assert.True(prediction is null || Enum.IsDefined(typeof(WhatlangScript), prediction.Value));
    }

    [SkippableTheory]
    [InlineData(Constants.PHRASE_NOISY_1)]
    [InlineData(Constants.PHRASE_SHORT_1)]
    [InlineData(Constants.PHRASE_NUMERIC_1)]
    public void WhatlangHandlesLowSignalText(string text)
    {
        Skip.IfNot(WhatlangDetector.IsSupported());

        using var whatlang = new WhatlangDetector();

        var language = whatlang.PredictLanguage(text);
        var script = whatlang.PredictScript(text);

        Assert.True(language is null || Enum.IsDefined(typeof(WhatlangLanguage), language.Language));
        Assert.True(language is null || Enum.IsDefined(typeof(WhatlangScript), language.Script));
        Assert.True(language is null || language.Confidence is >= 0 and <= 1);
        Assert.True(script is null || Enum.IsDefined(typeof(WhatlangScript), script.Value));
    }

    [SkippableFact]
    public void WhatlangRepeatedCallsDoNotCorruptResults()
    {
        Skip.IfNot(WhatlangDetector.IsSupported());

        using var whatlang = new WhatlangDetector();

        for (var i = 0; i < 100; i++)
        {
            var prediction = whatlang.PredictLanguage(Constants.PHRASE_UKR_1);
            Assert.NotNull(prediction);
            Assert.InRange(prediction.Confidence, 0, 1);
        }
    }

    [SkippableFact]
    public async Task WhatlangParallelPredictionsDoNotCrash()
    {
        Skip.IfNot(WhatlangDetector.IsSupported());

        using var whatlang = new WhatlangDetector();
        var tasks = Enumerable.Range(0, 64)
            .Select(i => Task.Run(() => (whatlang.PredictLanguage(i % 2 == 0 ? Constants.PHRASE_NOISY_1 : Constants.PHRASE_MIXED_1), whatlang.PredictScript(Constants.PHRASE_ENG_1))));

        var results = await Task.WhenAll(tasks);

        Assert.All(results, result =>
        {
            Assert.True(result.Item1 is null || Enum.IsDefined(typeof(WhatlangLanguage), result.Item1.Language));
            Assert.True(result.Item2 is null || Enum.IsDefined(typeof(WhatlangScript), result.Item2.Value));
        });
    }

    [SkippableFact]
    public void WhatlangThrowsAfterDispose()
    {
        Skip.IfNot(WhatlangDetector.IsSupported());

        var whatlang = new WhatlangDetector();
        whatlang.Dispose();

        Assert.Throws<ObjectDisposedException>(() => whatlang.PredictLanguage(Constants.PHRASE_ENG_1));
        Assert.Throws<ObjectDisposedException>(() => whatlang.PredictScript(Constants.PHRASE_ENG_1));
        Assert.Throws<ObjectDisposedException>(() => whatlang.GetLanguageCode(WhatlangLanguage.Eng));
        Assert.Throws<ObjectDisposedException>(() => whatlang.GetLanguageName(WhatlangLanguage.Eng));
        Assert.Throws<ObjectDisposedException>(() => whatlang.GetLanguageEnglishName(WhatlangLanguage.Eng));
        Assert.Throws<ObjectDisposedException>(() => whatlang.GetScriptName(WhatlangScript.Latn));
        Assert.Throws<ObjectDisposedException>(() => whatlang.GetLanguages().ToArray());
    }
}
