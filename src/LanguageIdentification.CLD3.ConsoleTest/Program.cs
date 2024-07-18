namespace Panlingo.LanguageIdentification.CLD3.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var cld3 = new CLD3Detector(0, 512);

            string text = "Hello, how are you? Привіт, як справи? Привет, как дела?";

            var result = cld3.PredictLanguage(text);

            Console.WriteLine($"Language: {result.Language}");
            Console.WriteLine($"Probability: {result.Probability}");
            Console.WriteLine($"IsReliable: {result.IsReliable}");
            Console.WriteLine($"Proportion: {result.Proportion}");

            var topLangs = cld3.PredictLangauges(text, 3);

            foreach (var lang in topLangs)
            {
                Console.WriteLine($"Language: {lang.Language}, Probability: {lang.Probability}, IsReliable: {lang.IsReliable}, Proportion: {lang.Proportion}");
            }

            ;
        }
    }
}
