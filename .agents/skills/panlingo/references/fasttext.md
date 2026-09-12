# FastText

Use this page after choosing `Panlingo.LanguageIdentification.FastText`.

## When to Choose It

- Choose it when model choice is part of the requirement.
- Use it when the project needs bundled, file-based, or stream-based model loading.
- Prefer it when the user asks for `lid.176`, `lid218e`, or another FastText-compatible classifier.

## Package

```sh
dotnet add package Panlingo.LanguageIdentification.FastText
```

## API Shape

- Create the detector with `new FastTextDetector()`.
- Load a model with `LoadDefaultModel()`, `LoadModel(path)`, or `LoadModel(stream)`.
- Use `Predict(text, count, threshold)` for ranked predictions.
- Labels come back as strings like `__label__en` or `__label__eng_Latn`.

## Integration Notes

- Strip the `__label__` prefix before exposing results to the rest of the app.
- Keep model loading out of hot paths; configure the model source once and reuse the integration path.
- If the app supports both bundled and custom models, make the selection explicit in configuration.
- Normalize the stripped code with `Panlingo.LanguageCode` only if the app requires a canonical format.
- Keep raw labels in logs when model-specific debugging matters.

## Good Fit

- Systems with versioned models outside the codebase.
- Projects that may swap between bundled and custom models later.
- Integrations where wider model choice matters more than one fixed detector API.
