# CLD3

Use this page after choosing `Panlingo.LanguageIdentification.CLD3`.

## When to Choose It

- Prefer it as the compact general-purpose default.
- Use it when the app needs one best guess plus top-N candidates.
- Use it when `Probability`, `IsReliable`, and `Proportion` should stay available to business logic or telemetry.

## Package

```sh
dotnet add package Panlingo.LanguageIdentification.CLD3
```

## API Shape

- Create the detector with `new CLD3Detector(minNumBytes, maxNumBytes)`.
- Use `PredictLanguage(text)` for the main result.
- Use `PredictLanguages(text, count)` for ranked candidates.
- Each prediction exposes `Language`, `Probability`, `IsReliable`, and `Proportion`.
- Treat `und` as unknown when the app needs an explicit fallback.

## Integration Notes

- Keep `minNumBytes` and `maxNumBytes` explicit in code or options; do not hide them as unexplained constants.
- Preserve raw CLD3 fields in logs or diagnostics if confidence handling matters.
- Normalize `Language` close to the app boundary with `Panlingo.LanguageCode` when the project needs a stable contract.
- Use an adapter layer so the rest of the app does not depend directly on CLD3-specific types.

## Good Fit

- APIs that return best-guess language plus confidence.
- Services that need a native-backed detector without model-file management.
- Cross-platform apps that want broader coverage than `MediaPipe` or `Lingua`.
