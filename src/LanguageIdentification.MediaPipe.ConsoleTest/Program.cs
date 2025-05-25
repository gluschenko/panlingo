namespace Panlingo.LanguageIdentification.MediaPipe.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // var modelPath = "/models/mediapipe_language_detector.tflite";
            // using var stream = File.Open(modelPath, FileMode.Open);

            // using var mediaPipe = new MediaPipeDetector(
            //     options: MediaPipeOptions.FromStream(stream)
            // );

            // using var mediaPipe = new MediaPipeDetector(
            //     options: MediaPipeOptions.FromFile(modelPath)
            // );

            using var mediaPipe = new MediaPipeDetector(
                options: MediaPipeOptions.FromDefault()
            );

            var labels = mediaPipe.GetLabels();

            var text = "Привіт, як справи?";

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
