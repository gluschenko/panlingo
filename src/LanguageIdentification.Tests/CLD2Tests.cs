using Panlingo.LanguageIdentification.CLD2;

namespace Panlingo.LanguageIdentification.Tests;

public class CLD2Tests
{
    [Theory]
    [InlineData("en", "Hello, how are you?")]
    [InlineData("uk", "Привіт, як справи?")]
    [InlineData("un", "Привет, как дела?")]
    public void CLD2SingleLanguage(string languageCode, string text)
    {
        using var cld2 = new CLD2Detector();

        var predictions = cld2.PredictLanguage(text);
        var mainLanguage = predictions.FirstOrDefault();

        Assert.Equal(languageCode, mainLanguage.Language);
    }
}
