namespace Panlingo.LanguageIdentification.Lingua.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var whatlang = new LinguaDetector(Enum.GetValues<LinguaLanguage>());

            var texts = new[]
            {
                "Hello, how are you?",
                "Привіт, як справи?",
                "Привет, как дела?",
            };

            var predictions = texts
                .Select(x => new
                {
                    Text = x,
                    Prediction = whatlang.PredictLanguage(x),
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

            var code1 = whatlang.GetLanguageCode(LinguaLanguage.Ukrainian);

            ;
        }
    }
}
