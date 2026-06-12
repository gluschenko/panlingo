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
            Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => true,
            Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => true,
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

    [SkippableFact]
    public void CLD2AcceptsNullText()
    {
        Skip.IfNot(CLD2Detector.IsSupported());

        using var cld2 = new CLD2Detector();

        var predictions = cld2.PredictLanguage(null!).ToArray();

        Assert.NotNull(predictions);
    }

    [SkippableFact]
    public void CLD2AcceptsEmbeddedNulText()
    {
        Skip.IfNot(CLD2Detector.IsSupported());

        using var cld2 = new CLD2Detector();

        var predictions = cld2.PredictLanguage(Constants.MALFORMED_BYTES_1);

        Assert.NotNull(predictions);
    }

    [SkippableTheory]
    [InlineData(Constants.PHRASE_ENG_1)]
    [InlineData(Constants.PHRASE_NOISY_1)]
    [InlineData(Constants.PHRASE_SHORT_1)]
    [InlineData(Constants.PHRASE_NUMERIC_1)]
    public void CLD2PredictionsStayInValidRange(string text)
    {
        Skip.IfNot(CLD2Detector.IsSupported());

        using var cld2 = new CLD2Detector();

        var predictions = cld2.PredictLanguage(text).ToArray();

        Assert.NotNull(predictions);
        Assert.All(predictions, prediction =>
        {
            Assert.False(string.IsNullOrWhiteSpace(prediction.Language));
            Assert.InRange(prediction.Probability, 0, 1);
        });
    }

    [SkippableFact]
    public void CLD2RepeatedCallsDoNotCorruptResults()
    {
        Skip.IfNot(CLD2Detector.IsSupported());

        using var cld2 = new CLD2Detector();

        for (var i = 0; i < 50; i++)
        {
            var predictions = cld2.PredictLanguage(Constants.PHRASE_ENG_1).ToArray();
            Assert.NotEmpty(predictions);
            Assert.All(predictions, prediction => Assert.InRange(prediction.Probability, 0, 1));
        }
    }

    [SkippableFact]
    public async Task CLD2ParallelPredictionsDoNotCrash()
    {
        Skip.IfNot(CLD2Detector.IsSupported());

        using var cld2 = new CLD2Detector();
        var tasks = Enumerable.Range(0, 32)
            .Select(_ => Task.Run(() => cld2.PredictLanguage(Constants.PHRASE_NOISY_1).ToArray()));

        var results = await Task.WhenAll(tasks);

        Assert.All(results, predictions => Assert.NotNull(predictions));
    }

    [SkippableFact]
    public void CLD2ThrowsAfterDispose()
    {
        Skip.IfNot(CLD2Detector.IsSupported());

        var cld2 = new CLD2Detector();
        cld2.Dispose();

        Assert.Throws<ObjectDisposedException>(() => cld2.PredictLanguage(Constants.PHRASE_ENG_1).ToArray());
        Assert.Throws<ObjectDisposedException>(() => cld2.GetLanguages().ToArray());
    }
}
