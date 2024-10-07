using Panlingo.LanguageIdentification.CLD2;
using Panlingo.LanguageIdentification.Tests.Helpers;

namespace Panlingo.LanguageIdentification.Tests;

public class CLD2Tests
{
    [SkippableTheory]
    [InlineData("en", Constants.PHRASE_ENG_1, 0.9999)]
    [InlineData("uk", Constants.PHRASE_UKR_1, 0.9999)]
    [InlineData("un", Constants.PHRASE_RUS_1, 0)]
    public void CLD2SingleLanguage(string languageCode, string text, double score)
    {
        Skip.IfNot(CLD2Detector.IsSupported());

        using var cld2 = new CLD2Detector();

        var predictions = cld2.PredictLanguage(text);
        var mainLanguage = predictions.FirstOrDefault();

        if (mainLanguage is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, mainLanguage.Language);
        Assert.Equal(score, mainLanguage.Probability, Constants.EPSILON);
    }
}
