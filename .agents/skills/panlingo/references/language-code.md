# LanguageCode

Use this page after choosing `Panlingo.LanguageCode` or when a detector's raw output must be normalized.

## When to Choose It

- Choose it whenever the app needs one canonical language-code format.
- Use it when the project mixes detector output, locale tags, enums, legacy codes, or external API payloads.
- Add it early if the contract must expose alpha-2, alpha-3, or English names consistently.

## Package

```sh
dotnet add package Panlingo.LanguageCode
```

## API Shape

- Build a normalization pipeline with `LanguageCodeResolver`.
- Resolve or convert through `LanguageCodeHelper`.
- Common resolver steps include:
  `ToLowerAndTrim()`,
  `ConvertFromIETF()`,
  `ConvertFromDeprecatedCode()`,
  `ReduceToMacrolanguage()`,
  and `Select(LanguageCodeEntity.Alpha2|Alpha3|EnglishName)`.

## Integration Notes

- Normalize once at the app boundary instead of repeatedly in every feature.
- Keep the project's canonical target explicit: alpha-2, alpha-3, English name, or another agreed representation.
- Use `ResolveUnknownCode()` only when the app genuinely owns the custom fallback behavior.
- Add tests for every detector-specific code shape the project accepts, such as stripped FastText labels or IETF tags like `en-US`.

## Good Fit

- Any app that exposes one stable language-code contract.
- Systems that aggregate outputs from several Panlingo detectors.
- Migrations away from legacy or inconsistent language-code handling.
