using Panlingo.LanguageIdentification.FastText;
using Panlingo.LanguageIdentification.Tests.Helpers;

namespace Panlingo.LanguageIdentification.Tests;

public class FastTextTests
{
    [Theory]
    [InlineData("__label__en", Constants.PHRASE_ENG_1, 0.9999)]
    [InlineData("__label__uk", Constants.PHRASE_UKR_1, 0.9999)]
    [InlineData("__label__ru", Constants.PHRASE_RUS_1, 0.9999)]
    public void FastTextSingleLanguage(string languageCode, string text, double score)
    {
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

    [Fact]
    public void FastTextLabels()
    {
        using var fastText = new FastTextDetector();

        var modelPath = "/models/fasttext176.bin";
        fastText.LoadModel(modelPath);

        var labels = fastText.GetLabels();

        Assert.Contains(labels, x => x.Label == "__label__en");
        Assert.Contains(labels, x => x.Label == "__label__uk");
        Assert.Contains(labels, x => x.Label == "__label__ru");
    }
}
