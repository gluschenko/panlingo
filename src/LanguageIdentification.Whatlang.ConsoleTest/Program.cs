namespace LanguageIdentification.Whatlang.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var cld2 = new WhatlangDetector();

            string text = "Hello, how are you? Привіт, як справи? Привет, как дела?";

            var topLangs = cld2.PredictLanguage(text);

            foreach (var lang in topLangs)
            {
                Console.WriteLine($"Language: {lang.Lang}, Probability: {lang.Confidence}, IsReliable: {lang.IsReliable}, Script: {lang.Script}");
            }

            ;
        }
    }
}
