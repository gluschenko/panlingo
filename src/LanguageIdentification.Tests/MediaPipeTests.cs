using Panlingo.LanguageIdentification.MediaPipe;
using Panlingo.LanguageIdentification.Tests.Helpers;

namespace Panlingo.LanguageIdentification.Tests;

public class MediaPipeTests
{
    [Theory]
    [InlineData("en", Constants.PHRASE_ENG_1, 0.9999)]
    [InlineData("uk", Constants.PHRASE_UKR_1, 0.9999)]
    [InlineData("ru", Constants.PHRASE_RUS_1, 0.9999)]
    public void MediaPipeSingleLanguage(string languageCode, string text, double score)
    {
        var modelPath = "/models/mediapipe_language_detector.tflite";
        using var mediaPipe = new MediaPipeDetector(resultCount: 10, modelPath: modelPath);

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
