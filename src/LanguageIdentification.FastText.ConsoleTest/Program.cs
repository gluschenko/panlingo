namespace LanguageIdentification.FastText.ConsoleTest
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

            var predictions = fastText.Predict(text, 10);

            var labels = fastText.GetLabels();

            Console.WriteLine($"{text}:");

            foreach (var x in predictions)
            {
                Console.WriteLine($"{x.Label.Replace("__label__", "")}: {x.Probability}");
            }
        }
    }
}
