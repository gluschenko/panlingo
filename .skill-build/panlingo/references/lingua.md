# Lingua

Use this page after choosing `Panlingo.LanguageIdentification.Lingua`.

## When to Choose It

- Choose it when short-text quality matters.
- Use it when the project needs mixed-language spans inside one input.
- Prefer it when the product domain already constrains the language set and that constraint can be encoded in the builder.

## Package

```sh
dotnet add package Panlingo.LanguageIdentification.Lingua
```

## API Shape

- Build through `new LinguaDetectorBuilder(languages)` or `FromLanguages(languages)`.
- Optional builder methods include `WithPreloadedLanguageModels()`, `WithMinimumRelativeDistance()`, and `WithLowAccuracyMode()`.
- Call `Build()` to create `LinguaDetector`.
- Use `PredictLanguages(text, count)` for ranked candidates.
- Use `PredictMixedLanguages(text)` for language ranges within a string.
- Convert enum output with `GetLanguageCode(language, codeType)`.

## Integration Notes

- Narrow the language list whenever the business domain allows it.
- Handle platform support early because `Lingua` is less portable than `CLD3` or `FastText`.
- Preserve range-level output when the feature needs multilingual span analysis; do not collapse it too early into one label.
- Convert enum results to string codes before exposing them from your app contract.

## Good Fit

- Chat, moderation, or search flows with short noisy text.
- Paragraph-level multilingual analysis.
- Products where likely languages are known in advance and can be constrained.
