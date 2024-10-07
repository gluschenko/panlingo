namespace Panlingo.LanguageIdentification.CLD2.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var cld2 = new CLD2Detector();

            var text = "Привіт, як справи?";

            var predictions = cld2.PredictLanguage(text);

            foreach (var prediction in predictions)
            {
                Console.WriteLine(
                    $"Language: {prediction.Language}, " +
                    $"Probability: {prediction.Probability}, " +
                    $"IsReliable: {prediction.IsReliable}, " +
                    $"Proportion: {prediction.Proportion}"
                );
            }
        }
    }
}
