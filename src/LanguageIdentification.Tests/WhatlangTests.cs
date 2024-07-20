using Panlingo.LanguageIdentification.Whatlang;

namespace Panlingo.LanguageIdentification.Tests;

public class WhatlangTests
{
    [Theory]
    [InlineData(WhatlangLanguage.Ron, "Hello, how are you?")]
    [InlineData(WhatlangLanguage.Ukr, "Привіт, як справи?")]
    [InlineData(WhatlangLanguage.Rus, "Привет, как дела?")]
    public void WhatlangSingleLanguage(WhatlangLanguage languageCode, string text)
    {
        using var whatlang = new WhatlangDetector();

        var prediction = whatlang.PredictLanguage(text: text);

        if (prediction is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, prediction.Language);
    }
}
