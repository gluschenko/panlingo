# Panlingo.LanguageIdentification.Whatlang

Welcome to **Panlingo.LanguageIdentification.Whatlang**, a .NET wrapper for the Whatlang library, which is an efficient and easy-to-use language detection library. This package simplifies the integration of language identification capabilities into .NET applications, leveraging the Whatlang library to accurately and quickly recognize the languages of given texts. Perfect for projects that require lightweight yet reliable language detection.

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
                $"Language: {x.Prediction?.Language.ToString() ?? "NULL"}, " +
                $"Probability: {x.Prediction?.Confidence.ToString() ?? "NULL"}, " +
                $"IsReliable: {x.Prediction?.IsReliable.ToString() ?? "NULL"}, " +
                $"Script: {x.Prediction?.Script.ToString() ?? "NULL"}"
            );
        }
    }
}
```

## Supported languages

| Language    | ISO 639-3 | Enum        |
| ----------- | --------- | ----------- |
| Esperanto   | epo       | `WhatlangLanguage.Epo` |
| English     | eng       | `WhatlangLanguage.Eng` |
| Russian     | rus       | `WhatlangLanguage.Rus` |
| Mandarin    | cmn       | `WhatlangLanguage.Cmn` |
| Spanish     | spa       | `WhatlangLanguage.Spa` |
| Portuguese  | por       | `WhatlangLanguage.Por` |
| Italian     | ita       | `WhatlangLanguage.Ita` |
| Bengali     | ben       | `WhatlangLanguage.Ben` |
| French      | fra       | `WhatlangLanguage.Fra` |
| German      | deu       | `WhatlangLanguage.Deu` |
| Ukrainian   | ukr       | `WhatlangLanguage.Ukr` |
| Georgian    | kat       | `WhatlangLanguage.Kat` |
| Arabic      | ara       | `WhatlangLanguage.Ara` |
| Hindi       | hin       | `WhatlangLanguage.Hin` |
| Japanese    | jpn       | `WhatlangLanguage.Jpn` |
| Hebrew      | heb       | `WhatlangLanguage.Heb` |
| Yiddish     | yid       | `WhatlangLanguage.Yid` |
| Polish      | pol       | `WhatlangLanguage.Pol` |
| Amharic     | amh       | `WhatlangLanguage.Amh` |
| Javanese    | jav       | `WhatlangLanguage.Jav` |
| Korean      | kor       | `WhatlangLanguage.Kor` |
| Bokmal      | nob       | `WhatlangLanguage.Nob` |
| Danish      | dan       | `WhatlangLanguage.Dan` |
| Swedish     | swe       | `WhatlangLanguage.Swe` |
| Finnish     | fin       | `WhatlangLanguage.Fin` |
| Turkish     | tur       | `WhatlangLanguage.Tur` |
| Dutch       | nld       | `WhatlangLanguage.Nld` |
| Hungarian   | hun       | `WhatlangLanguage.Hun` |
| Czech       | ces       | `WhatlangLanguage.Ces` |
| Greek       | ell       | `WhatlangLanguage.Ell` |
| Bulgarian   | bul       | `WhatlangLanguage.Bul` |
| Belarusian  | bel       | `WhatlangLanguage.Bel` |
| Marathi     | mar       | `WhatlangLanguage.Mar` |
| Kannada     | kan       | `WhatlangLanguage.Kan` |
| Romanian    | ron       | `WhatlangLanguage.Ron` |
| Slovene     | slv       | `WhatlangLanguage.Slv` |
| Croatian    | hrv       | `WhatlangLanguage.Hrv` |
| Serbian     | srp       | `WhatlangLanguage.Srp` |
| Macedonian  | mkd       | `WhatlangLanguage.Mkd` |
| Lithuanian  | lit       | `WhatlangLanguage.Lit` |
| Latvian     | lav       | `WhatlangLanguage.Lav` |
| Estonian    | est       | `WhatlangLanguage.Est` |
| Tamil       | tam       | `WhatlangLanguage.Tam` |
| Vietnamese  | vie       | `WhatlangLanguage.Vie` |
| Urdu        | urd       | `WhatlangLanguage.Urd` |
| Thai        | tha       | `WhatlangLanguage.Tha` |
| Gujarati    | guj       | `WhatlangLanguage.Guj` |
| Uzbek       | uzb       | `WhatlangLanguage.Uzb` |
| Punjabi     | pan       | `WhatlangLanguage.Pan` |
| Azerbaijani | aze       | `WhatlangLanguage.Aze` |
| Indonesian  | ind       | `WhatlangLanguage.Ind` |
| Telugu      | tel       | `WhatlangLanguage.Tel` |
| Persian     | pes       | `WhatlangLanguage.Pes` |
| Malayalam   | mal       | `WhatlangLanguage.Mal` |
| Oriya       | ori       | `WhatlangLanguage.Ori` |
| Burmese     | mya       | `WhatlangLanguage.Mya` |
| Nepali      | nep       | `WhatlangLanguage.Nep` |
| Sinhalese   | sin       | `WhatlangLanguage.Sin` |
| Khmer       | khm       | `WhatlangLanguage.Khm` |
| Turkmen     | tuk       | `WhatlangLanguage.Tuk` |
| Akan        | aka       | `WhatlangLanguage.Aka` |
| Zulu        | zul       | `WhatlangLanguage.Zul` |
| Shona       | sna       | `WhatlangLanguage.Sna` |
| Afrikaans   | afr       | `WhatlangLanguage.Afr` |
| Latin       | lat       | `WhatlangLanguage.Lat` |
| Slovak      | slk       | `WhatlangLanguage.Slk` |
| Catalan     | cat       | `WhatlangLanguage.Cat` |
| Tagalog     | tgl       | `WhatlangLanguage.Tgl` |
| Armenian    | hye       | `WhatlangLanguage.Hye` |

---

We value your feedback. Feel free to open issues or contribute to the repository. Let’s make language detection in .NET even more powerful and versatile! 🌍📝

Happy coding! 👩‍💻👨‍💻

---

Stay updated by following our repository. For any inquiries or support, reach out through the [issues page](https://github.com/gluschenko/language-identification/issues).