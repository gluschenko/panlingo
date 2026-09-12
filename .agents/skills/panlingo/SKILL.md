---
name: panlingo
description: Integrate Panlingo libraries into real .NET projects. Use when Codex needs to add, refactor, or review language detection, mixed-language detection, script detection, model loading, or language-code normalization with Panlingo packages such as Panlingo.LanguageIdentification.CLD2, CLD3, FastText, Whatlang, MediaPipe, Lingua, or Panlingo.LanguageCode.
---

# Panlingo

## Overview

Use this skill to choose the right Panlingo package, integrate it into application code, and normalize detector output into the project's language-code contract.

Prefer small, production-oriented integrations over demo code. Keep the detector choice explicit, preserve raw model output when useful, and normalize only at the boundary where the application needs a stable contract.

## Workflow

1. Clarify what the project actually needs:
   single-language classification, top-N candidates, mixed-language ranges, script detection, bundled model, custom model, or strict platform support.
2. Choose the package before writing code.
   Read [references/package-selection.md](references/package-selection.md) when detector choice is still open.
3. Implement the smallest viable integration.
   Favor one adapter/service that hides detector-specific output from the rest of the app.
4. Normalize output codes only after deciding the target format.
   Read [references/integration-patterns.md](references/integration-patterns.md) when you need code patterns.
5. Add validation around the project's real texts, unsupported platforms, and malformed input.

## Default Approach

If the user has not already picked a detector, start with this decision order:

- Choose `Panlingo.LanguageIdentification.CLD3` for a compact general-purpose default when the project needs probability, reliability, and top-N predictions.
- Choose `Panlingo.LanguageIdentification.FastText` when the project needs custom or external models, or when model choice is part of the requirement.
- Choose `Panlingo.LanguageIdentification.Whatlang` when script detection is a first-class requirement and single-language prediction is enough.
- Choose `Panlingo.LanguageIdentification.Lingua` when short texts or mixed-language spans matter more than broad platform coverage.
- Choose `Panlingo.LanguageIdentification.MediaPipe` when a bundled or custom `.tflite` model is desired and the deployment platform is supported.
- Choose `Panlingo.LanguageIdentification.CLD2` only when its behavior or output shape is explicitly desired.
- Add `Panlingo.LanguageCode` whenever the app needs one canonical language-code format across multiple detectors or upstream systems.

After the package is chosen, read its dedicated page instead of relying on the short summary in this file:

- [references/cld2.md](references/cld2.md)
- [references/cld3.md](references/cld3.md)
- [references/fasttext.md](references/fasttext.md)
- [references/whatlang.md](references/whatlang.md)
- [references/mediapipe.md](references/mediapipe.md)
- [references/lingua.md](references/lingua.md)
- [references/language-code.md](references/language-code.md)

## Integration Rules

- Always dispose detector instances with `using` or an explicit lifetime wrapper. These packages hold unmanaged resources.
- Call `IsSupported()` before wiring optional features on cross-platform apps.
- Treat detector instances as not thread-safe by default unless the project already has proof otherwise. This is a safe inference from the source shape: wrappers hold native handles and do not publish a general concurrency contract.
- Keep raw detector output if it carries useful detail such as probability, reliability, script, or original label.
- Normalize into project-facing DTOs instead of leaking package-specific types through the app.
- For web apps and workers, prefer a small adapter interface plus one implementation per chosen detector.
- When integrating `FastText` or `MediaPipe`, decide whether the project should use bundled models or provide files explicitly. Do not mix both paths accidentally.

## Output Normalization

- `CLD2`, `CLD3`, and `MediaPipe` return language strings directly, but those strings may still need normalization for the app contract.
- `FastText` returns labels such as `__label__en` or `__label__eng_Latn`; strip the `__label__` prefix before further normalization.
- `Whatlang` and `Lingua` return enums; convert them to codes with their helper methods before mapping them into the app contract.
- Use `Panlingo.LanguageCode` when the project needs alpha-2, alpha-3, English names, IETF cleanup, deprecated-code conversion, or macrolanguage reduction.
- Normalize once, close to the application boundary, and keep tests for the exact contract you expose.

## Validation

- Add tests with short real phrases from the product domain, not only textbook examples.
- Add at least one malformed or noisy input case when the app will process user-generated text.
- Add one platform-gating test or fallback path when the chosen detector is not supported everywhere the app ships.
- If detector choice is still uncertain, implement the adapter first so swapping packages is cheap.

## References

- Read [references/package-selection.md](references/package-selection.md) for detector capabilities, platform tradeoffs, and package-selection heuristics.
- Read [references/integration-patterns.md](references/integration-patterns.md) for production code patterns, normalization snippets, and testing guidance.
- Read the model-specific pages once the package is chosen:
  [references/cld2.md](references/cld2.md),
  [references/cld3.md](references/cld3.md),
  [references/fasttext.md](references/fasttext.md),
  [references/whatlang.md](references/whatlang.md),
  [references/mediapipe.md](references/mediapipe.md),
  [references/lingua.md](references/lingua.md),
  [references/language-code.md](references/language-code.md).
