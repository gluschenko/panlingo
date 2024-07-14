namespace Panlingo.LanguageIdentification.CLD2.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var cld2 = new CLD2Detector();

            string text = "Hello, how are you? Привіт, як справи? Привет, как дела?";

            var topLangs = cld2.PredictLanguage(text);

            foreach (var lang in topLangs)
            {
                Console.WriteLine($"Language: {lang.Language}, Probability: {lang.Probability}, IsReliable: {lang.IsReliable}, Proportion: {lang.Proportion}");
            }

            ;
        }
    }
}
