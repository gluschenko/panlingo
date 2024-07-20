# Panlingo.LanguageIdentification.FastText

This is a .NET wrapper for the FastText library by Facebook AI Research (FAIR).
      
This package facilitates the integration of language identification capabilities into .NET applications,
leveraging the powerful and efficient FastText library to recognize and classify texts in multiple languages.
Ideal for applications that require fast and accurate language detection.

## Requirements

* .NET >=5.0
* Linux

## How to use?

### Installation

To use the FastText you have to install thos NuGet package:

```
dotnet add package Panlingo.LanguageIdentification.FastText
```

Also requires to download one of LID models by Facebook:
```
curl --location -o /models/fasttext176.bin https://dl.fbaipublicfiles.com/fasttext/supervised-models/lid.176.bin
curl --location -o /models/fasttext217.bin https://huggingface.co/facebook/fasttext-language-identification/resolve/main/model.bin?download=true
```

About pretrained LID models:
* [176 languages](https://fasttext.cc/docs/en/language-identification.html)
* [217 languages + script](https://huggingface.co/facebook/fasttext-language-identification)

### API

TODO

## Alternatives

* [theolivenbaum/fastText](https://github.com/theolivenbaum/fastText) — unmaintained wrapper for .NET
* [olegtarasov/FastText.NetWrapper](https://github.com/olegtarasov/FastText.NetWrapper) — unmaintained wrapper for .NET
