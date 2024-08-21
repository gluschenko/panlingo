namespace Panlingo.LanguageIdentification.Lingua.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>());
            using var lingua = linguaBuilder.Build();

            var texts = new[]
            {
                "Привіт",
                "Привет",
                "Hello",
                "Hello, how are you? Привіт, як справи? Hello, how are you? Привет, как дела?",
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

            var predictionsMultiple = lingua.PredictMixedLanguages(string.Join(" ", texts));

            var code1 = lingua.GetLanguageCode(LinguaLanguage.Ukrainian, LinguaLanguageCode.Alpha2);
            var code2 = lingua.GetLanguageCode(LinguaLanguage.Ukrainian, LinguaLanguageCode.Alpha3);
            var code3 = lingua.GetLanguageCode(LinguaLanguage.Ukrainian, (LinguaLanguageCode)99);
            var code4 = lingua.GetLanguageCode((LinguaLanguage)255, LinguaLanguageCode.Alpha3);
            

            ;
        }
    }
}
