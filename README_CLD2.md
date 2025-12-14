# Panlingo.LanguageIdentification.CLD2

Welcome to **Panlingo.LanguageIdentification.CLD2**, a .NET wrapper for the Compact Language Detector (CLD2) library by Google Inc. This package seamlessly integrates language identification capabilities into .NET applications, enabling accurate and efficient recognition of over 80 languages with minimal effort. Perfect for applications dealing with multilingual texts or requiring automatic language detection.

## Requirements

- Runtime: **.NET >= 5.0**
- OS: **Linux (Ubuntu, Debian)**, **Windows 10+** or **Windows Server 2019+**, **macOS**
- Arch: **AMD64** (or **ARM** for macOS)

## Platform support

| **OS / Arch** | x86_64             | arm64              |
| :------------ | :----------------: | :----------------: |
| **Linux**     | :white_check_mark: | :white_check_mark: |
| **Windows**   | :white_check_mark: | :x:                |
| **macOS**     | :white_check_mark: | :white_check_mark: |

:white_check_mark: — Full support |
:x: — No support |
:construction: — Under research

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
        using var cld2 = new CLD2Detector();

        // Input text to detect language
        var predictions = cld2.PredictLanguage("Привіт, як справи?");

        // Print the language
        foreach (var prediction in predictions)
        {
            Console.WriteLine(
                $"Language: {prediction.Language}, " + 
                $"Probability: {prediction.Probability}, " +
                $"IsReliable: {prediction.IsReliable}, " + 
                $"Proportion: {prediction.Proportion}"
            );
        }
    }
}
```

## Alternatives

If you are exploring other options, here are some alternatives to consider:

- **[diadistis/cld2.net](https://github.com/diadistis/cld2.net)**: An unmaintained wrapper for .NET.
- **[nuvi/NCLD2](https://github.com/nuvi/NCLD2)**: Another unmaintained wrapper for .NET ([NuGet](https://www.nuget.org/packages/NCLD2)).
- **[curiosity-ai/catalyst](https://github.com/curiosity-ai/catalyst)**: Contains CLD2, with an example available [here](https://github.com/curiosity-ai/catalyst/blob/master/samples/LanguageDetection/Program.cs).

## Supported languages

| Language Name      | ISO 639-1                  |
|--------------------|----------------------------|
| Afrikaans             | af        |
| Albanian              | sq        |
| Arabic                | ar        |
| Armenian              | hy        |
| Azerbaijani           | az        |
| Basque                | eu        |
| Belarusian            | be        |
| Bengali               | bn        |
| Bihari                | bh        |
| Bulgarian             | bg        |
| Catalan               | ca        |
| Cebuano               | ceb       |
| Cherokee              | chr       |
| Croatian              | hr        |
| Czech                 | cs        |
| Chinese               | zh        |
| Chinese_T             | zh-Hant   |
| Danish                | da        |
| Dhivehi               | dv        |
| Dutch                 | nl        |
| English               | en        |
| Estonian              | et        |
| Finnish               | fi        |
| French                | fr        |
| Galician              | gl        |
| Ganda                 | lg        |
| Georgian              | ka        |
| German                | de        |
| Greek                 | el        |
| Gujarati              | gu        |
| Haitian_Creole        | ht        |
| Hebrew                | iw        |
| Hindi                 | hi        |
| Hmong                 | hmn       |
| Hungarian             | hu        |
| Icelandic             | is        |
| Indonesian            | id        |
| Inuktitut             | iu        |
| Irish                 | ga        |
| Italian               | it        |
| Javanese              | jw        |
| Japanese              | ja        |
| Kannada               | kn        |
| Khmer                 | km        |
| Kinyarwanda           | rw        |
| Korean                | ko        |
| Laothian              | lo        |
| Latvian               | lv        |
| Limbu                 | lif       |
| Lithuanian            | lt        |
| Macedonian            | mk        |
| Malay                 | ms        |
| Malayalam             | ml        |
| Maltese               | mt        |
| Marathi               | mr        |
| Nepali                | ne        |
| Norwegian             | no        |
| Oriya                 | or        |
| Persian               | fa        |
| Polish                | pl        |
| Portuguese            | pt        |
| Punjabi               | pa        |
| Romanian              | ro        |
| Russian               | ru        |
| Scots_Gaelic          | gd        |
| Serbian               | sr        |
| Sinhalese             | si        |
| Slovak                | sk        |
| Slovenian             | sl        |
| Spanish               | es        |
| Swahili               | sw        |
| Swedish               | sv        |
| Syriac                | syr       |
| Tagalog               | tl        |
| Tamil                 | ta        |
| Telugu                | te        |
| Thai                  | th        |
| Turkish               | tr        |
| Ukrainian             | uk        |
| Urdu                  | ur        |
| Vietnamese            | vi        |
| Welsh                 | cy        |
| Yiddish               | yi        |

## Sources

1. [Original CLD2 Repository](https://github.com/CLD2Owners/cld2)
2. [Computation and Language](https://arxiv.org/abs/1608.08515)
3. [Language identification at Wikipedia](https://en.wikipedia.org/wiki/Language_identification)

## License

This project is licensed under the [MIT License](./LICENSE) © 2024–2025 Alexander Gluschenko.

Includes software from the following project(s):
- [CLD2](https://github.com/CLD2Owners/cld2) — © 2013 Google Inc., licensed under Apache-2.0  

See the [LICENSE](./LICENSE) file for full details.

---

We value your feedback. Feel free to open issues or contribute to the repository. Let’s make language detection in .NET even more powerful and versatile! 🌍📝

Happy coding! 👩‍💻👨‍💻

---

Stay updated by following our repository. For any inquiries or support, reach out through the [issues page](https://github.com/gluschenko/panlingo/issues).