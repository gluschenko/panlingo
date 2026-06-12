using System.Runtime.InteropServices;
using Panlingo.LanguageIdentification.MediaPipe;
using Panlingo.LanguageIdentification.Tests.Helpers;

namespace Panlingo.LanguageIdentification.Tests;

public class MediaPipeTests : IAsyncLifetime
{
    private readonly string _modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models/mediapipe_language_detector.tflite");

    [Fact]
    public void MediaPipeCheckPlatformSupport()
    {
        var isSupported = RuntimeInformation.OSArchitecture switch
        {
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
            Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => true,
            Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
            _ => false,
        };

        Assert.Equal(isSupported, MediaPipeDetector.IsSupported());
    }

    [SkippableTheory]
    [InlineData("en", Constants.PHRASE_ENG_1, 0.9994)]
    [InlineData("uk", Constants.PHRASE_UKR_1, 0.9999)]
    [InlineData("ru", Constants.PHRASE_RUS_1, 0.9999)]
    public void MediaPipeFileSingleLanguage(string languageCode, string text, double score)
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        using var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromFile(_modelPath).WithResultCount(10)
        );

        var predictions = mediaPipe.PredictLanguages(text: text);
        var mainLanguage = predictions.FirstOrDefault();

        if (mainLanguage is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, mainLanguage.Language);
        Assert.Equal(score, mainLanguage.Probability, Constants.EPSILON);
    }

    [SkippableTheory]
    [InlineData("en", Constants.PHRASE_ENG_1, 0.9994)]
    [InlineData("uk", Constants.PHRASE_UKR_1, 0.9999)]
    [InlineData("ru", Constants.PHRASE_RUS_1, 0.9999)]
    public void MediaPipeStreamSingleLanguage(string languageCode, string text, double score)
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        using var stream = File.Open(_modelPath, FileMode.Open);

        using var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromStream(stream).WithResultCount(10)
        );

        var predictions = mediaPipe.PredictLanguages(text: text);
        var mainLanguage = predictions.FirstOrDefault();

        if (mainLanguage is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, mainLanguage.Language);
        Assert.Equal(score, mainLanguage.Probability, Constants.EPSILON);
    }

    [SkippableTheory]
    [InlineData("en", Constants.PHRASE_ENG_1, 0.9994)]
    [InlineData("uk", Constants.PHRASE_UKR_1, 0.9999)]
    [InlineData("ru", Constants.PHRASE_RUS_1, 0.9999)]
    public void MediaPipeContainedSingleLanguage(string languageCode, string text, double score)
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        using var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromDefault().WithResultCount(10)
        );

        var predictions = mediaPipe.PredictLanguages(text: text);
        var mainLanguage = predictions.FirstOrDefault();

        if (mainLanguage is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, mainLanguage.Language);
        Assert.Equal(score, mainLanguage.Probability, Constants.EPSILON);
    }

    [SkippableFact]
    public void MediaPipeGetLables()
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        using var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromDefault().WithResultCount(10)
        );

        var labels = mediaPipe.GetLabels();
        Assert.Equal(111, labels.Count());
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
    public void MediaPipeMalformedBytes(string text)
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        using var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromDefault().WithResultCount(10)
        );

        var predictions = mediaPipe.PredictLanguages(text: text);
    }

    [SkippableFact]
    public void MediaPipeAcceptsNullText()
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        using var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromDefault().WithResultCount(10)
        );

        var predictions = mediaPipe.PredictLanguages(null!).ToArray();

        Assert.NotNull(predictions);
    }

    [SkippableFact]
    public void MediaPipeStreamModelCanReadLabels()
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        using var stream = File.Open(_modelPath, FileMode.Open);
        using var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromStream(stream).WithResultCount(10)
        );

        var labels = mediaPipe.GetLabels();

        Assert.Equal(111, labels.Count());
        Assert.Contains("en", labels);
        Assert.Contains("uk", labels);
    }

    [SkippableFact]
    public void MediaPipeThrowsAfterDispose()
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromDefault().WithResultCount(10)
        );
        mediaPipe.Dispose();

        Assert.Throws<ObjectDisposedException>(() => mediaPipe.PredictLanguages(Constants.PHRASE_ENG_1).ToArray());
        Assert.Throws<ObjectDisposedException>(() => mediaPipe.GetLabels().ToArray());
    }

    [SkippableFact]
    public async Task MediaPipeParallelPredictionsDoNotCrash()
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        using var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromDefault().WithResultCount(10)
        );
        var tasks = Enumerable.Range(0, 24)
            .Select(i => Task.Run(() => mediaPipe.PredictLanguages(i % 2 == 0 ? Constants.PHRASE_NOISY_1 : Constants.PHRASE_MIXED_1).ToArray()));

        var results = await Task.WhenAll(tasks);

        Assert.All(results, predictions =>
        {
            Assert.NotNull(predictions);
            Assert.All(predictions, prediction => Assert.InRange(prediction.Probability, 0, 1));
        });
    }

    [SkippableFact]
    public async Task MediaPipePredictAndDisposeRaceDoesNotCrash()
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromDefault().WithResultCount(10)
        );
        var predict = Task.Run(() =>
        {
            try
            {
                _ = mediaPipe.PredictLanguages(Constants.PHRASE_ENG_1).ToArray();
            }
            catch (ObjectDisposedException)
            {
            }
        });
        var dispose = Task.Run(mediaPipe.Dispose);

        await Task.WhenAll(predict, dispose);
    }

    public async Task InitializeAsync()
    {
        var url = "https://storage.googleapis.com/mediapipe-models/language_detector/language_detector/float32/1/language_detector.tflite";
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
