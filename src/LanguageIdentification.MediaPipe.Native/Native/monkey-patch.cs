string Normalize(string path) => path.Replace('\\', '/');

string[] FindFiles(string suffix) => Directory
    .EnumerateFiles(".", "*", SearchOption.AllDirectories)
    .Where(path => Normalize(path).EndsWith(suffix, StringComparison.Ordinal))
    .ToArray();

void PatchFile(string suffix, params Patch[] patches)
{
    var files = FindFiles(suffix);
    if (files.Length != 1)
    {
        throw new InvalidOperationException(
            $"Expected exactly one '{suffix}', found {files.Length}."
        );
    }

    var file = files[0];
    var original = File.ReadAllText(file);
    var content = original.Replace("\r\n", "\n");

    foreach (var patch in patches)
    {
        var matchCount = CountOccurrences(content, patch.Pattern);
        if (matchCount != patch.ExpectedMatches)
        {
            throw new InvalidOperationException(
                $"Expected {patch.ExpectedMatches} match(es) for patch pattern in " +
                $"'{Normalize(file)}', found {matchCount}."
            );
        }

        content = content.Replace(patch.Pattern, patch.Replacement, StringComparison.Ordinal);
    }

    if (content == original.Replace("\r\n", "\n"))
    {
        throw new InvalidOperationException(
            $"Patch made no changes in '{Normalize(file)}'."
        );
    }

    var newline = original.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
    File.WriteAllText(file, content.Replace("\n", newline));
    Console.WriteLine($"Patched: {Normalize(file)}");
}

int CountOccurrences(string content, string pattern)
{
    var count = 0;
    var startIndex = 0;

    while (true)
    {
        var matchIndex = content.IndexOf(pattern, startIndex, StringComparison.Ordinal);
        if (matchIndex < 0)
        {
            return count;
        }

        count++;
        startIndex = matchIndex + pattern.Length;
    }
}

void VerifyMarker(string suffix, string marker)
{
    var files = FindFiles(suffix);
    if (files.Length != 1)
    {
        throw new InvalidOperationException(
            $"Expected exactly one '{suffix}' during verification, found {files.Length}."
        );
    }

    var content = File.ReadAllText(files[0]);
    if (!content.Contains(marker, StringComparison.Ordinal))
    {
        throw new InvalidOperationException(
            $"Verification marker '{marker}' was not found in '{Normalize(files[0])}'."
        );
    }
}

Console.WriteLine("[Monkey patching is started]");

PatchFile(
    ".h.template",
    new Patch("{LATEST_METADATA_PARSER_VERSION}", "1.5.0")
);

PatchFile(
    ".bazelrc",
    new Patch(
        """
        build:linux --define=xnn_enable_avx512amx=false
        """,
        $$"""
        build:linux --define=xnn_enable_avx512amx=false
        build:linux --define=xnn_enable_avx512fp16=false
        build:linux --define=xnn_enable_avxvnni=false
        build:linux --define=xnn_enable_avxvnniint8=false

        """
    ),
    new Patch(
        """
        build:windows --host_copt=/D_USE_MATH_DEFINES
        """,
        $$"""
        build:windows --host_copt=/D_USE_MATH_DEFINES
        build:windows --define=xnn_enable_avx512amx=false
        build:windows --define=xnn_enable_avx512fp16=false
        build:windows --define=xnn_enable_avxvnni=false
        build:windows --define=xnn_enable_avxvnniint8=false
        build:windows --copt=/MT
        build:windows --cxxopt=/MT
        build:windows --host_copt=/MT
        build:windows --host_cxxopt=/MT
        build:windows --linkopt=libcmt.lib
        build:windows --linkopt=libvcruntime.lib
        build:windows --linkopt=libucrt.lib
        build:windows --linkopt=/NODEFAULTLIB:msvcrt.lib
        build:windows --linkopt=/NODEFAULTLIB:ucrtbase.lib
        build:windows --define=absl_disable_stacktrace=1
        build:windows --define=absl_disable_symbolize=1

        """
    )
);

PatchFile(
    "tasks/cc/text/language_detector/language_detector.cc",
    new Patch(
        """
            language_detector_result.push_back(
                {.language_code = *category.category_name,
                 .probability = category.score});
        """,
        $$"""
            LanguageDetectorPrediction prediction;
            prediction.language_code = *category.category_name;
            prediction.probability = category.score;
            language_detector_result.push_back(prediction);
        """
    )
);

PatchFile(
    "tasks/c/text/language_detector/language_detector.h",
    new Patch(
        "#define MP_EXPORT __attribute__((visibility(\"default\")))",
        $$"""
        #if defined(_WIN32) || defined(_WIN64)
        #define MP_EXPORT __declspec(dllexport)
        #elif defined(__GNUC__) || defined(__clang__)
        #define MP_EXPORT __attribute__((visibility("default")))
        #else
        #define MP_EXPORT
        #endif
        """
    )
);

