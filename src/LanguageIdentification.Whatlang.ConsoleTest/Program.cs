namespace Panlingo.LanguageIdentification.Whatlang.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var whatlang = new WhatlangDetector();

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
                    $"Language: {x.Prediction?.Lang.ToString() ?? "NULL"}, " +
                    $"Probability: {x.Prediction?.Confidence.ToString() ?? "NULL"}, " +
                    $"IsReliable: {x.Prediction?.IsReliable.ToString() ?? "NULL"}, " +
                    $"Script: {x.Prediction?.Script.ToString() ?? "NULL"}"
                );
            }

            var code1 = whatlang.GetLangCode(WhatlangLanguage.Ukr);

            var code2 = whatlang.GetLangName(WhatlangLanguage.Ukr);

            var code3 = whatlang.GetScriptName(WhatlangScript.Cyrl);

            var code4 = whatlang.GetLangEngName(WhatlangLanguage.Ukr);

            ;
        }
    }
}
