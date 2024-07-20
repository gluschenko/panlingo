namespace Panlingo.LanguageIdentification.FastText.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var text = "Hello, how are you? Привіт, як справи? Привет, как дела?";
            var modelPath = "/models/fasttext217.bin";

            using var fastText = new FastTextDetector();

            fastText.LoadModel(modelPath);

            var dimensions = fastText.GetModelDimensions();

            var predictions = fastText.Predict(text: text, count: 10);

            var labels = fastText.GetLabels();

            foreach (var prediction in predictions)
            {
                Console.WriteLine($"{prediction.Label}: {prediction.Probability}");
            }
        }
    }
}