PatchFile(
    "WORKSPACE",
    new Patch(
        "ad37707084a6d4ff41be10cbe8540c75bea057ba79d0de6c367c1bfac6ba0852",
        "8eeb81ff6bc7ab2de678c0c4a3d18b02c382a5122ac4edc26a3334c858531739"
    )
);

PatchFile(
    "tasks/c/text/language_detector/language_detector.h",
    new Patch(
        "  struct ClassifierOptions classifier_options;",
        $$"""
          struct ClassifierOptions classifier_options;

          // Number of CPU threads used by the TensorFlow Lite interpreter.
          int cpu_num_threads;
        """
    )
);

PatchFile(
    "tasks/c/text/language_detector/language_detector.cc",
    new Patch(
        """
          CppConvertToClassifierOptions(options.classifier_options,
                                        &cpp_options->classifier_options);
        """,
        $$"""
          CppConvertToClassifierOptions(options.classifier_options,
                                        &cpp_options->classifier_options);
          cpp_options->cpu_num_threads = options.cpu_num_threads;
        """
    )
);

PatchFile(
    "tasks/cc/text/language_detector/language_detector.h",
    new Patch(
        "  components::processors::ClassifierOptions classifier_options;",
        $$"""
          components::processors::ClassifierOptions classifier_options;

          int cpu_num_threads = -1;
        """
    )
);

PatchFile(
    "tasks/cc/text/language_detector/language_detector.cc",
    new Patch(
        """
          options_proto->mutable_classifier_options()->Swap(
              classifier_options_proto.get());
        """,
        $$"""
          options_proto->mutable_classifier_options()->Swap(
              classifier_options_proto.get());
          options_proto->set_cpu_num_threads(options->cpu_num_threads);
        """
    )
);

PatchFile(
    "tasks/cc/text/text_classifier/proto/text_classifier_graph_options.proto",
    new Patch(
        "  optional components.processors.proto.ClassifierOptions classifier_options = 2;",
        $$"""
          optional components.processors.proto.ClassifierOptions classifier_options = 2;

          optional int32 cpu_num_threads = 3 [default = -1];
        """
    )
);

PatchFile(
    "tasks/cc/text/text_classifier/text_classifier_graph.cc",
    new Patch(
        "        model_resources, task_options.base_options().acceleration(), graph);",
        $$"""
                model_resources, task_options.base_options().acceleration(),
                task_options.cpu_num_threads(), graph);
        """
    )
);

PatchFile(
    "tasks/cc/core/proto/inference_subgraph.proto",
    new Patch(
        "  optional string model_resources_tag = 2;",
        $$"""
          optional string model_resources_tag = 2;

          optional int32 cpu_num_threads = 3 [default = -1];
        """
    )
);

PatchFile(
    "tasks/cc/core/model_task_graph.h",
    new Patch(
        """
              const proto::Acceleration& acceleration,
              api2::builder::Graph& graph) const;
        """,
        $$"""
              const proto::Acceleration& acceleration,
              int cpu_num_threads,
              api2::builder::Graph& graph) const;
        """
    )
);

PatchFile(
    "tasks/cc/core/model_task_graph.cc",
    new Patch(
        """
            inference_node.GetOptions<mediapipe::InferenceCalculatorOptions>()
                .mutable_delegate()
                ->CopyFrom(inference_delegate);
        """,
        $$"""
        auto& inference_options =
            inference_node.GetOptions<mediapipe::InferenceCalculatorOptions>();
        inference_options.mutable_delegate()->CopyFrom(inference_delegate);
        inference_options.set_cpu_num_thread(
            subgraph_options->cpu_num_threads());
        """
    ),
    new Patch(
        "    const proto::Acceleration& acceleration, Graph& graph) const {",
        $$"""
        const proto::Acceleration& acceleration, int cpu_num_threads,
        Graph& graph) const {
        """
    ),
    new Patch(
        "  inference_subgraph_opts.mutable_base_options()",
        $$"""
        inference_subgraph_opts.set_cpu_num_threads(cpu_num_threads);
        inference_subgraph_opts.mutable_base_options()
        """,
        ExpectedMatches: 2
    )
);

VerifyMarker(
    "tasks/c/text/language_detector/language_detector.h",
    "int cpu_num_threads;"
);
VerifyMarker(
    "tasks/c/text/language_detector/language_detector.cc",
    "cpp_options->cpu_num_threads = options.cpu_num_threads;"
);
VerifyMarker(
    "tasks/cc/text/language_detector/language_detector.cc",
    "options_proto->set_cpu_num_threads(options->cpu_num_threads);"
);
VerifyMarker(
    "tasks/cc/core/model_task_graph.cc",
    "inference_options.set_cpu_num_thread"
);

Console.WriteLine("[Monkey patch verification passed: 4 markers]");
Console.WriteLine("[Monkey patching is done]");

record Patch(string Pattern, string Replacement, int ExpectedMatches = 1);
