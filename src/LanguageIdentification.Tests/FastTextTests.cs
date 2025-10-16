using System.Runtime.InteropServices;
using Panlingo.LanguageIdentification.FastText;
using Panlingo.LanguageIdentification.Tests.Helpers;

namespace Panlingo.LanguageIdentification.Tests;

public class FastTextTests : IAsyncLifetime
{
    private readonly string _modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models/fasttext176.bin");

    [Fact]
    public void FastTextCheckPlatformSupport()
    {
        var isSupported = RuntimeInformation.OSArchitecture switch
        {
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => true,
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
            Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
            _ => false,
        };

        Assert.Equal(isSupported, FastTextDetector.IsSupported());
    }

    [SkippableTheory]
    [InlineData("__label__en", Constants.PHRASE_ENG_1, 0.9955)]
    [InlineData("__label__uk", Constants.PHRASE_UKR_1, 0.9900)]
    [InlineData("__label__ru", Constants.PHRASE_RUS_1, 0.9983)]
    public void FastTextFileSingleLanguage(string languageCode, string text, double score)
    {
        Skip.IfNot(FastTextDetector.IsSupported());

        using var fastText = new FastTextDetector();

        fastText.LoadModel(_modelPath);

        var predictions = fastText.Predict(text: text, count: 10);
        var mainLanguage = predictions.FirstOrDefault();

        if (mainLanguage is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, mainLanguage.Label);
        Assert.Equal(score, mainLanguage.Probability, Constants.EPSILON);
    }

    [SkippableTheory]
    [InlineData("__label__en", Constants.PHRASE_ENG_1, 0.9955)]
    [InlineData("__label__uk", Constants.PHRASE_UKR_1, 0.9900)]
    [InlineData("__label__ru", Constants.PHRASE_RUS_1, 0.9983)]
    public void FastTextStreamSingleLanguage(string languageCode, string text, double score)
    {
        Skip.IfNot(FastTextDetector.IsSupported());

        using var fastText = new FastTextDetector();

        using var stream = File.Open(_modelPath, FileMode.Open);

        fastText.LoadModel(stream);

        var predictions = fastText.Predict(text: text, count: 10);
        var mainLanguage = predictions.FirstOrDefault();

        if (mainLanguage is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, mainLanguage.Label);
        Assert.Equal(score, mainLanguage.Probability, Constants.EPSILON);
    }

    [SkippableTheory]
    [InlineData("__label__en", Constants.PHRASE_ENG_1, 1.0000)]
    [InlineData("__label__uk", Constants.PHRASE_UKR_1, 0.8511)]
    [InlineData("__label__ru", Constants.PHRASE_RUS_1, 0.9693)]
    public void FastTextContainedSingleLanguage(string languageCode, string text, double score)
    {
        Skip.IfNot(FastTextDetector.IsSupported());

        using var fastText = new FastTextDetector();
        fastText.LoadDefaultModel();

        var predictions = fastText.Predict(text: text, count: 10);
        var mainLanguage = predictions.FirstOrDefault();

        if (mainLanguage is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, mainLanguage.Label);
        Assert.Equal(score, mainLanguage.Probability, Constants.EPSILON);
    }

    [SkippableFact]
    public void FastTextLabels()
    {
        Skip.IfNot(FastTextDetector.IsSupported());

        using var fastText = new FastTextDetector();

        fastText.LoadModel(_modelPath);

        var labels = fastText.GetLabels();

        Assert.Equal(176, labels.Count());
        Assert.Contains(labels, x => x.Label == "__label__en");
        Assert.Contains(labels, x => x.Label == "__label__uk");
        Assert.Contains(labels, x => x.Label == "__label__ru");
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
    public void FastTextMalformedBytes(string text)
    {
        using var fastText = new FastTextDetector();
        fastText.LoadDefaultModel();

        var predictions = fastText.Predict(text: text, count: 10);
    }

    public async Task InitializeAsync()
    {
        var url = "https://dl.fbaipublicfiles.com/fasttext/supervised-models/lid.176.bin";
        await FileHelper.DownloadAsync(
            path: _modelPath,
            url: url
        );
    }

    public async Task DisposeAsync()
    {
        if (File.Exists(_modelPath))
        {
            File.Delete(_modelPath);
        }

        await Task.CompletedTask;
    }
}
