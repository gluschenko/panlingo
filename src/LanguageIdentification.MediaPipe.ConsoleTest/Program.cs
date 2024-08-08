namespace Panlingo.LanguageIdentification.MediaPipe.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var mediaPipe = new MediaPipeDetector();

            var text = "Hello, how are you? Привіт, як справи? Привет, как дела?";

            var singlePrediction = mediaPipe.PredictLanguages(text);

            var predictions = mediaPipe.PredictLanguages(text);

            foreach (var prediction in predictions)
            {
                Console.WriteLine(
                    $"Language: {prediction.Language}, " +
                    $"Probability: {prediction.Probability}"
                );
            }
        }
    }
}
