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

            var list = new List<WhatlangPredictionResult>();

            foreach (var text in texts)
            {
                var prediction = whatlang.PredictLanguage(text);
                if (prediction.HasValue)
                {
                    list.Add(prediction.Value);
                }
            }

            foreach (var lang in list)
            {
                Console.WriteLine($"Language: {lang.Lang}, Probability: {lang.Confidence}, IsReliable: {lang.IsReliable}, Script: {lang.Script}");
            }

            var code1 = whatlang.GetLangCode(WhatlangLanguage.Ukr);

            var code2 = whatlang.GetLangName(WhatlangLanguage.Ukr);

            var code3 = whatlang.GetScriptName(WhatlangScriptX.Cyrl);

            var code4 = whatlang.GetLangEngName(WhatlangLanguage.Ukr);

            ;
        }
    }
}
