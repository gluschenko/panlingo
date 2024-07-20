# Panlingo.LanguageIdentification.Whatlang

Welcome to **Panlingo.LanguageIdentification.Whatlang**, a .NET wrapper for the Whatlang library, which is an efficient and easy-to-use language detection library. This package simplifies the integration of language identification capabilities into .NET applications, leveraging the Whatlang library to accurately and quickly recognize the languages of given texts. Perfect for projects that require lightweight yet reliable language detection.

## Table of Contents

- [Requirements](#requirements)
- [Installation](#installation)
- [Usage](#usage)

## Requirements

- .NET >= 5.0
- Linux

## Installation

To integrate the Whatlang functionality, follow these steps:

1. **Install the NuGet package**:

   ```sh
   dotnet add package Panlingo.LanguageIdentification.Whatlang
   ```

## Usage

Integrating the Whatlang library into your .NET application is straightforward. Here’s a quick guide to get you started:

1. **Install the Package**: Ensure you have added the `Panlingo.LanguageIdentification.Whatlang` package to your project using the provided installation command.
2. **Initialize the Library**: Follow the example snippet to initialize and use the Whatlang library for detecting languages.

```csharp
using Panlingo.LanguageIdentification.Whatlang;

class Program
{
    static void Main()
    {
        // Create an instance of the language detector
        using var whatlang = new WhatlangDetector();

        var texts = new[]
        {
            "Hello, how are you?",
            "Привіт, як справи?",
            "Привет, как дела?",
        };

        var predictions = texts
            .Select(x => new
            {
                Text = x,
                Prediction = whatlang.PredictLanguage(x),
            })
            .ToArray();

        foreach (var x in predictions)
        {
            Console.WriteLine(
                $"Text: {x.Text}, " +
                $"Language: {x.Prediction?.Lang.ToString() ?? "NULL"}, " +
                $"Probability: {x.Prediction?.Confidence.ToString() ?? "NULL"}, " +
                $"IsReliable: {x.Prediction?.IsReliable.ToString() ?? "NULL"}, " +
                $"Script: {x.Prediction?.Script.ToString() ?? "NULL"}"
            );
        }
    }
}
```

---

We value your feedback. Feel free to open issues or contribute to the repository. Let’s make language detection in .NET even more powerful and versatile! 🌍📝

Happy coding! 👩‍💻👨‍💻

---

Stay updated by following our repository. For any inquiries or support, reach out through the [issues page](https://github.com/gluschenko/language-identification/issues).