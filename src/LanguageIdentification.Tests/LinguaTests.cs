using Panlingo.LanguageIdentification.Lingua;

namespace Panlingo.LanguageIdentification.Tests;

public class LinguaTests
{
    [Theory]
    [InlineData(LinguaLanguage.English, "Hello, how are you?")]
    [InlineData(LinguaLanguage.Ukrainian, "Привіт, як справи?")]
    [InlineData(LinguaLanguage.Russian, "Привет, как дела?")]
    public void LinguaSingleLanguage(LinguaLanguage languageCode, string text)
    {
        using var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>());
        using var lingua = linguaBuilder.Build();

        var prediction = lingua.PredictLanguage(text: text);

        if (prediction is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, prediction.Language);
    }
}
