using Panlingo.LanguageIdentification.FastText;
using Panlingo.LanguageIdentification.Tests.Helpers;

namespace Panlingo.LanguageIdentification.Tests;

public class FastTextTests
{
    [SkippableTheory]
    [InlineData("__label__en", Constants.PHRASE_ENG_1, 0.9955)]
    [InlineData("__label__uk", Constants.PHRASE_UKR_1, 0.9900)]
    [InlineData("__label__ru", Constants.PHRASE_RUS_1, 0.9983)]
    public void FastTextFileSingleLanguage(string languageCode, string text, double score)
    {
        Skip.IfNot(FastTextDetector.IsSupported());

        using var fastText = new FastTextDetector();

        var modelPath = "/models/fasttext176.bin";
        fastText.LoadModel(modelPath);

        var predictions = fastText.Predict(text: text, count: 10);
        var mainLanguage = predictions.FirstOrDefault();

        if (mainLanguage is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, mainLanguage.Label);
        Assert.Equal(score, mainLanguage.Probability, Constants.EPSILON);
    }

    [SkippableTheory]
    [InlineData("__label__en", Constants.PHRASE_ENG_1, 0.9955)]
    [InlineData("__label__uk", Constants.PHRASE_UKR_1, 0.9900)]
    [InlineData("__label__ru", Constants.PHRASE_RUS_1, 0.9983)]
    public void FastTextStreamSingleLanguage(string languageCode, string text, double score)
    {
        Skip.IfNot(FastTextDetector.IsSupported());

        using var fastText = new FastTextDetector();

        var modelPath = "/models/fasttext176.bin";
        using var stream = File.Open(modelPath, FileMode.Open);

        fastText.LoadModel(stream);

        var predictions = fastText.Predict(text: text, count: 10);
        var mainLanguage = predictions.FirstOrDefault();

        if (mainLanguage is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, mainLanguage.Label);
        Assert.Equal(score, mainLanguage.Probability, Constants.EPSILON);
    }

    [SkippableTheory]
    [InlineData("__label__en", Constants.PHRASE_ENG_1, 1.0000)]
    [InlineData("__label__uk", Constants.PHRASE_UKR_1, 0.8511)]
    [InlineData("__label__ru", Constants.PHRASE_RUS_1, 0.9693)]
    public void FastTextContainedSingleLanguage(string languageCode, string text, double score)
    {
        Skip.IfNot(FastTextDetector.IsSupported());

        using var fastText = new FastTextDetector();
        fastText.LoadDefaultModel();

        var predictions = fastText.Predict(text: text, count: 10);
        var mainLanguage = predictions.FirstOrDefault();

        if (mainLanguage is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, mainLanguage.Label);
        Assert.Equal(score, mainLanguage.Probability, Constants.EPSILON);
    }

    [SkippableFact]
    public void FastTextLabels()
    {
        Skip.IfNot(FastTextDetector.IsSupported());

        using var fastText = new FastTextDetector();

        var modelPath = "/models/fasttext176.bin";
        fastText.LoadModel(modelPath);

        var labels = fastText.GetLabels();

        Assert.Contains(labels, x => x.Label == "__label__en");
        Assert.Contains(labels, x => x.Label == "__label__uk");
        Assert.Contains(labels, x => x.Label == "__label__ru");
    }
}
