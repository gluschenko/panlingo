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
