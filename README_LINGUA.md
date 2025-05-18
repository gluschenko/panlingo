# Panlingo.LanguageIdentification.Lingua

Welcome to **Panlingo.LanguageIdentification.Lingua**, a .NET wrapper for the Lingua library, which is an efficient and easy-to-use language detection library. This package simplifies the integration of language identification capabilities into .NET applications, leveraging the Lingua library to accurately and quickly recognize the languages of given texts. Perfect for projects that require lightweight yet reliable language detection.

## Requirements

- Runtime: **.NET >= 5.0**
- OS: **Linux (Ubuntu, Debian)**, **Windows 10+** or **Windows Server 2019+**, **macOS**
- Arch: **AMD64** (or **ARM** for macOS)

## Installation

To integrate the Lingua functionality, follow these steps:

1. **Install the NuGet package**:

   ```sh
   dotnet add package Panlingo.LanguageIdentification.Lingua
   ```

## Usage

Integrating the Lingua library into your .NET application is straightforward. Here’s a quick guide to get you started:

1. **Install the Package**: Ensure you have added the `Panlingo.LanguageIdentification.Lingua` package to your project using the provided installation command.
2. **Initialize the Library**: Follow the example snippet to initialize and use the Lingua library for detecting languages.

```csharp
using Panlingo.LanguageIdentification.Lingua;

class Program
{
    static void Main()
    {
        // Create an instance of the language detector
        using var linguaBuilder = new LinguaDetectorBuilder(Enum.GetValues<LinguaLanguage>())
            .WithPreloadedLanguageModels()     // optional
            .WithMinimumRelativeDistance(0.95) // optional
            .WithLowAccuracyMode();            // optional

        using var lingua = linguaBuilder.Build();

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
                Predictions = lingua.PredictLanguages(x),
            })
            .ToArray();

        foreach (var x in predictions)
        {
            var prediction = x.Predictions.FirstOrDefault();

            Console.WriteLine(
                $"Text: {x.Text}, " +
                $"Language: {prediction?.Language.ToString() ?? "NULL"}, " +
                $"Probability: {prediction?.Confidence.ToString() ?? "NULL"}"
            );
        }
    }
}
```

## Alternatives

If you are exploring other options, here are some alternatives to consider:

