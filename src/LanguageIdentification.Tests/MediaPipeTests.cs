using System.Runtime.InteropServices;
using System.Diagnostics;
using Panlingo.LanguageIdentification.MediaPipe;
using Panlingo.LanguageIdentification.Tests.Helpers;
using Xunit.Abstractions;

namespace Panlingo.LanguageIdentification.Tests;

public class MediaPipeTests : IAsyncLifetime
{
    private const int TimingIterations = 40;
    private const int TimingRounds = 5;

    private readonly string _modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models/mediapipe_language_detector.tflite");
    private readonly ITestOutputHelper _output;

    public MediaPipeTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void MediaPipeCpuNumThreadsOptionsAreValidated()
    {
        var options = MediaPipeOptions.FromDefault().WithCpuNumThreads(4);

        Assert.Equal(4, options.CpuNumThreads);
        Assert.Equal(-1, MediaPipeOptions.FromDefault().CpuNumThreads);
        Assert.Throws<ArgumentOutOfRangeException>(
            () => MediaPipeOptions.FromDefault().WithCpuNumThreads(-2)
        );
    }

    public static IEnumerable<object[]> CpuNumThreadsValues()
    {
        yield return new object[] { -1 };
        yield return new object[] { 1 };
        yield return new object[] { 2 };
        yield return new object[] { 4 };
    }

    public static IReadOnlyList<(string ExpectedLanguage, string Text)> InferenceTexts { get; } = new[]
    {
        ("en", "The quick brown fox jumps over the lazy dog."),
        ("en", "Language identification should continue to work when the interpreter uses multiple CPU threads."),
        ("en", "This is a longer English sentence used to verify MediaPipe inference."),
        ("en", "A reliable detector must return a prediction for ordinary application text."),
        ("en", "The package is tested with default models and several native interpreter configurations."),
        ("fr", "Le détecteur doit continuer à fonctionner avec plusieurs textes et plusieurs threads du processeur."),
        ("fr", "Cette phrase française vérifie que l'inférence MediaPipe produit une prédiction fiable."),
        ("de", "Der Detektor muss mit verschiedenen Texten und mehreren Prozessorthreads funktionieren."),
        ("de", "Dieser deutsche Satz überprüft die zuverlässige MediaPipe-Inferenz."),
        ("es", "El detector debe funcionar correctamente con diferentes textos y varios hilos del procesador."),
        ("es", "Esta frase en español comprueba que la inferencia de MediaPipe produce una predicción fiable."),
        ("it", "Il rilevatore deve funzionare correttamente con testi diversi e più thread del processore."),
        ("it", "Questa frase italiana verifica che l'inferenza di MediaPipe produca una previsione affidabile."),
        ("pt", "O detector deve funcionar corretamente com textos diferentes e várias threads do processador."),
        ("pt", "Esta frase em português verifica se a inferência do MediaPipe produz uma previsão confiável."),
        ("pl", "Detektor powinien poprawnie działać z różnymi tekstami i wieloma wątkami procesora."),
        ("pl", "To polskie zdanie sprawdza, czy inferencja MediaPipe zwraca wiarygodną prognozę."),
        ("tr", "Algılayıcı farklı metinlerle ve birden fazla işlemci iş parçacığıyla çalışmalıdır."),
        ("tr", "Bu Türkçe cümle MediaPipe çıkarımının güvenilir bir tahmin ürettiğini doğrular."),
        ("ar", "يجب أن يعمل الكاشف بشكل صحيح مع نصوص مختلفة ومع عدة خيوط للمعالج."),
        ("ar", "تتحقق هذه الجملة العربية من أن استدلال MediaPipe ينتج تنبؤاً موثوقاً."),
        ("uk", "Швидка бура лисиця перестрибує через ледачого собаку."),
        ("uk", "Визначення мови має продовжувати працювати під час використання кількох потоків CPU."),
        ("uk", "Це довше українське речення для перевірки інференсу MediaPipe."),
        ("uk", "Надійний детектор повинен повертати прогноз для звичайного тексту застосунку."),
        ("uk", "Пакет перевіряється з типовою моделлю та кількома конфігураціями нативного інтерпретатора."),
        ("zh", "语言检测器应该能够使用多个处理器线程处理不同的文本。"),
        ("zh", "这句话用于验证 MediaPipe 推理能够返回可靠的预测结果。"),
        ("ja", "検出器は複数のプロセッサスレッドとさまざまなテキストで正常に動作する必要があります。"),
        ("ja", "この日本語の文章はMediaPipe推論が信頼できる予測を返すことを確認します。"),
    };

