using Panlingo.LanguageIdentification.CLD3;

namespace Panlingo.LanguageIdentification.Tests;

public class CLD3Tests
{
    [Theory]
    [InlineData("en", "Hello, how are you?")]
    [InlineData("uk", "Привіт, як справи?")]
    [InlineData("un", "Привет, как дела?")]
    public void CLD3SingleLanguage(string languageCode, string text)
    {
        using var cld3 = new CLD3Detector(0, 512);

        var prediction = cld3.PredictLanguage(text: text);
        var predictions = cld3.PredictLangauges(text: text, numLangs: 3);
        var mainLanguage = predictions.FirstOrDefault();

        if (prediction is null || mainLanguage is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, prediction.Language);
        Assert.Equal(languageCode, mainLanguage.Language);
    }
}
