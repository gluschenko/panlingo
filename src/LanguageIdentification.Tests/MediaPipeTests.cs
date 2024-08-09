using Panlingo.LanguageIdentification.MediaPipe;

namespace Panlingo.LanguageIdentification.Tests;

public class MediaPipeTests
{
    /*[Theory]
    [InlineData("en", "Hello, how are you?")]
    [InlineData("uk", "Привіт, як справи?")]
    [InlineData("ru", "Привет, как дела?")]
    public void MediaPipeSingleLanguage(string languageCode, string text)
    {
        using var mediaPipe = new MediaPipeDetector(resultCount: 10);

        var predictions = mediaPipe.PredictLanguages(text: text);
        var mainLanguage = predictions.FirstOrDefault();

        if (mainLanguage is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, mainLanguage.Language);
    }*/
}