    [SkippableTheory]
    [MemberData(nameof(CpuNumThreadsValues))]
    public void MediaPipeInferenceWorksWithConfiguredCpuNumThreads(int cpuNumThreads)
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        using var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromDefault()
                .WithResultCount(3)
                .WithCpuNumThreads(cpuNumThreads)
        );

        foreach (var testCase in InferenceTexts)
        {
            var predictions = mediaPipe.PredictLanguages(testCase.Text).ToArray();

            Assert.NotEmpty(predictions);
            Assert.Equal(testCase.ExpectedLanguage, predictions[0].Language);
            Assert.All(
                predictions,
                prediction => Assert.InRange(prediction.Probability, 0.0, 1.0)
            );
        }
    }

    [SkippableFact]
    [Trait("Category", "Performance")]
    public void MediaPipeCpuNumThreadsTimingShowsConfiguredModes()
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());
        Skip.If(
            Environment.ProcessorCount == 1,
            "Timing comparison requires more than one logical processor."
        );

        var machineThreadCount = Environment.ProcessorCount;

        using var singleThreadDetector = new MediaPipeDetector(
            MediaPipeOptions.FromDefault()
                .WithResultCount(1)
                .WithCpuNumThreads(1)
        );
        using var multiThreadDetector = new MediaPipeDetector(
            MediaPipeOptions.FromDefault()
                .WithResultCount(1)
                .WithCpuNumThreads(machineThreadCount)
        );

        var texts = InferenceTexts.Select(testCase => testCase.Text).ToArray();

        // Warm up model execution and JIT before measuring either mode.
        RunInference(singleThreadDetector, texts, iterations: 2);
        RunInference(multiThreadDetector, texts, iterations: 2);

        var singleThreadTimings = MeasureInference(singleThreadDetector, texts);
        var multiThreadTimings = MeasureInference(multiThreadDetector, texts);
        var singleThreadMedian = Median(singleThreadTimings);
        var multiThreadMedian = Median(multiThreadTimings);
        var speedup = singleThreadMedian / multiThreadMedian;

        _output.WriteLine($"CpuNumThreads=1 timings (ms): {string.Join(", ", singleThreadTimings.Select(x => x.ToString("F1")))}");
        _output.WriteLine($"CpuNumThreads={machineThreadCount} timings (ms): {string.Join(", ", multiThreadTimings.Select(x => x.ToString("F1")))}");
        _output.WriteLine($"Median speedup (1 thread / {machineThreadCount} threads): {speedup:F2}x");

        Assert.All(singleThreadTimings, timing => Assert.True(timing > 0));
        Assert.All(multiThreadTimings, timing => Assert.True(timing > 0));
    }

    private static double[] MeasureInference(MediaPipeDetector detector, string[] texts)
    {
        var timings = new double[TimingRounds];

        for (var round = 0; round < timings.Length; round++)
        {
            var stopwatch = Stopwatch.StartNew();
            RunInference(detector, texts, TimingIterations);
            stopwatch.Stop();
            timings[round] = stopwatch.Elapsed.TotalMilliseconds;
        }

        return timings;
    }

    private static void RunInference(MediaPipeDetector detector, string[] texts, int iterations)
    {
        for (var iteration = 0; iteration < iterations; iteration++)
        {
            foreach (var text in texts)
            {
                var predictions = detector.PredictLanguages(text).ToArray();
                if (predictions.Length == 0)
                {
                    throw new InvalidOperationException("MediaPipe returned no predictions during timing measurement.");
                }
            }
        }
    }

    private static double Median(IEnumerable<double> values)
    {
        var sorted = values.OrderBy(value => value).ToArray();
        return sorted[sorted.Length / 2];
    }

    [Fact]
    public void MediaPipeCheckPlatformSupport()
    {
        var isSupported = RuntimeInformation.OSArchitecture switch
        {
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
            Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => true,
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => true,
            Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => true,
            _ => false,
        };

        Assert.Equal(isSupported, MediaPipeDetector.IsSupported());
    }

    [SkippableTheory]
    [InlineData("en", Constants.PHRASE_ENG_1, 0.9994)]
    [InlineData("uk", Constants.PHRASE_UKR_1, 0.9999)]
    [InlineData("ru", Constants.PHRASE_RUS_1, 0.9999)]
    public void MediaPipeFileSingleLanguage(string languageCode, string text, double score)
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        using var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromFile(_modelPath).WithResultCount(10)
        );

        var predictions = mediaPipe.PredictLanguages(text: text);
        var mainLanguage = predictions.FirstOrDefault();

        if (mainLanguage is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, mainLanguage.Language);
        Assert.Equal(score, mainLanguage.Probability, Constants.EPSILON);
    }

    [SkippableTheory]
    [InlineData("en", Constants.PHRASE_ENG_1, 0.9994)]
    [InlineData("uk", Constants.PHRASE_UKR_1, 0.9999)]
    [InlineData("ru", Constants.PHRASE_RUS_1, 0.9999)]
    public void MediaPipeStreamSingleLanguage(string languageCode, string text, double score)
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        using var stream = File.Open(_modelPath, FileMode.Open);

        using var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromStream(stream).WithResultCount(10)
        );

        var predictions = mediaPipe.PredictLanguages(text: text);
        var mainLanguage = predictions.FirstOrDefault();

        if (mainLanguage is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, mainLanguage.Language);
        Assert.Equal(score, mainLanguage.Probability, Constants.EPSILON);
    }

    [SkippableTheory]
    [InlineData("en", Constants.PHRASE_ENG_1, 0.9994)]
    [InlineData("uk", Constants.PHRASE_UKR_1, 0.9999)]
    [InlineData("ru", Constants.PHRASE_RUS_1, 0.9999)]
    public void MediaPipeContainedSingleLanguage(string languageCode, string text, double score)
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        using var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromDefault().WithResultCount(10)
        );

        var predictions = mediaPipe.PredictLanguages(text: text);
        var mainLanguage = predictions.FirstOrDefault();

        if (mainLanguage is null)
        {
            throw new NullReferenceException();
        }

        Assert.Equal(languageCode, mainLanguage.Language);
        Assert.Equal(score, mainLanguage.Probability, Constants.EPSILON);
    }

    [SkippableFact]
    public void MediaPipeGetLables()
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        using var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromDefault().WithResultCount(10)
        );

        var labels = mediaPipe.GetLabels();
        Assert.Equal(111, labels.Count());
        Assert.Contains("uz", labels);
        Assert.Contains("uk", labels);
        Assert.Contains("en", labels);
        Assert.Contains("zh-Latn", labels);
    }

    [SkippableTheory]
    [InlineData(Constants.MALFORMED_BYTES_0)]
    [InlineData(Constants.MALFORMED_BYTES_1)]
    [InlineData(Constants.MALFORMED_BYTES_2)]
    [InlineData(Constants.MALFORMED_BYTES_3)]
    [InlineData(Constants.MALFORMED_BYTES_4)]
    [InlineData(Constants.MALFORMED_BYTES_5)]
    [InlineData(Constants.MALFORMED_BYTES_6)]
    [InlineData(Constants.MALFORMED_BYTES_7)]
    [InlineData(Constants.MALFORMED_BYTES_8)]
    [InlineData(Constants.MALFORMED_BYTES_9)]
    [InlineData(Constants.MALFORMED_BYTES_10)]
    [InlineData(Constants.MALFORMED_BYTES_11)]
    [InlineData(Constants.MALFORMED_BYTES_12)]
    public void MediaPipeMalformedBytes(string text)
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        using var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromDefault().WithResultCount(10)
        );

        var predictions = mediaPipe.PredictLanguages(text: text);
    }

    [SkippableFact]
    public void MediaPipeAcceptsNullText()
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        using var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromDefault().WithResultCount(10)
        );

        var predictions = mediaPipe.PredictLanguages(null!).ToArray();

        Assert.NotNull(predictions);
    }

    [SkippableFact]
    public void MediaPipeStreamModelCanReadLabels()
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        using var stream = File.Open(_modelPath, FileMode.Open);
        using var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromStream(stream).WithResultCount(10)
        );

        var labels = mediaPipe.GetLabels();

        Assert.Equal(111, labels.Count());
        Assert.Contains("en", labels);
        Assert.Contains("uk", labels);
    }

    [SkippableFact]
    public void MediaPipeThrowsAfterDispose()
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromDefault().WithResultCount(10)
        );
        mediaPipe.Dispose();

        Assert.Throws<ObjectDisposedException>(() => mediaPipe.PredictLanguages(Constants.PHRASE_ENG_1).ToArray());
        Assert.Throws<ObjectDisposedException>(() => mediaPipe.GetLabels().ToArray());
    }

    [SkippableFact]
    public async Task MediaPipeParallelPredictionsDoNotCrash()
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        using var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromDefault().WithResultCount(10)
        );
        var tasks = Enumerable.Range(0, 24)
            .Select(i => Task.Run(() => mediaPipe.PredictLanguages(i % 2 == 0 ? Constants.PHRASE_NOISY_1 : Constants.PHRASE_MIXED_1).ToArray()));

        var results = await Task.WhenAll(tasks);

        Assert.All(results, predictions =>
        {
            Assert.NotNull(predictions);
            Assert.All(predictions, prediction => Assert.InRange(prediction.Probability, 0, 1));
        });
    }

    [SkippableFact]
    public async Task MediaPipePredictAndDisposeRaceDoesNotCrash()
    {
        Skip.IfNot(MediaPipeDetector.IsSupported());

        var mediaPipe = new MediaPipeDetector(
            options: MediaPipeOptions.FromDefault().WithResultCount(10)
        );
        var predict = Task.Run(() =>
        {
            try
            {
                _ = mediaPipe.PredictLanguages(Constants.PHRASE_ENG_1).ToArray();
            }
            catch (ObjectDisposedException)
            {
            }
        });
        var dispose = Task.Run(mediaPipe.Dispose);

        await Task.WhenAll(predict, dispose);
    }

    public async Task InitializeAsync()
    {
        var url = "https://storage.googleapis.com/mediapipe-models/language_detector/language_detector/float32/1/language_detector.tflite";
        await FileHelper.DownloadAsync(
            path: _modelPath,
            url: url
        );
    }

    public async Task DisposeAsync()
    {
        if (File.Exists(_modelPath))
        {
            File.Delete(_modelPath);
        }

        await Task.CompletedTask;
    }
}
