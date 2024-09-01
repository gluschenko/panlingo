using Panlingo.LanguageIdentification.CLD3;
using Panlingo.LanguageIdentification.Tests.Helpers;

namespace Panlingo.LanguageIdentification.Tests;

public class CLD3Tests
{
    [Theory]
    [InlineData("en", Constants.PHRASE_ENG_1, 0.9985)]
    [InlineData("uk", Constants.PHRASE_UKR_1, 0.9992)]
    [InlineData("ru", Constants.PHRASE_RUS_1, 0.9770)]
    public void CLD3SingleLanguage(string languageCode, string text, double score)
    {
        using var cld3 = new CLD3Detector(0, 512);

        var prediction = cld3.PredictLanguage(text: text);
        var predictions = cld3.PredictLanguages(text: text, count: 3);
        var mainLanguage = predictions.FirstOrDefault();

        if (prediction is null || mainLanguage is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, prediction.Language);
        Assert.Equal(languageCode, mainLanguage.Language);
        Assert.Equal(score, mainLanguage.Probability, Constants.EPSILON);
    }
}
