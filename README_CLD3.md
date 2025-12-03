# Panlingo.LanguageIdentification.CLD3

Welcome to **Panlingo.LanguageIdentification.CLD3**, a .NET wrapper for the Compact Language Detector (CLD3) library by Google Inc. This package seamlessly integrates language identification capabilities into .NET applications, enabling accurate and efficient recognition of over 107 languages with minimal effort. Perfect for applications dealing with multilingual texts or requiring automatic language detection.

## Requirements

- Runtime: **.NET >= 5.0**
- OS: **Linux (Ubuntu, Debian)**, **Windows 10+** or **Windows Server 2019+**
- Arch: **AMD64** (or **ARM** for macOS and Linux)

## Installation

To integrate the CLD3 functionality, follow these steps:

**Install the NuGet package**:

```sh
dotnet add package Panlingo.LanguageIdentification.CLD3
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
        using var cld3 = new CLD3Detector(minNumBytes: 0, maxNumBytes: 512);

        var text = "Hello, how are you? Привіт, як справи? Привет, как дела?";

        var singlePrediction = cld3.PredictLanguage("Привіт, як справи?");

        Console.WriteLine($"Language: {singlePrediction.Language}");
        Console.WriteLine($"Probability: {singlePrediction.Probability}");
        Console.WriteLine($"IsReliable: {singlePrediction.IsReliable}");
        Console.WriteLine($"Proportion: {singlePrediction.Proportion}");

        var predictions = cld3.PredictLanguages("Hello, how are you? Привіт, як справи? Привет, как дела?", 3);

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

- **[NikulovE/cld3.net](https://github.com/NikulovE/cld3.net)**: An unmaintained wrapper for .NET.
- **[uranium62/cld3-net](https://github.com/uranium62/cld3-net)**: Another unmaintained wrapper for .NET.

## Sources

1. [Original CLD3 Repository](https://github.com/google/cld3)
2. [Evaluation of Off-the-Shelf Language Identification Tools on Bulgarian Social Media Posts](https://aclanthology.org/2022.clib-1.18.pdf)
3. [Language identification at Wikipedia](https://en.wikipedia.org/wiki/Language_identification)

## Supported languages

Output Code | Language Name   | Script Name
----------- | --------------- | ------------------------------------------
af          | Afrikaans       | Latin
am          | Amharic         | Ethiopic
ar          | Arabic          | Arabic
bg          | Bulgarian       | Cyrillic
bg-Latn     | Bulgarian       | Latin
bn          | Bangla          | Bangla
bs          | Bosnian         | Latin
ca          | Catalan         | Latin
ceb         | Cebuano         | Latin
co          | Corsican        | Latin
cs          | Czech           | Latin
cy          | Welsh           | Latin
da          | Danish          | Latin
de          | German          | Latin
el          | Greek           | Greek
el-Latn     | Greek           | Latin
en          | English         | Latin
eo          | Esperanto       | Latin
es          | Spanish         | Latin
et          | Estonian        | Latin
eu          | Basque          | Latin
fa          | Persian         | Arabic
fi          | Finnish         | Latin
fil         | Filipino        | Latin
fr          | French          | Latin
fy          | Western Frisian | Latin
ga          | Irish           | Latin
gd          | Scottish Gaelic | Latin
gl          | Galician        | Latin
gu          | Gujarati        | Gujarati
ha          | Hausa           | Latin
haw         | Hawaiian        | Latin
hi          | Hindi           | Devanagari
hi-Latn     | Hindi           | Latin
hmn         | Hmong           | Latin
hr          | Croatian        | Latin
ht          | Haitian Creole  | Latin
hu          | Hungarian       | Latin
hy          | Armenian        | Armenian
id          | Indonesian      | Latin
ig          | Igbo            | Latin
is          | Icelandic       | Latin
it          | Italian         | Latin
iw          | Hebrew          | Hebrew
ja          | Japanese        | Japanese
ja-Latn     | Japanese        | Latin
jv          | Javanese        | Latin
ka          | Georgian        | Georgian
kk          | Kazakh          | Cyrillic
km          | Khmer           | Khmer
kn          | Kannada         | Kannada
ko          | Korean          | Korean
ku          | Kurdish         | Latin
ky          | Kyrgyz          | Cyrillic
la          | Latin           | Latin
lb          | Luxembourgish   | Latin
lo          | Lao             | Lao
lt          | Lithuanian      | Latin
lv          | Latvian         | Latin
mg          | Malagasy        | Latin
mi          | Maori           | Latin
mk          | Macedonian      | Cyrillic
ml          | Malayalam       | Malayalam
mn          | Mongolian       | Cyrillic
mr          | Marathi         | Devanagari
ms          | Malay           | Latin
mt          | Maltese         | Latin
my          | Burmese         | Myanmar
ne          | Nepali          | Devanagari
nl          | Dutch           | Latin
no          | Norwegian       | Latin
ny          | Nyanja          | Latin
pa          | Punjabi         | Gurmukhi
pl          | Polish          | Latin
ps          | Pashto          | Arabic
pt          | Portuguese      | Latin
ro          | Romanian        | Latin
ru          | Russian         | Cyrillic
ru-Latn     | Russian         | English
sd          | Sindhi          | Arabic
si          | Sinhala         | Sinhala
sk          | Slovak          | Latin
sl          | Slovenian       | Latin
sm          | Samoan          | Latin
sn          | Shona           | Latin
so          | Somali          | Latin
sq          | Albanian        | Latin
sr          | Serbian         | Cyrillic
st          | Southern Sotho  | Latin
su          | Sundanese       | Latin
sv          | Swedish         | Latin
sw          | Swahili         | Latin
ta          | Tamil           | Tamil
te          | Telugu          | Telugu
tg          | Tajik           | Cyrillic
th          | Thai            | Thai
tr          | Turkish         | Latin
uk          | Ukrainian       | Cyrillic
ur          | Urdu            | Arabic
uz          | Uzbek           | Latin
vi          | Vietnamese      | Latin
xh          | Xhosa           | Latin
yi          | Yiddish         | Hebrew
yo          | Yoruba          | Latin
zh          | Chinese         | Han (including Simplified and Traditional)
zh-Latn     | Chinese         | Latin
zu          | Zulu            | Latin

## License

This project is licensed under the [MIT License](./LICENSE) © 2024–2025 Alexander Gluschenko.

Includes software from the following project(s):
- [CLD3](https://github.com/google/cld3) — © 2016 Google Inc., licensed under Apache-2.0  

See the [LICENSE](./LICENSE) file for full details.

---

We value your feedback. Feel free to open issues or contribute to the repository. Let’s make language detection in .NET even more powerful and versatile! 🌍📝

Happy coding! 👩‍💻👨‍💻

---

Stay updated by following our repository. For any inquiries or support, reach out through the [issues page](https://github.com/gluschenko/panlingo/issues).