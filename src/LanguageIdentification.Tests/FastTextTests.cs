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
            Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => true,
            Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => true,
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
        Skip.IfNot(FastTextDetector.IsSupported());

        using var fastText = new FastTextDetector();
        fastText.LoadDefaultModel();

        var predictions = fastText.Predict(text: text, count: 10);
    }

    [SkippableFact]
    public void FastTextRejectsNullText()
    {
        Skip.IfNot(FastTextDetector.IsSupported());

        using var fastText = new FastTextDetector();
        fastText.LoadDefaultModel();

        Assert.Throws<ArgumentNullException>(() => fastText.Predict(null!, 10));
    }

    [SkippableFact]
    public void FastTextRejectsNullModelStream()
    {
        Skip.IfNot(FastTextDetector.IsSupported());

        using var fastText = new FastTextDetector();

        Assert.Throws<ArgumentNullException>(() => fastText.LoadModel((Stream)null!));
    }

    [SkippableFact]
    public void FastTextRejectsNegativeCount()
    {
        Skip.IfNot(FastTextDetector.IsSupported());

        using var fastText = new FastTextDetector();
        fastText.LoadDefaultModel();

        Assert.Throws<ArgumentOutOfRangeException>(() => fastText.Predict(Constants.PHRASE_ENG_1, -1));
    }

    [SkippableFact]
    public void FastTextAcceptsEmbeddedNulText()
    {
        Skip.IfNot(FastTextDetector.IsSupported());

        using var fastText = new FastTextDetector();
        fastText.LoadDefaultModel();

        var predictions = fastText.Predict(Constants.MALFORMED_BYTES_1, 10);

        Assert.NotNull(predictions);
    }

    [SkippableFact]
    public async Task FastTextPredictAndDisposeRaceDoesNotCrash()
    {
        Skip.IfNot(FastTextDetector.IsSupported());

        var fastText = new FastTextDetector();
        fastText.LoadDefaultModel();
        var predict = Task.Run(() =>
        {
            try
            {
                _ = fastText.Predict(Constants.PHRASE_ENG_1, 10).ToArray();
            }
            catch (ObjectDisposedException)
            {
            }
        });
        var dispose = Task.Run(fastText.Dispose);

        await Task.WhenAll(predict, dispose);
    }

    [SkippableFact]
    public void FastTextReturnsEmptyForZeroCount()
    {
        Skip.IfNot(FastTextDetector.IsSupported());

        using var fastText = new FastTextDetector();
        fastText.LoadDefaultModel();

        var predictions = fastText.Predict(Constants.PHRASE_ENG_1, 0);

        Assert.Empty(predictions);
    }

    [SkippableFact]
    public void FastTextModelDimensionsAreAvailableAfterLoad()
    {
        Skip.IfNot(FastTextDetector.IsSupported());

        using var fastText = new FastTextDetector();
        fastText.LoadDefaultModel();

        var dimensions = fastText.GetModelDimensions();

        Assert.True(dimensions > 0);
    }

    [SkippableFact]
    public void FastTextThrowsAfterDispose()
    {
        Skip.IfNot(FastTextDetector.IsSupported());

        var fastText = new FastTextDetector();
        fastText.LoadDefaultModel();
        fastText.Dispose();

        Assert.Throws<ObjectDisposedException>(() => fastText.Predict(Constants.PHRASE_ENG_1, 10).ToArray());
        Assert.Throws<ObjectDisposedException>(() => fastText.GetLabels().ToArray());
        Assert.Throws<ObjectDisposedException>(() => fastText.GetModelDimensions());
        Assert.Throws<ObjectDisposedException>(() => fastText.LoadModel(_modelPath));
        Assert.Throws<ObjectDisposedException>(() => fastText.LoadModel(new MemoryStream(Array.Empty<byte>())));
        Assert.Throws<ObjectDisposedException>(() => fastText.LoadDefaultModel());
    }

    [SkippableFact]
    public async Task FastTextLoadModelAndDisposeRaceDoesNotCrash()
    {
        Skip.IfNot(FastTextDetector.IsSupported());

        var fastText = new FastTextDetector();
        var load = Task.Run(() =>
        {
            try
            {
                using var stream = File.OpenRead(_modelPath);
                fastText.LoadModel(stream);
            }
            catch (ObjectDisposedException)
            {
            }
        });
        var dispose = Task.Run(fastText.Dispose);

        await Task.WhenAll(load, dispose);
    }

    [SkippableFact]
    public async Task FastTextParallelPredictionsDoNotCrash()
    {
        Skip.IfNot(FastTextDetector.IsSupported());

        using var fastText = new FastTextDetector();
        fastText.LoadDefaultModel();
        var tasks = Enumerable.Range(0, 32)
            .Select(i => Task.Run(() => fastText.Predict(i % 2 == 0 ? Constants.PHRASE_NOISY_1 : Constants.PHRASE_MIXED_1, 10).ToArray()));

        var results = await Task.WhenAll(tasks);

        Assert.All(results, predictions =>
        {
            Assert.NotNull(predictions);
            Assert.All(predictions, prediction => Assert.InRange(prediction.Probability, 0, 1));
        });
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
