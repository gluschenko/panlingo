# MediaPipe

Use this page after choosing `Panlingo.LanguageIdentification.MediaPipe`.

## When to Choose It

- Choose it when a bundled or custom `.tflite` model is part of the requirement.
- Use it when `WithResultCount()` and `WithScoreThreshold()` should be configured as part of detector setup.
- Prefer it only after confirming platform support for the target deployment.

## Package

```sh
dotnet add package Panlingo.LanguageIdentification.MediaPipe
```

## API Shape

- Build options with `MediaPipeOptions.FromDefault()`, `.FromFile(path)`, `.FromStream(stream)`, or `.FromData(bytes)`.
- Adjust behavior with `.WithResultCount()` and `.WithScoreThreshold()`.
- Create the detector with `new MediaPipeDetector(options)`.
- Use `PredictLanguages(text)` to get ranked predictions.
- Each prediction exposes `Language` and `Probability`.

## Integration Notes

- Gate registration or feature enablement with `MediaPipeDetector.IsSupported()`.
- Keep the model source explicit in config so the project does not silently switch between bundled and external model flows.
- Preserve raw language strings when the app needs debugging against model labels.
- Normalize output with `Panlingo.LanguageCode` when the app contract expects alpha-2, alpha-3, or IETF cleanup.
- Set score threshold in options rather than scattering post-filters through application code.

## Good Fit

- Apps that already manage `.tflite` assets.
- Deployments where the supported OS and architecture are known in advance.
- Integrations that want model-driven configuration at startup.
