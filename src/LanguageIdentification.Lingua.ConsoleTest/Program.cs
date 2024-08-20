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
                "Hello, how are you? Привіт, як справи? Привет, как дела?",
                "Hello, how are you?",
                "Привіт, як справи?",
                "Привет, как дела?",
            };

            var predictions = texts
                .Select(x => new
                {
                    Text = x,
                    Prediction = lingua.PredictLanguage(x),
                    Predictions = lingua.PredictLanguages(x),
                })
                .ToArray();

            foreach (var x in predictions)
            {
                Console.WriteLine(
                    $"Text: {x.Text}, " +
                    $"Language: {x.Prediction?.Language.ToString() ?? "NULL"}, " +
                    $"Probability: {x.Prediction?.Confidence.ToString() ?? "NULL"}"
                );
            }

            var predictionsMultiple = lingua.PredictLanguages(string.Join("", texts));

            var code1 = lingua.GetLanguageCode(LinguaLanguage.Ukrainian, LinguaLanguageCode.Alpha2);
            var code2 = lingua.GetLanguageCode(LinguaLanguage.Ukrainian, LinguaLanguageCode.Alpha3);

            ;
        }
    }
}