- **[searchpioneer/lingua-dotnet](https://github.com/searchpioneer/lingua-dotnet)**: An alternative port for .NET.

## Sources

1. [Original Lingua Repository](https://github.com/pemistahl/lingua-rs)
2. [Language identification at Wikipedia](https://en.wikipedia.org/wiki/Language_identification)

## Supported languages

| Language    | ISO 639-3 | Enum        |
| ----------- | --------- | ----------- |
| Afrikaans     | afr | `LinguaLanguage.Afrikaans`   |
| Albanian      | sqi | `LinguaLanguage.Albanian`    |
| Arabic        | ara | `LinguaLanguage.Arabic`      |
| Armenian      | hye | `LinguaLanguage.Armenian`    |
| Azerbaijani   | aze | `LinguaLanguage.Azerbaijani` |
| Basque        | eus | `LinguaLanguage.Basque`      |
| Belarusian    | bel | `LinguaLanguage.Belarusian`  |
| Bengali       | ben | `LinguaLanguage.Bengali`     |
| Bokmal        | nob | `LinguaLanguage.Bokmal`      |
| Bosnian       | bos | `LinguaLanguage.Bosnian`     |
| Bulgarian     | bul | `LinguaLanguage.Bulgarian`   |
| Catalan       | cat | `LinguaLanguage.Catalan`     |
| Chinese       | zho | `LinguaLanguage.Chinese`     |
| Croatian      | hrv | `LinguaLanguage.Croatian`    |
| Czech         | ces | `LinguaLanguage.Czech`       |
| Danish        | dan | `LinguaLanguage.Danish`      |
| Dutch         | nld | `LinguaLanguage.Dutch`       |
| English       | eng | `LinguaLanguage.English`     |
| Esperanto     | epo | `LinguaLanguage.Esperanto`   |
| Estonian      | est | `LinguaLanguage.Estonian`    |
| Finnish       | fin | `LinguaLanguage.Finnish`     |
| French        | fra | `LinguaLanguage.French`      |
| Ganda         | lug | `LinguaLanguage.Ganda`       |
| Georgian      | kat | `LinguaLanguage.Georgian`    |
| German        | deu | `LinguaLanguage.German`      |
| Greek         | ell | `LinguaLanguage.Greek`       |
| Gujarati      | guj | `LinguaLanguage.Gujarati`    |
| Hebrew        | heb | `LinguaLanguage.Hebrew`      |
| Hindi         | hin | `LinguaLanguage.Hindi`       |
| Hungarian     | hun | `LinguaLanguage.Hungarian`   |
| Icelandic     | isl | `LinguaLanguage.Icelandic`   |
| Indonesian    | ind | `LinguaLanguage.Indonesian`  |
| Irish         | gle | `LinguaLanguage.Irish`       |
| Italian       | ita | `LinguaLanguage.Italian`     |
| Japanese      | jpn | `LinguaLanguage.Japanese`    |
| Kazakh        | kaz | `LinguaLanguage.Kazakh`      |
| Korean        | kor | `LinguaLanguage.Korean`      |
| Latin         | lat | `LinguaLanguage.Latin`       |
| Latvian       | lav | `LinguaLanguage.Latvian`     |
| Lithuanian    | lit | `LinguaLanguage.Lithuanian`  |
| Macedonian    | mkd | `LinguaLanguage.Macedonian`  |
| Malay         | msa | `LinguaLanguage.Malay`       |
| Maori         | mri | `LinguaLanguage.Maori`       |
| Marathi       | mar | `LinguaLanguage.Marathi`     |
| Mongolian     | mon | `LinguaLanguage.Mongolian`   |
| Nynorsk       | nno | `LinguaLanguage.Nynorsk`     |
| Persian       | fas | `LinguaLanguage.Persian`     |
| Polish        | pol | `LinguaLanguage.Polish`      |
| Portuguese    | por | `LinguaLanguage.Portuguese`  |
| Punjabi       | pan | `LinguaLanguage.Punjabi`     |
| Romanian      | ron | `LinguaLanguage.Romanian`    |
| Russian       | rus | `LinguaLanguage.Russian`     |
| Serbian       | srp | `LinguaLanguage.Serbian`     |
| Shona         | sna | `LinguaLanguage.Shona`       |
| Slovak        | slk | `LinguaLanguage.Slovak`      |
| Slovene       | slv | `LinguaLanguage.Slovene`     |
| Somali        | som | `LinguaLanguage.Somali`      |
| Sotho         | sot | `LinguaLanguage.Sotho`       |
| Spanish       | spa | `LinguaLanguage.Spanish`     |
| Swahili       | swa | `LinguaLanguage.Swahili`     |
| Swedish       | swe | `LinguaLanguage.Swedish`     |
| Tagalog       | tgl | `LinguaLanguage.Tagalog`     |
| Tamil         | tam | `LinguaLanguage.Tamil`       |
| Telugu        | tel | `LinguaLanguage.Telugu`      |
| Thai          | tha | `LinguaLanguage.Thai`        |
| Tsonga        | tso | `LinguaLanguage.Tsonga`      |
| Tswana        | tsn | `LinguaLanguage.Tswana`      |
| Turkish       | tur | `LinguaLanguage.Turkish`     |
| Ukrainian     | ukr | `LinguaLanguage.Ukrainian`   |
| Urdu          | urd | `LinguaLanguage.Urdu`        |
| Vietnamese    | vie | `LinguaLanguage.Vietnamese`  |
| Welsh         | cym | `LinguaLanguage.Welsh`       |
| Xhosa         | xho | `LinguaLanguage.Xhosa`       |
| Yoruba        | yor | `LinguaLanguage.Yoruba`      |
| Zulu          | zul | `LinguaLanguage.Zulu`        |

## License

This project is licensed under the [MIT License](./LICENSE) © 2024–2025 Alexander Gluschenko.

Includes software from the following project(s):
- [Lingua](https://github.com/pemistahl/lingua-rs) — © 2020–2023 Peter M. Stahl, licensed under Apache-2.0  

See the [LICENSE](./LICENSE) file for full details.

---

We value your feedback. Feel free to open issues or contribute to the repository. Let’s make language detection in .NET even more powerful and versatile! 🌍📝

Happy coding! 👩‍💻👨‍💻

---

Stay updated by following our repository. For any inquiries or support, reach out through the [issues page](https://github.com/gluschenko/panlingo/issues).