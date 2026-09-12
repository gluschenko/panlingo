# Whatlang

Use this page after choosing `Panlingo.LanguageIdentification.Whatlang`.

## When to Choose It

- Choose it when script detection is a first-class requirement.
- Use it when single-language detection is enough.
- Prefer it when downstream routing depends on both language and writing system.

## Package

```sh
dotnet add package Panlingo.LanguageIdentification.Whatlang
```

## README Example

```csharp
using Panlingo.LanguageIdentification.Whatlang;

class Program
{
    static void Main()
    {
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

## API Shape

- Create the detector with `new WhatlangDetector()`.
- Use `PredictLanguage(text)` for the main prediction.
- Use `PredictScript(text)` when only the script is needed.
- Convert enum output with `GetLanguageCode()`, `GetLanguageName()`, `GetLanguageEnglishName()`, or `GetScriptName()`.

## Integration Notes

- Handle the nullable result from `PredictLanguage(text)` instead of assuming every input resolves cleanly.
- Convert the enum to a string code before storing or returning it from the app.
- Preserve the script value if the product uses transliteration, moderation rules, or search partitioning by script.
- Add `Panlingo.LanguageCode` only when the app needs further conversion after `GetLanguageCode()`.

## Good Fit

- Moderation or routing systems that distinguish language and script.
- UX flows that need both detected language and detected writing system.
- Lightweight integrations where one best guess is enough.
