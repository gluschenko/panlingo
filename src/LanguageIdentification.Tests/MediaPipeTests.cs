using System.IO;
using System.Runtime.InteropServices;
using Panlingo.LanguageIdentification.MediaPipe;
using Panlingo.LanguageIdentification.Tests.Helpers;

namespace Panlingo.LanguageIdentification.Tests;

public class MediaPipeTests
{
    private static string GetModelPath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "~/models/mediapipe_language_detector.tflite";
        }

        return "/models/mediapipe_language_detector.tflite";
    }

    [SkippableTheory]
    [InlineData("en", Constants.PHRASE_ENG_1, 0.9994)]
    [InlineData("uk", Constants.PHRASE_UKR_1, 0.9999)]
    [InlineData("ru", Constants.PHRASE_RUS_1, 0.9999)]
    public void MediaPipeFileSingleLanguage(string languageCode, string text, double score)
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        var modelPath = GetModelPath();
        using var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromFile(modelPath).WithResultCount(10)
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

        var modelPath = GetModelPath();
        using var stream = File.Open(modelPath, FileMode.Open);

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

        var modelPath = GetModelPath();
        using var stream = File.Open(modelPath, FileMode.Open);

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
}
