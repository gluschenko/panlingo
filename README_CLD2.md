# Panlingo.LanguageIdentification.CLD2

Welcome to **Panlingo.LanguageIdentification.CLD2**, a .NET wrapper for the Chrome Language Detection (CLD2) library by Google Inc. This package seamlessly integrates language identification capabilities into .NET applications, enabling accurate and efficient recognition of over 80 languages with minimal effort. Perfect for applications dealing with multilingual texts or requiring automatic language detection.

## Table of Contents

- [Requirements](#requirements)
- [Installation](#installation)
- [Usage](#usage)
- [API](#api)
- [Alternatives](#alternatives)

## Requirements

- .NET >= 5.0
- Linux

## Installation

To integrate the CLD2 functionality, you need to add this NuGet package to your project:

```sh
dotnet add package Panlingo.LanguageIdentification.CLD2
```

## Usage

Integrating the CLD2 library into your .NET application is straightforward. Here’s a quick guide to get you started:

1. **Install the Package**: Ensure you have added the `Panlingo.LanguageIdentification.CLD2` package to your project using the provided installation command.
2. **Initialize the Library**: Follow the example snippet to initialize and use the CLD2 library for detecting languages.

```csharp
using Panlingo.LanguageIdentification.CLD2;

class Program
{
    static void Main()
    {
        // Create an instance of the language detector
        var detector = new LanguageDetector();

        // Input text to detect language
        string text = "こんにちは、世界！";

        // Detect and print the language
        var language = detector.DetectLanguage(text);
        Console.WriteLine($"Detected language: {language}");
    }
}
```

## Alternatives

If you are exploring other options, here are some alternatives to consider:

- **[diadistis/cld2.net](https://github.com/diadistis/cld2.net)**: An unmaintained wrapper for .NET.
- **[nuvi/NCLD2](https://github.com/nuvi/NCLD2)**: Another unmaintained wrapper for .NET ([NuGet](https://www.nuget.org/packages/NCLD2)).
- **[curiosity-ai/catalyst](https://github.com/curiosity-ai/catalyst)**: Contains CLD2, with an example available [here](https://github.com/curiosity-ai/catalyst/blob/master/samples/LanguageDetection/Program.cs).

---

We value your feedback. Feel free to open issues or contribute to the repository. Let’s make language detection in .NET even more powerful and versatile! 🌍📝

Happy coding! 👩‍💻👨‍💻

---

Stay updated by following our repository. For any inquiries or support, reach out through the [issues page](https://github.com/gluschenko/language-identification/issues).