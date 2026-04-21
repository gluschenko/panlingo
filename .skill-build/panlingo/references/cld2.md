# CLD2

Use this page after choosing `Panlingo.LanguageIdentification.CLD2`.

## When to Choose It

- Choose it only when the project explicitly wants CLD2 behavior or its output shape.
- Use it when downstream logic benefits from the extra `Script` field on predictions.
- Treat it as a deliberate compatibility choice, not the default first pick.

## Package

```sh
dotnet add package Panlingo.LanguageIdentification.CLD2
```

## API Shape

- Create the detector with `new CLD2Detector()`.
- Call `PredictLanguage(text)`.
- Expect multiple predictions ordered by probability after the wrapper maps native output.
- Each prediction exposes `Language`, `Script`, `Probability`, `IsReliable`, and `Proportion`.

## Integration Notes

- Dispose with `using` even though the public `Dispose()` is lightweight.
- Normalize `Language` only after deciding whether the project wants raw CLD2 codes or one canonical format.
- If the project exposes script downstream, preserve `Script` in the DTO instead of discarding it.
- Use `Panlingo.LanguageCode` if the app contract requires alpha-2, alpha-3, English names, or deprecated-code cleanup.

## Good Fit

- Legacy systems already validated against CLD2 output.
- Pipelines where script is part of routing or analytics.
- Cases where behavior parity with older CLD-family integrations matters.
