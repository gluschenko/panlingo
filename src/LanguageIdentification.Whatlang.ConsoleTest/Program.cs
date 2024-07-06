namespace LanguageIdentification.Whatlang.ConsoleTest
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

            var list = new List<IEnumerable<WhatlangPredictionResult>>();

            foreach (var text in texts)
            {
                var predictions = whatlang.PredictLanguage(text);
                list.Add(predictions);
            }

            foreach (var lang in list.SelectMany(x => x))
            {
                Console.WriteLine($"Language: {lang.Lang}, Probability: {lang.Confidence}, IsReliable: {lang.IsReliable}, Script: {lang.Script}");
            }

            var code1 = whatlang.GetLangCode(WhatLangLang.Ukr);

            var code2 = whatlang.GetLangName(WhatLangLang.Ukr);

            var code3 = whatlang.GetScriptName(WhatLangScript.Cyrl);

            var code4 = whatlang.GetLangEngName(WhatLangLang.Ukr);

            ;
        }
    }
}
