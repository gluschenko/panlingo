# Panlingo.LanguageIdentification.FastText

Welcome to **Panlingo.LanguageIdentification.FastText**, a .NET wrapper for the FastText library by Facebook AI Research (FAIR). This package seamlessly integrates language identification capabilities into .NET applications, leveraging the powerful and efficient FastText library to recognize and classify texts in multiple languages. Ideal for applications that require fast and accurate language detection.

## Requirements

- Runtime: **.NET >= 5.0**
- OS: **Linux (Ubuntu, Debian)**, **Windows 10+** or **Windows Server 2019+**, **macOS**
- Arch: **AMD64** (or **ARM** for macOS)

## Platform support

| **OS / Arch** | x86_64             | arm64              |
| :------------ | :----------------: | :----------------: |
| **Linux**     | :white_check_mark: | :x:                |
| **Windows**   | :white_check_mark: | :x:                |
| **macOS**     | :white_check_mark: | :white_check_mark: |

:white_check_mark: — Full support |
:x: — No support |
:construction: — Under research

## Installation

To integrate the FastText functionality, follow these steps:

**Install the NuGet package**:

```sh
dotnet add package Panlingo.LanguageIdentification.FastText
```

## Usage

Integrating the FastText library into your .NET application is straightforward. Here’s a quick guide to get you started:

1. **Install the Package**: Ensure you have added the `Panlingo.LanguageIdentification.FastText` package to your project using the provided installation command.
2. **Download the Models**: Follow the provided commands to download the pretrained language identification models.
3. **Initialize the Library**: Follow the example snippet to initialize and use the FastText library for detecting languages.

```csharp
using Panlingo.LanguageIdentification.FastText;

class Program
{
    static void Main()
    {
        using var fastText = new FastTextDetector();
        fastText.LoadDefaultModel();

        var predictions = fastText.Predict(
            text: "Привіт, як справи?", 
            count: 10
        );

        foreach (var prediction in predictions)
        {
            Console.WriteLine($"{prediction.Label}: {prediction.Probability}");
        }

        var dimensions = fastText.GetModelDimensions();
        var labels = fastText.GetLabels();
    }
}
```

### Custom models

The default model for this package is [quantized](https://fasttext.cc/docs/en/language-identification.html#:~:text=size%20of%20126MB%20%3B-,lid.176.ftz,-%2C%20which%20is%20the) `lid.176.ftz` (see below). 
You can use the default model by calling the `LoadDefaultModel()` method.

We recommend using the following models, but you can use any model depending on your needs. 
It could even be a model for another text classinfiction tasks, e.g: 
[supervised models](https://fasttext.cc/docs/en/supervised-tutorial.html)

| Model       | Vendor               | Languages | Label format | Learn more | Download |
| :---------- | :------------------- | :-------: | :----------- | :--------- | :------: |
| **lid.176** | Meta Platforms, Inc. | 176       | `__label__en` `__label__uk` `__label__hi` | [fasttext.cc](https://fasttext.cc/docs/en/language-identification.html) | [lid.176.bin](https://dl.fbaipublicfiles.com/fasttext/supervised-models/lid.176.bin) |
| **lid218e** | Meta Platforms, Inc. | 217       | `__label__eng_Latn` `__label__ukr_Cyrl` `__label__hin_Deva` | [@facebook/fasttext-language-identification](https://huggingface.co/facebook/fasttext-language-identification) | [model.bin](https://huggingface.co/facebook/fasttext-language-identification/resolve/main/model.bin?download=true) |
| **GlotLID** | CIS, LMU Munich      | 2155(?)   | `__label__eng_Latn` `__label__ukr_Cyrl` `__label__hin_Deva` | [@cis-lmu/glotlid](https://huggingface.co/cis-lmu/glotlid) | [model_v3.bin](https://huggingface.co/cis-lmu/glotlid/resolve/main/model_v3.bin?download=true) |

#### Use custom model in codes

**You can use the model included in this NuGet package:**
```
using var fastText = new FastTextDetector();
fastText.LoadDefaultModel();
```

**You can specify the path to the model file:**
```
using var fastText = new FastTextDetector();

var modelPath = "/path/to/model/fasttext176.bin";
fastText.LoadModel(modelPath);
```

**Also you can also load the model as a memory stream:**
```
using var fastText = new FastTextDetector();

var modelPath = "/path/to/model/fasttext176.bin";
using var stream = File.Open(modelPath, FileMode.Open);
fastText.LoadModel(stream);
```

## Alternatives

If you are exploring other options, here are some alternatives to consider:

- **[theolivenbaum/fastText](https://github.com/theolivenbaum/fastText)**: An unmaintained wrapper for .NET.
- **[olegtarasov/FastText.NetWrapper](https://github.com/olegtarasov/FastText.NetWrapper)**: Another unmaintained wrapper for .NET.

## Sources

1. [Original FastText Repository](https://github.com/facebookresearch/fastText)
2. [Enriching Word Vectors with Subword Information](https://arxiv.org/abs/1607.04606v2)
3. [Language identification at Wikipedia](https://en.wikipedia.org/wiki/Language_identification)

## License

This project is licensed under the [MIT License](./LICENSE) © 2024–2025 Alexander Gluschenko.

Includes software from the following project(s):
- [FastText](https://github.com/facebookresearch/fastText) — © 2016–present Meta Platforms, Inc., licensed under MIT  

See the [LICENSE](./LICENSE) file for full details.

---

We value your feedback. Feel free to open issues or contribute to the repository. Let’s make language detection in .NET even more powerful and versatile! 🌍📝

Happy coding! 👩‍💻👨‍💻

---

Stay updated by following our repository. For any inquiries or support, reach out through the [issues page](https://github.com/gluschenko/panlingo/issues).