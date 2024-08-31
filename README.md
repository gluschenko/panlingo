# Language Identification

[![GitHub CI](https://github.com/gluschenko/language-identification/actions/workflows/github-ci.yml/badge.svg)](https://github.com/gluschenko/language-identification/actions/workflows/github-ci.yml)

## Overview

Welcome to the **Language Identification** repository! 🚀

This project presents a comprehensive collection of language identification 
libraries for .NET. Its primary purpose is to bring popular 
language identification models to the .NET ecosystem, 
allowing developers to seamlessly integrate language detection 
functionality into their applications.

## Libraries

| Library | Nuget Release |
| :------ | :------------ |
| [Panlingo.LanguageIdentification.CLD2](./README_CLD2.md) |  [![NuGet Version](https://buildstats.info/nuget/Panlingo.LanguageIdentification.CLD2?includePreReleases=true)](https://www.nuget.org/packages/Panlingo.LanguageIdentification.CLD2/) |
| [Panlingo.LanguageIdentification.CLD3](./README_CLD3.md) |  [![NuGet Version](https://buildstats.info/nuget/Panlingo.LanguageIdentification.CLD3?includePreReleases=true)](https://www.nuget.org/packages/Panlingo.LanguageIdentification.CLD3/) |
| [Panlingo.LanguageIdentification.FastText](./README_FASTTEXT.md) |  [![NuGet Version](https://buildstats.info/nuget/Panlingo.LanguageIdentification.FastText?includePreReleases=true)](https://www.nuget.org/packages/Panlingo.LanguageIdentification.FastText/) |
| [Panlingo.LanguageIdentification.Whatlang](./README_WHATLANG.md) |  [![NuGet Version](https://buildstats.info/nuget/Panlingo.LanguageIdentification.Whatlang?includePreReleases=true)](https://www.nuget.org/packages/Panlingo.LanguageIdentification.Whatlang/) |
| [Panlingo.LanguageIdentification.MediaPipe](./README_MEDIAPIPE.md) |  [![NuGet Version](https://buildstats.info/nuget/Panlingo.LanguageIdentification.MediaPipe?includePreReleases=true)](https://www.nuget.org/packages/Panlingo.LanguageIdentification.MediaPipe/) |
| [Panlingo.LanguageIdentification.Lingua](./README_LINGUA.md) |  [![NuGet Version](https://buildstats.info/nuget/Panlingo.LanguageIdentification.Lingua?includePreReleases=true)](https://www.nuget.org/packages/Panlingo.LanguageIdentification.Lingua/) |
| [Panlingo.LanguageCode](./README_LANGUAGE_CODE.md) |  [![NuGet Version](https://buildstats.info/nuget/Panlingo.LanguageCode?includePreReleases=true)](https://www.nuget.org/packages/Panlingo.LanguageCode/) |

## Contents

1. [Models](#models)
    1. [CLD2](#cld2)
    2. [CLD3](#cld3)
    3. [FastText](#fasttext)
    4. [Whatlang](#whatlang)
    5. [MediaPipe](#mediapipe)
    6. [Lingua](#lingua)
2. [Features](#features)
3. [Platform Support](#platform-support)
4. [Key Concerns](#key-concerns)
5. [TODO](#todo)

## Models

### CLD2
- **Wrapper docs**: [Documentation](./README_CLD2.md)
- **Original source code**: [CLD2 Repository](https://github.com/CLD2Owners/cld2)

### CLD3
- **Wrapper docs**: [Documentation](./README_CLD3.md)
- **Original source code**: [CLD3 Repository](https://github.com/google/cld3)

### FastText
- **Wrapper docs**: [Documentation](./README_FASTTEXT.md)
- **Original source code**: [FastText Repository](https://github.com/facebookresearch/fastText)

### Whatlang
- **Wrapper docs**: [Documentation](./README_WHATLANG.md)
- **Original source code**: [Whatlang Repository](https://github.com/greyblake/whatlang-rs)

### MediaPipe
- **Wrapper docs**: [Documentation](./README_MEDIAPIPE.md)
- **Original source code**: [MediaPipe Repository](https://github.com/google-ai-edge/mediapipe)

### Lingua
- **Wrapper docs**: [Documentation](./README_LINGUA.md)
- **Original source code**: [MediaPipe Repository](https://github.com/pemistahl/lingua-rs)

## Key concerns

- Zero-dependency development. 
- The original code of libraries (CLD2, CLD3, FastText, Whatlang) is used as submodules without additional modifications or improvements. Third-party code is not included into this repository.
- Preserve the original library behavior without breaking changes.

## Features

| Feature                    | CLD2      | CLD3           | FastText*          | Whatlang | MediaPipe**    | Lingua   |
| :------------------------- | :-------: | :------------: | :----------------: | :------: | :------------: | :------: |
| Single language prediction | Yes       | Yes            | Yes                | Yes      | Yes            | Yes      |
| Multi language prediction  | Yes       | Yes            | Yes                | No       | Yes            | Yes      |
| Supported languages        | 80        | 107            | 176 or 217         | 69       | 110            | 75       |
| Unknown language detection | Yes       | Yes            | No                 | No       | Yes            | No       |
| Algorithm                  | quadgrams | neural network | neural network     | trigrams | neural network | trigrams |
| Script detection           | No        | No             | Yes (only lid218e) | Yes      | No             | No       |

\* When using these models: 
[lid176](https://fasttext.cc/docs/en/language-identification.html), 
[lid218e](https://huggingface.co/facebook/fasttext-language-identification)

\*\* When using [MediaPipe Language Detector](https://storage.googleapis.com/mediapipe-assets/LanguageDetector%20Model%20Card.pdf)

## Platform support

| Model     |        Linux       |     Windows      |  macOS |  Blazor WASM   |
| :-------- | :----------------: | :--------------: | :----: | :------------: |
| CLD2      | :white_check_mark: | :construction:   | :x:    | :x:            |
| CLD3      | :white_check_mark: | :construction:   | :x:    | :x:            |
| FastText  | :white_check_mark: | :construction:   | :x:    | :x:            |
| Whatlang  | :white_check_mark: | :construction:   | :x:    | :x:            |
| MediaPipe | :white_check_mark: | :construction:   | :x:    | :x:            |
| Lingua    | :white_check_mark: | :construction:   | :x:    | :x:            |

:white_check_mark: — Full support |
:x: — No support |
:construction: — Under research

## TODO

- [ ] Research support for other platforms (Windows, macOS).
- [ ] Enhance the suite of unit tests.
- [ ] Introduce more native methods (FastText).

---

Feel free to open issues or contribute to the repository. Together, let's enhance the .NET language identification capabilities! 🌐

---

Happy hacking! 👩‍💻👨‍💻