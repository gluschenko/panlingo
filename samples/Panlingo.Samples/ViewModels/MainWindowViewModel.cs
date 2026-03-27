using Panlingo.LanguageCode;
using Panlingo.LanguageIdentification.CLD2;
using Panlingo.LanguageIdentification.CLD3;
using Panlingo.LanguageIdentification.FastText;
using Panlingo.LanguageIdentification.Lingua;
using Panlingo.LanguageIdentification.MediaPipe;
using Panlingo.LanguageIdentification.Whatlang;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace Panlingo.Samples.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly DelegateCommand _loadSampleCommand;
    private readonly DelegateCommand _runSelectedCommand;
    private readonly DelegateCommand _runAllCommand;
    private readonly DelegateCommand _clearResultsCommand;

    private DetectorOption _selectedEngine;
    private SampleTextOption _selectedSample;
    private string _inputText;
    private string _statusMessage;
    private bool _useLinguaMixedMode;
    private bool _preloadLinguaModels;
    private decimal _resultCount;

    public MainWindowViewModel()
    {
        Engines = new ObservableCollection<DetectorOption>
        {
            new(
                DetectorKind.CLD2,
                "CLD2",
                "Google quadgram detector",
                "Fast classical detector with multi-language output, script information and reliability flags."
            ),
            new(
                DetectorKind.CLD3,
                "CLD3",
                "Google neural detector",
                "Compact neural detector with top-N predictions and reliability signals."
            ),
            new(
                DetectorKind.FastText,
                "FastText",
                "Meta neural detector",
                "Requires a model, here the built-in lid.176 model is loaded from the Panlingo package."
            ),
            new(
                DetectorKind.Lingua,
                "Lingua",
                "Rust trigram detector",
                "Builder-based detector that can score one language or segment mixed-language text."
            ),
            new(
                DetectorKind.MediaPipe,
                "MediaPipe",
                "Google TFLite detector",
                "Neural model packaged in Panlingo with configurable result count and threshold."
            ),
            new(
                DetectorKind.Whatlang,
                "Whatlang",
                "Rust script-aware detector",
                "Single-language detector that also reports script and reliability."
            ),
        };

        Samples = new ObservableCollection<SampleTextOption>
        {
            new("English", "The quick brown fox jumps over the lazy dog. This sentence is commonly used in typography."),
            new("Armenian", "\u0532\u0561\u0580\u0565\u0582, \u056B\u0576\u0579\u057A\u0565\u057D \u0565\u057D\u0589 \u0531\u0575\u057D \u0576\u0561\u056D\u0561\u0564\u0561\u057D\u0578\u0582\u0569\u0575\u0578\u0582\u0576\u0568 \u057A\u0565\u057F\u0584 \u0567 \u0573\u0561\u0576\u0561\u0579\u057E\u056B \u0578\u0580\u057A\u0565\u057D \u0570\u0561\u0575\u0565\u0580\u0565\u0576\u0589"),
            new("Russian", "\u041F\u0440\u0438\u0432\u0435\u0442, \u043A\u0430\u043A \u0434\u0435\u043B\u0430? \u042D\u0442\u043E \u0434\u0435\u043C\u043E\u043D\u0441\u0442\u0440\u0430\u0446\u0438\u043E\u043D\u043D\u044B\u0439 \u0442\u0435\u043A\u0441\u0442 \u0434\u043B\u044F \u043F\u0440\u043E\u0432\u0435\u0440\u043A\u0438 \u043E\u043F\u0440\u0435\u0434\u0435\u043B\u0435\u043D\u0438\u044F \u044F\u0437\u044B\u043A\u0430."),
            new("Mixed", "Hello team! \u041F\u0440\u0438\u0432\u0456\u0442 \u0443\u0441\u0456\u043C. \u0532\u0561\u0580\u0565\u0582 \u0562\u0578\u056C\u0578\u0580\u056B\u0576. Bonjour tout le monde."),
            new("Asian scripts", "\u3053\u3093\u306B\u3061\u306F\u4E16\u754C\u3002\uC548\uB155\uD558\uC138\uC694 \uC5EC\uB7EC\uBD84\u3002\u4F60\u597D\uFF0C\u4E16\u754C\u3002"),
            new("Short noisy text", "Ciao! Hola! hi"),
        };

        _selectedEngine = Engines[0];
        _selectedSample = Samples[3];
        _inputText = _selectedSample.Text;
        _statusMessage = "Choose a detector, optionally load a sample, then run detection.";
        _resultCount = 5;

        SupportMatrix = new ObservableCollection<string>();
        Results = new ObservableCollection<DetectionResultItem>();

        _loadSampleCommand = new DelegateCommand(LoadSelectedSample);
        _runSelectedCommand = new DelegateCommand(RunSelectedDetector, CanRunDetection);
        _runAllCommand = new DelegateCommand(RunAllDetectors, CanRunDetection);
        _clearResultsCommand = new DelegateCommand(ClearResults);

        RefreshInspector();
        LoadSelectedSample();
    }

    public ObservableCollection<DetectorOption> Engines { get; }

    public ObservableCollection<SampleTextOption> Samples { get; }

    public ObservableCollection<DetectionResultItem> Results { get; }

    public ObservableCollection<string> SupportMatrix { get; }

    public DelegateCommand LoadSampleCommand => _loadSampleCommand;

    public DelegateCommand RunSelectedCommand => _runSelectedCommand;

    public DelegateCommand RunAllCommand => _runAllCommand;

    public DelegateCommand ClearResultsCommand => _clearResultsCommand;

    public string HeaderText =>
        $"Runtime: {RuntimeInformation.RuntimeIdentifier} ({RuntimeInformation.OSArchitecture}). " +
        "Desktop sample for comparing Panlingo detectors and normalizing their output with Panlingo.LanguageCode.";

    public DetectorOption SelectedEngine
    {
        get => _selectedEngine;
        set
        {
            if (SetProperty(ref _selectedEngine, value))
            {
                RefreshInspector();
            }
        }
    }

    public SampleTextOption SelectedSample
    {
        get => _selectedSample;
        set => SetProperty(ref _selectedSample, value);
    }

    public string InputText
    {
        get => _inputText;
        set
        {
            if (SetProperty(ref _inputText, value))
            {
                _runSelectedCommand.RaiseCanExecuteChanged();
                _runAllCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public bool UseLinguaMixedMode
    {
        get => _useLinguaMixedMode;
        set => SetProperty(ref _useLinguaMixedMode, value);
    }

    public bool PreloadLinguaModels
    {
        get => _preloadLinguaModels;
        set => SetProperty(ref _preloadLinguaModels, value);
    }

    public decimal ResultCount
    {
        get => _resultCount;
        set => SetProperty(ref _resultCount, value);
    }

    public string InspectorTitle => $"{SelectedEngine.DisplayName} notes";

    public string InspectorText =>
        SelectedEngine.Kind switch
        {
            DetectorKind.CLD2 => "Returns language, script, reliability and text proportion. Good baseline for comparing old-school heuristics against neural models.",
            DetectorKind.CLD3 => "Uses min/max byte configuration. This sample uses 0/512 and asks CLD3 for top-N predictions.",
            DetectorKind.FastText => "Loads the built-in lid.176 model before predicting. Labels are converted from __label__xx to ISO-like codes.",
            DetectorKind.Lingua => "Can run either regular ranking or mixed-language segmentation. Preloading makes the first inference slower but more stable afterwards.",
            DetectorKind.MediaPipe => "Loads the built-in TFLite model through MediaPipeOptions.FromDefault and respects the selected Top N value.",
            DetectorKind.Whatlang => "Single best language plus script detection. Great for compact single-shot checks.",
            _ => string.Empty,
        };

    private bool CanRunDetection()
    {
        return !string.IsNullOrWhiteSpace(InputText);
    }

    private void LoadSelectedSample()
    {
        InputText = SelectedSample.Text;
        StatusMessage = $"Loaded sample: {SelectedSample.Title}.";
    }

    private void ClearResults()
    {
        Results.Clear();
        StatusMessage = "Results cleared.";
    }

    private void RunSelectedDetector()
    {
        Results.Clear();
        ExecuteDetector(SelectedEngine.Kind);
    }

    private void RunAllDetectors()
    {
        Results.Clear();

        foreach (var engine in Engines)
        {
            ExecuteDetector(engine.Kind);
        }

        StatusMessage = $"Compared all detectors on {CountInputUnits()} chars of input.";
    }

    private void ExecuteDetector(DetectorKind detectorKind)
    {
        try
        {
            switch (detectorKind)
            {
                case DetectorKind.CLD2:
                    RunCld2();
                    break;
                case DetectorKind.CLD3:
                    RunCld3();
                    break;
                case DetectorKind.FastText:
                    RunFastText();
                    break;
                case DetectorKind.Lingua:
                    RunLingua();
                    break;
                case DetectorKind.MediaPipe:
                    RunMediaPipe();
                    break;
                case DetectorKind.Whatlang:
                    RunWhatlang();
                    break;
            }

            StatusMessage = $"{detectorKind} finished successfully.";
        }
        catch (Exception ex)
        {
            Results.Add(new DetectionResultItem(
                title: $"{detectorKind}: error",
                summary: ex.GetType().Name,
                details: ex.Message
            ));

            StatusMessage = $"{detectorKind} failed: {ex.Message}";
        }
    }

    private void RunCld2()
    {
        if (!CLD2Detector.IsSupported())
        {
            AddUnsupportedResult("CLD2");
            return;
        }

        using var detector = new CLD2Detector();
        foreach (var prediction in detector.PredictLanguage(InputText))
        {
            AddCodeBasedResult(
                engine: "CLD2",
                rawCode: prediction.Language,
                score: prediction.Probability,
                details: $"Script={prediction.Script}; Reliable={prediction.IsReliable}; Proportion={prediction.Proportion:P2}"
            );
        }
    }

    private void RunCld3()
    {
        if (!CLD3Detector.IsSupported())
        {
            AddUnsupportedResult("CLD3");
            return;
        }

        using var detector = new CLD3Detector(minNumBytes: 0, maxNumBytes: 512);
        foreach (var prediction in detector.PredictLanguages(InputText, (int)ResultCount))
        {
            AddCodeBasedResult(
                engine: "CLD3",
                rawCode: prediction.Language,
                score: prediction.Probability,
                details: $"Reliable={prediction.IsReliable}; Proportion={prediction.Proportion:P2}"
            );
        }
    }

    private void RunFastText()
    {
        if (!FastTextDetector.IsSupported())
        {
            AddUnsupportedResult("FastText");
            return;
        }

        using var detector = new FastTextDetector();
        detector.LoadDefaultModel();

        foreach (var prediction in detector.Predict(InputText, (int)ResultCount))
        {
            var code = NormalizeFastTextLabel(prediction.Label);
            AddCodeBasedResult(
                engine: "FastText",
                rawCode: code,
                score: prediction.Probability,
                details: $"Model label={prediction.Label}"
            );
        }
    }

    private void RunLingua()
    {
        if (!LinguaDetector.IsSupported())
        {
            AddUnsupportedResult("Lingua");
            return;
        }

        using var builder = LinguaDetectorBuilder.FromLanguages(Enum.GetValues<LinguaLanguage>());
        if (PreloadLinguaModels)
        {
            builder.WithPreloadedLanguageModels();
        }

        using var detector = builder.Build();

        if (UseLinguaMixedMode)
        {
            foreach (var prediction in detector.PredictMixedLanguages(InputText))
            {
                var alpha3 = detector.GetLanguageCode(prediction.Language, LinguaLanguageCode.Alpha3);
                AddCodeBasedResult(
                    engine: "Lingua",
                    rawCode: alpha3,
                    score: prediction.Confidence,
                    details: $"Fragment=\"{prediction.Fragment}\"; Words={prediction.WordCount}; Range={prediction.Range}"
                );
            }

            return;
        }

        foreach (var prediction in detector.PredictLanguages(InputText, (int)ResultCount))
        {
            var alpha3 = detector.GetLanguageCode(prediction.Language, LinguaLanguageCode.Alpha3);
            AddCodeBasedResult(
                engine: "Lingua",
                rawCode: alpha3,
                score: prediction.Confidence,
                details: $"Lingua enum={prediction.Language}"
            );
        }
    }

    private void RunMediaPipe()
    {
        if (!MediaPipeDetector.IsSupported())
        {
            AddUnsupportedResult("MediaPipe");
            return;
        }

        var options = MediaPipeOptions.FromDefault()
            .WithResultCount((int)ResultCount)
            .WithScoreThreshold(0.0f);

        using var detector = new MediaPipeDetector(options);

        foreach (var prediction in detector.PredictLanguages(InputText))
        {
            AddCodeBasedResult(
                engine: "MediaPipe",
                rawCode: prediction.Language,
                score: prediction.Probability,
                details: $"Unknown={prediction.IsUnknown()}"
            );
        }
    }

    private void RunWhatlang()
    {
        if (!WhatlangDetector.IsSupported())
        {
            AddUnsupportedResult("Whatlang");
            return;
        }

        using var detector = new WhatlangDetector();
        var prediction = detector.PredictLanguage(InputText);
        if (prediction is null)
        {
            Results.Add(new DetectionResultItem(
                title: "Whatlang: no result",
                summary: "The detector could not classify the current input.",
                details: "Try a longer text or a sample with cleaner script."
            ));
            return;
        }

        var alpha3 = detector.GetLanguageCode(prediction.Language);
        var englishName = detector.GetLanguageEnglishName(prediction.Language);

        var normalized = TryNormalizeCode(alpha3) ?? alpha3;
        var alpha2 = TryGetLanguageEntity(normalized, Panlingo.LanguageCode.Models.LanguageCodeEntity.Alpha2);
        var alpha3Normalized = TryGetLanguageEntity(normalized, Panlingo.LanguageCode.Models.LanguageCodeEntity.Alpha3) ?? alpha3;

        Results.Add(new DetectionResultItem(
            title: $"Whatlang: {englishName}",
            summary: $"raw={prediction.Language}; alpha2={alpha2 ?? "-"}; alpha3={alpha3Normalized}; normalized={normalized}; score={prediction.Confidence:F4}",
            details: $"Script={prediction.Script}; Reliable={prediction.IsReliable}"
        ));
    }

    private void AddUnsupportedResult(string engine)
    {
        Results.Add(new DetectionResultItem(
            title: $"{engine}: unsupported",
            summary: $"The current runtime is {RuntimeInformation.RuntimeIdentifier} ({RuntimeInformation.OSArchitecture}).",
            details: "This detector reports IsSupported() == false on the current platform."
        ));
    }

    private void AddCodeBasedResult(string engine, string rawCode, double score, string details)
    {
        var normalized = TryNormalizeCode(rawCode);
        var alpha2 = normalized is null ? null : TryGetLanguageEntity(normalized, Panlingo.LanguageCode.Models.LanguageCodeEntity.Alpha2);
        var alpha3 = normalized is null ? null : TryGetLanguageEntity(normalized, Panlingo.LanguageCode.Models.LanguageCodeEntity.Alpha3);
        var englishName = normalized is null ? null : TryGetLanguageEntity(normalized, Panlingo.LanguageCode.Models.LanguageCodeEntity.EnglishName);

        Results.Add(new DetectionResultItem(
            title: $"{engine}: {englishName ?? rawCode}",
            summary: $"raw={rawCode}; normalized={normalized ?? "-"}; alpha2={alpha2 ?? "-"}; alpha3={alpha3 ?? "-"}; score={score:F4}",
            details: details
        ));
    }

    private static string NormalizeFastTextLabel(string label)
    {
        const string Prefix = "__label__";

        if (label.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
        {
            return label[Prefix.Length..];
        }

        return label;
    }

    private static string? TryNormalizeCode(string code)
    {
        var resolver = new LanguageCodeResolver()
            .ToLowerAndTrim()
            .ConvertFromIETF()
            .ConvertFromDeprecatedCode()
            .ReduceToMacrolanguage()
            .Select(Panlingo.LanguageCode.Models.LanguageCodeEntity.Alpha3);

        if (!LanguageCodeHelper.TryResolve(code, out var normalized, resolver))
        {
            return null;
        }

        return normalized;
    }

    private static string? TryGetLanguageEntity(string code, Panlingo.LanguageCode.Models.LanguageCodeEntity entity)
    {
        if (LanguageCodeHelper.TryGetEntity(code, entity, out var value))
        {
            return value;
        }

        return null;
    }

    private int CountInputUnits()
    {
        return InputText.Length;
    }

    private void RefreshInspector()
    {
        SupportMatrix.Clear();
        SupportMatrix.Add($"CLD2: {(CLD2Detector.IsSupported() ? "Yes" : "No")}");
        SupportMatrix.Add($"CLD3: {(CLD3Detector.IsSupported() ? "Yes" : "No")}");
        SupportMatrix.Add($"FastText: {(FastTextDetector.IsSupported() ? "Yes" : "No")}");
        SupportMatrix.Add($"Lingua: {(LinguaDetector.IsSupported() ? "Yes" : "No")}");
        SupportMatrix.Add($"MediaPipe: {(MediaPipeDetector.IsSupported() ? "Yes" : "No")}");
        SupportMatrix.Add($"Whatlang: {(WhatlangDetector.IsSupported() ? "Yes" : "No")}");

        RaisePropertyChanged(nameof(InspectorTitle));
        RaisePropertyChanged(nameof(InspectorText));
    }
}
