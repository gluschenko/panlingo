﻿using Panlingo.LanguageIdentification.CLD3;
using Panlingo.LanguageIdentification.Tests.Helpers;

namespace Panlingo.LanguageIdentification.Tests;

public class CLD3Tests
{
    [SkippableTheory]
    [InlineData("en", Constants.PHRASE_ENG_1, 0.9985)]
    [InlineData("uk", Constants.PHRASE_UKR_1, 0.9992)]
    [InlineData("ru", Constants.PHRASE_RUS_1, 0.9770)]
    public void CLD3SingleLanguage(string languageCode, string text, double score)
    {
        Skip.IfNot(CLD3Detector.IsSupported());

        using var cld3 = new CLD3Detector(0, 512);

        var prediction = cld3.PredictLanguage(text: text);
        var predictions = cld3.PredictLanguages(text: text, count: 3);

        if (prediction is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, prediction.Language);
        Assert.Equal(score, prediction.Probability, Constants.EPSILON);
    }

    [SkippableTheory]
    [InlineData("en", Constants.PHRASE_ENG_1, 0.9985)]
    [InlineData("uk", Constants.PHRASE_UKR_1, 0.9992)]
    [InlineData("ru", Constants.PHRASE_RUS_1, 0.9770)]
    public void CLD3MixedLanguage(string languageCode, string text, double score)
    {
        Skip.IfNot(CLD3Detector.IsSupported());

        using var cld3 = new CLD3Detector(0, 512);

        var predictions = cld3.PredictLanguages(text: text, count: 3);
        var mainLanguage = predictions.FirstOrDefault();

        if (mainLanguage is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, mainLanguage.Language);
        Assert.Equal(score, mainLanguage.Probability, Constants.EPSILON);
    }
}
