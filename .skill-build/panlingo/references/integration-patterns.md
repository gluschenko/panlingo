# Integration Patterns

Use this file when writing production code with a chosen Panlingo package.

## Common Adapter Pattern

Hide detector-specific output behind a small DTO and one adapter.

```csharp
public sealed record LanguageDetectionResult(
    string RawCode,
    string NormalizedCode,
    double Score,
    string? Script = null,
    bool? IsReliable = null
);
```

Prefer one adapter per detector instead of sprinkling package-specific calls across controllers, handlers, and jobs.

## CLD3 Pattern

```csharp
using Panlingo.LanguageIdentification.CLD3;
using Panlingo.LanguageCode;
using Panlingo.LanguageCode.Models;

using var detector = new CLD3Detector(minNumBytes: 0, maxNumBytes: 512);

var prediction = detector.PredictLanguage(text);

var resolver = new LanguageCodeResolver()
    .ToLowerAndTrim()
    .ConvertFromIETF()
    .ConvertFromDeprecatedCode()
    .Select(LanguageCodeEntity.Alpha2);

var normalizedCode = LanguageCodeHelper.Resolve(prediction.Language, resolver);
```

Use this pattern when the app wants CLD3's raw reliability fields but still needs a normalized alpha-2 or alpha-3 code.

## FastText Pattern

```csharp
using Panlingo.LanguageIdentification.FastText;

using var detector = new FastTextDetector();
detector.LoadDefaultModel();

var prediction = detector.Predict(text, count: 3).First();
var rawCode = prediction.Label.Replace("__label__", "", StringComparison.Ordinal);
```

If the project supplies its own model, use `LoadModel(path)` or `LoadModel(stream)` and keep model location/configuration outside business logic.

## MediaPipe Pattern

```csharp
using Panlingo.LanguageIdentification.MediaPipe;

if (!MediaPipeDetector.IsSupported())
{
    throw new PlatformNotSupportedException();
}

using var detector = new MediaPipeDetector(
    MediaPipeOptions.FromDefault()
        .WithResultCount(3)
        .WithScoreThreshold(0.2f)
);

var predictions = detector.PredictLanguages(text);
```

Prefer setting score threshold and result count in `MediaPipeOptions` instead of filtering after the fact.

## Whatlang Pattern

```csharp
using Panlingo.LanguageIdentification.Whatlang;

using var detector = new WhatlangDetector();
var prediction = detector.PredictLanguage(text);

if (prediction is null)
{
    return null;
}

var code = detector.GetLanguageCode(prediction.Language);
var script = prediction.Script.ToString();
```

Use this when script matters for downstream routing, transliteration rules, or UI behavior.

## Lingua Pattern

```csharp
using Panlingo.LanguageIdentification.Lingua;

using var builder = new LinguaDetectorBuilder(new[]
{
    LinguaLanguage.English,
    LinguaLanguage.Ukrainian,
    LinguaLanguage.Russian,
})
    .WithMinimumRelativeDistance(0.95);

using var detector = builder.Build();

var predictions = detector.PredictLanguages(text);
var mixed = detector.PredictMixedLanguages(text);
```

Prefer narrowing the language set when the product domain already constrains possible languages.

## LanguageCode Pattern

Use `Panlingo.LanguageCode` to keep detector output and app-facing codes aligned.

```csharp
using Panlingo.LanguageCode;
using Panlingo.LanguageCode.Models;

var resolver = new LanguageCodeResolver()
    .ToLowerAndTrim()
    .ConvertFromIETF()
    .ConvertFromDeprecatedCode()
    .ReduceToMacrolanguage()
    .Select(LanguageCodeEntity.Alpha3);

var code = LanguageCodeHelper.Resolve(inputCode, resolver);
```

Useful cases:

- Convert `en-US` to `en`.
- Convert deprecated values like `iw`.
- Reduce dialect or script-specific output to the project's canonical macrolanguage.
- Convert enum-derived or detector-derived codes into alpha-2, alpha-3, or English names.

## Lifetime Guidance

- Dispose detectors promptly.
- Prefer factory-created or scoped detector instances in request-driven apps unless the codebase already proves singleton reuse is safe.
- Keep model loading out of hot paths when using `FastText` or `MediaPipe`.
- If startup cost matters, initialize the adapter once and measure before optimizing further.

## Testing Guidance

- Test with the actual languages the feature supports.
- Add at least one noisy or malformed input case.
- Assert the normalized code you expose, not only the detector's raw value.
- When platform support is conditional, test the guard path around `IsSupported()`.
- For `FastText` and `MediaPipe`, test both bundled-model and explicit-model flows when the app supports both.
