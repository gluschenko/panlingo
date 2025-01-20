using Panlingo.LanguageIdentification.MediaPipe;
using Panlingo.LanguageIdentification.Tests.Helpers;

namespace Panlingo.LanguageIdentification.Tests;

public class MediaPipeTests : IAsyncLifetime
{
    private readonly string _modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models/mediapipe_language_detector.tflite");

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
