namespace Panlingo.LanguageIdentification.FastText.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var text = "Привіт, як справи?";

            using var fastText = new FastTextDetector();

            // var modelPath = "/models/fasttext176.ftz";
            // fastText.LoadModel(modelPath);

            // using var stream = File.Open(modelPath, FileMode.Open);
            // fastText.LoadModel(stream);

            fastText.LoadDefaultModel();

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
