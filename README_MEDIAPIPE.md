# Panlingo.LanguageIdentification.MediaPipe

Welcome to **Panlingo.LanguageIdentification.MediaPipe**, a .NET wrapper for the MediaPipe library by Google Inc. This package seamlessly integrates language identification capabilities into .NET applications, enabling accurate and efficient recognition of over 107 languages with minimal effort. Perfect for applications dealing with multilingual texts or requiring automatic language detection.

## Requirements

- Runtime: **.NET >= 5.0**
- OS: **Linux**
- Arch: **AMD64**

## Installation

To integrate the MediaPipe functionality, follow these steps:

**Install the NuGet package**:

```sh
dotnet add package Panlingo.LanguageIdentification.MediaPipe
```

## Usage

Integrating the MediaPipe library into your .NET application is straightforward. Here’s a quick guide to get you started:

1. **Install the Package**: Ensure you have added the `Panlingo.LanguageIdentification.MediaPipe` package to your project using the provided installation command.
2. **Initialize the Library**: Follow the example snippet to initialize and use the MediaPipe library for detecting languages.

```csharp
using Panlingo.LanguageIdentification.MediaPipe;

class Program
{
    static void Main()
    {
        using var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromDefault()
        );

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

### Custom models

#### Download the Pretrained Models

Download the pretrained language identification (LID) model provided by Google:

```sh
curl --location -o /models/mediapipe_language_detector.tflite https://storage.googleapis.com/mediapipe-models/language_detector/language_detector/float32/1/language_detector.tflite
```

Learn more about this model here:
- [Google AI Edge](https://ai.google.dev/edge/mediapipe/solutions/text/language_detector)

#### Use custom model in code

**You can use the model included in this NuGet package:**
```
using var mediaPipe = new MediaPipeDetector(
    options: MediaPipeOptions.FromDefault()
);
```

**You can specify the path to the model file:**
```
var modelPath = "/models/mediapipe_language_detector.tflite";
using var mediaPipe = new MediaPipeDetector(
    options: MediaPipeOptions.FromFile(modelPath)
);
```

**Also you can also load the model as a memory stream:**
```
var modelPath = "/models/mediapipe_language_detector.tflite";
using var stream = File.Open(modelPath, FileMode.Open);

using var mediaPipe = new MediaPipeDetector(
    options: MediaPipeOptions.FromStream(stream)
);
```

## Sources

1. [Original MediaPipe Repository](https://github.com/google-ai-edge/mediapipe)
2. [Language detection guide](https://ai.google.dev/edge/mediapipe/solutions/text/language_detector)
3. [Language detector (model card)](https://storage.googleapis.com/mediapipe-assets/LanguageDetector%20Model%20Card.pdf)
4. [Language identification at Wikipedia](https://en.wikipedia.org/wiki/Language_identification)

## License

This project is licensed under the [MIT License](./LICENSE) © 2024–2025 Alexander Gluschenko.

Includes software from the following project(s):
- [MediaPipe Language Detector](https://github.com/google-ai-edge/mediapipe) — © 2023–2025 Google Inc., licensed under Apache-2.0  

See the [LICENSE](./LICENSE) file for full details.

---

We value your feedback. Feel free to open issues or contribute to the repository. Let’s make language detection in .NET even more powerful and versatile! 🌍📝

Happy coding! 👩‍💻👨‍💻

---

