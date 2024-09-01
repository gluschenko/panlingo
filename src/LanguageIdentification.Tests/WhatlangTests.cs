using Panlingo.LanguageIdentification.Whatlang;
using Panlingo.LanguageIdentification.Tests.Helpers;

namespace Panlingo.LanguageIdentification.Tests;

public class WhatlangTests
{
    [Theory]
    [InlineData(WhatlangLanguage.Ron, Constants.PHRASE_ENG_1, 0.0274)]
    [InlineData(WhatlangLanguage.Ukr, Constants.PHRASE_UKR_1, 0.9999)]
    [InlineData(WhatlangLanguage.Rus, Constants.PHRASE_RUS_1, 0.2308)]
    public void WhatlangSingleLanguage(WhatlangLanguage languageCode, string text, double score)
    {
        using var whatlang = new WhatlangDetector();

        var prediction = whatlang.PredictLanguage(text: text);

        if (prediction is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, prediction.Language);
        Assert.Equal(score, prediction.Confidence, Constants.EPSILON);
    }

    [Theory]
    [InlineData(WhatlangLanguage.Ukr, "ukr")]
    [InlineData(WhatlangLanguage.Uzb, "uzb")]
    [InlineData(WhatlangLanguage.Heb, "heb")]
    [InlineData(WhatlangLanguage.Srp, "srp")]
    public void WhatlangGetLanguageCode(WhatlangLanguage language, string code)
    {
        using var whatlang = new WhatlangDetector();

        var languageCode = whatlang.GetLanguageCode(language);
        Assert.Equal(code, languageCode);
    }
}
