using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Panlingo.LanguageIdentification.Lingua.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>())
                .WithPreloadedLanguageModels()
                .WithMinimumRelativeDistance(0.95)
                .WithLowAccuracyMode();

            using var lingua = linguaBuilder.Build();

            var texts = new[]
            {
                "\u0080",
                "\u00BF",
                "hello\0world",
                "\0",
                "\u0080",
                "\u00BF",
                "\uD800",
                "\uDC00",
                "\uFFFF",
                "\uFFFE",
                "\uFEFF" + "Hello",
                "A" + "\u200D" + "B",
                "A" + "\u200C" + "B",
                "abc" + "\u202E" + "def",
                "Привіт",
                "Привет",
                "Hello",
                "Hello, how are you?",
                "Привіт, як справи?",
                "Привет, как дела?",
            };

            var predictions = texts
                .Select(x => new
                {
                    Text = x,
                    Predictions = lingua.PredictLanguages(x),
                })
                .ToArray();

            foreach (var x in predictions)
            {
                var prediction = x.Predictions.FirstOrDefault();

                Console.WriteLine(
                    $"Text: {x.Text}, " +
                    $"Language: {prediction?.Language.ToString() ?? "NULL"}, " +
                    $"Probability: {prediction?.Confidence.ToString() ?? "NULL"}"
                );
            }

            var mixedText = string.Join(" ", texts);
            var predictionsMultiple1 = lingua.PredictLanguages(mixedText);
            var predictionsMultiple2 = lingua.PredictMixedLanguages(mixedText);

            var code1 = lingua.GetLanguageCode(LinguaLanguage.Ukrainian, LinguaLanguageCode.Alpha2);
            var code2 = lingua.GetLanguageCode(LinguaLanguage.Ukrainian, LinguaLanguageCode.Alpha3);
            // var code3 = lingua.GetLanguageCode(LinguaLanguage.Ukrainian, (LinguaLanguageCode)99);
            // var code4 = lingua.GetLanguageCode((LinguaLanguage)255, LinguaLanguageCode.Alpha3);


            ;
        }
    }
}
