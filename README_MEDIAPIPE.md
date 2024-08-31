# Panlingo.LanguageIdentification.MediaPipe

Welcome to **Panlingo.LanguageIdentification.MediaPipe**, a .NET wrapper for the MediaPipe library by Google Inc. This package seamlessly integrates language identification capabilities into .NET applications, enabling accurate and efficient recognition of over 107 languages with minimal effort. Perfect for applications dealing with multilingual texts or requiring automatic language detection.

## Requirements

- Runtime: **.NET >= 5.0**
- OS: **Linux**
- Arch: **AMD64**

## Installation

To integrate the MediaPipe functionality, follow these steps:

1. **Install the NuGet package**:

   ```sh
   dotnet add package Panlingo.LanguageIdentification.MediaPipe
   ```

2. **Download the Pretrained Models**:

   Download the pretrained language identification (LID) model provided by Google:

     ```sh
     curl --location -o /models/mediapipe_language_detector.tflite https://storage.googleapis.com/mediapipe-models/language_detector/language_detector/float32/1/language_detector.tflite
     ```

   Learn more about this model here:
   - [Google AI Edge](https://ai.google.dev/edge/mediapipe/solutions/text/language_detector)

## Usage

Integrating the MediaPipe library into your .NET application is straightforward. Here’s a quick guide to get you started:

1. **Install the Package**: Ensure you have added the `Panlingo.LanguageIdentification.MediaPipe` package to your project using the provided installation command.
2. **Download the Models**: Follow the provided commands to download the pretrained language identification models.
3. **Initialize the Library**: Follow the example snippet to initialize and use the MediaPipe library for detecting languages.

```csharp
using Panlingo.LanguageIdentification.MediaPipe;

class Program
{
    static void Main()
    {
        var modelPath = "/models/mediapipe_language_detector.tflite";
        using var mediaPipe = new MediaPipeDetector(resultCount: 10, modelPath: modelPath);

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
```

---

We value your feedback. Feel free to open issues or contribute to the repository. Let’s make language detection in .NET even more powerful and versatile! 🌍📝

Happy coding! 👩‍💻👨‍💻

---

