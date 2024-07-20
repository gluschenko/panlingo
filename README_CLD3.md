# Panlingo.LanguageIdentification.CLD3

Welcome to **Panlingo.LanguageIdentification.CLD3**, a .NET wrapper for the Chrome Language Detection (CLD3) library by Google Inc. This package seamlessly integrates language identification capabilities into .NET applications, enabling accurate and efficient recognition of over 107 languages with minimal effort. Perfect for applications dealing with multilingual texts or requiring automatic language detection.

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

To integrate the CLD3 functionality, follow these steps:

1. **Install the NuGet package**:

   ```sh
   dotnet add package Panlingo.LanguageIdentification.CLD3
   ```

2. **Install Protobuf**:

   Depending on your Linux distribution, use the appropriate command to install Protobuf:

   - **Ubuntu/Debian**:
     ```sh
     sudo apt -y update
     sudo apt -y install protobuf-compiler libprotobuf-dev
     ```

   - **Fedora**:
     ```sh
     sudo yum install protobuf-devel
     ```

   - **CentOS**:
     ```sh
     sudo yum install epel-release
     sudo yum install protobuf-devel
     ```

## Usage

Integrating the CLD3 library into your .NET application is straightforward. Here’s a quick guide to get you started:

1. **Install the Package**: Ensure you have added the `Panlingo.LanguageIdentification.CLD3` package to your project using the provided installation command.
2. **Initialize the Library**: Follow the example snippet to initialize and use the CLD3 library for detecting languages.

```csharp
using Panlingo.LanguageIdentification.CLD3;

class Program
{
    static void Main()
    {
        // Create an instance of the language detector
        var detector = new LanguageDetector();

        // Input text to detect language
        string text = "Hello, world!";

        // Detect and print the language
        var language = detector.DetectLanguage(text);
        Console.WriteLine($"Detected language: {language}");
    }
}
```

## Alternatives

If you are exploring other options, here are some alternatives to consider:

- **[NikulovE/cld3.net](https://github.com/NikulovE/cld3.net)**: An unmaintained wrapper for .NET.
- **[uranium62/cld3-net](https://github.com/uranium62/cld3-net)**: Another unmaintained wrapper for .NET.

---

We value your feedback. Feel free to open issues or contribute to the repository. Let’s make language detection in .NET even more powerful and versatile! 🌍📝

Happy coding! 👩‍💻👨‍💻

---

Stay updated by following our repository. For any inquiries or support, reach out through the [issues page](https://github.com/gluschenko/language-identification/issues).