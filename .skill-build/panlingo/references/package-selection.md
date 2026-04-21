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

## Open the Detailed Page

Once the package is chosen, switch to the dedicated reference page:

- `CLD2`: [cld2.md](cld2.md)
- `CLD3`: [cld3.md](cld3.md)
- `FastText`: [fasttext.md](fasttext.md)
- `Whatlang`: [whatlang.md](whatlang.md)
- `MediaPipe`: [mediapipe.md](mediapipe.md)
- `Lingua`: [lingua.md](lingua.md)
- `LanguageCode`: [language-code.md](language-code.md)

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
