# Package Selection

Use this file when the user has not already committed to one Panlingo package.

## Quick Matrix

| Need | Prefer |
| --- | --- |
| Compact general-purpose detector with probabilities and top-N results | `Panlingo.LanguageIdentification.CLD3` |
| Custom or external model files | `Panlingo.LanguageIdentification.FastText` or `Panlingo.LanguageIdentification.MediaPipe` |
| Script detection as part of the API | `Panlingo.LanguageIdentification.Whatlang` |
| Mixed-language ranges inside one string | `Panlingo.LanguageIdentification.Lingua` |
| Default bundled `.tflite` model or custom `.tflite` file | `Panlingo.LanguageIdentification.MediaPipe` |
| Older CLD-family behavior plus script field in predictions | `Panlingo.LanguageIdentification.CLD2` |
| One canonical language-code pipeline across detectors | `Panlingo.LanguageCode` |

## Package Notes

### Panlingo.LanguageIdentification.CLD3

- Good default when the app needs `PredictLanguage()` and `PredictLanguages(text, count)`.
- Returns `Language`, `Probability`, `IsReliable`, and `Proportion`.
- Constructor requires byte-window parameters, so keep those explicit instead of hiding them as magic constants.
- Use when you want a simple native-backed detector without model-file management.

### Panlingo.LanguageIdentification.FastText

- Use when model choice matters.
- Supports `LoadDefaultModel()`, `LoadModel(path)`, and `LoadModel(stream)`.
- Returns `Label` values such as `__label__en`; normalize them before exposing them to the rest of the app.
- Best choice when the user asks for a specific FastText model like `lid.176`, `lid218e`, or a custom classifier.

### Panlingo.LanguageIdentification.Whatlang

- Use for single-language detection with script output.
- `PredictLanguage()` returns a nullable prediction and `PredictScript()` is available separately.
- Convert enum output with `GetLanguageCode()`, `GetLanguageName()`, or `GetLanguageEnglishName()` before building DTOs.

### Panlingo.LanguageIdentification.Lingua

- Use when short-text quality or mixed-language ranges are important.
- Build through `LinguaDetectorBuilder`, then call `Build()`.
- `PredictLanguages()` gives ranked candidates; `PredictMixedLanguages()` gives spans.
- Platform support is narrower than CLD3 or FastText, so check deployment targets early.

### Panlingo.LanguageIdentification.MediaPipe

- Use when a `.tflite` model is part of the requirement.
- Build options through `MediaPipeOptions.FromDefault()`, `.FromFile()`, `.FromStream()`, or `.FromData()`.
- Tune `WithResultCount()` and `WithScoreThreshold()` instead of post-filtering everything manually.
- Platform support is selective, so gate registration with `MediaPipeDetector.IsSupported()`.

### Panlingo.LanguageIdentification.CLD2

- Use only when the project explicitly wants CLD2 behavior or its output shape.
- `PredictLanguage()` returns multiple predictions, each with `Language`, `Script`, `Probability`, `IsReliable`, and `Proportion`.
- Treat it as a compatibility or deliberate-choice option, not the automatic default.

### Panlingo.LanguageCode

- Use alongside a detector when the app contract requires normalized language codes.
- Prefer it whenever the project mixes detector outputs, user input, locale tags, or legacy codes.
- `LanguageCodeResolver` is the main pipeline builder; `LanguageCodeHelper` performs resolution and lookup.

## Platform Heuristics

- Prefer `CLD3`, `FastText`, or `Whatlang` when the app must cover Windows, Linux, and macOS across both `x64` and `arm64`.
- Be more cautious with `MediaPipe` and `Lingua`; verify the target OS and architecture before choosing them.
- If cross-platform parity is a release requirement, check support before writing business logic around a detector-specific feature.

## Real-Project Defaults

- For a REST API that just needs best-guess language plus confidence: start with `CLD3`.
- For an ML-heavy system where models are swapped or versioned outside the app: start with `FastText`.
- For moderation, search, or routing systems that care about script: start with `Whatlang`, optionally combined with `LanguageCode`.
- For paragraph-level multilingual analysis: start with `Lingua`.
- For a project that must speak in one code format like ISO 639-1 or ISO 639-3: add `LanguageCode` from the beginning.
