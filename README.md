# Panlingo

[![GitHub CI](https://github.com/gluschenko/panlingo/actions/workflows/github-ci.yml/badge.svg)](https://github.com/gluschenko/panlingo/actions/workflows/github-ci.yml)

## Overview

Welcome to the **Panlingo** repository! 🚀

This project presents a comprehensive collection of language identification 
libraries for .NET. Its primary purpose is to bring popular 
language identification models to the .NET ecosystem, 
allowing developers to seamlessly integrate language detection 
functionality into their applications.

## Libraries

| Library | NuGet Package |
| :------ | :------------ |
| [Panlingo.LanguageIdentification.CLD2](./README_CLD2.md) |  [![NuGet Version](https://img.shields.io/nuget/vpre/Panlingo.LanguageIdentification.CLD2)](https://www.nuget.org/packages/Panlingo.LanguageIdentification.CLD2/) |
| [Panlingo.LanguageIdentification.CLD3](./README_CLD3.md) |  [![NuGet Version](https://img.shields.io/nuget/vpre/Panlingo.LanguageIdentification.CLD3)](https://www.nuget.org/packages/Panlingo.LanguageIdentification.CLD3/) |
| [Panlingo.LanguageIdentification.FastText](./README_FASTTEXT.md) |  [![NuGet Version](https://img.shields.io/nuget/vpre/Panlingo.LanguageIdentification.FastText)](https://www.nuget.org/packages/Panlingo.LanguageIdentification.FastText/) |
| [Panlingo.LanguageIdentification.Whatlang](./README_WHATLANG.md) |  [![NuGet Version](https://img.shields.io/nuget/vpre/Panlingo.LanguageIdentification.Whatlang)](https://www.nuget.org/packages/Panlingo.LanguageIdentification.Whatlang/) |
| [Panlingo.LanguageIdentification.MediaPipe](./README_MEDIAPIPE.md) |  [![NuGet Version](https://img.shields.io/nuget/vpre/Panlingo.LanguageIdentification.MediaPipe)](https://www.nuget.org/packages/Panlingo.LanguageIdentification.MediaPipe/) |
| [Panlingo.LanguageIdentification.Lingua](./README_LINGUA.md) |  [![NuGet Version](https://img.shields.io/nuget/vpre/Panlingo.LanguageIdentification.Lingua)](https://www.nuget.org/packages/Panlingo.LanguageIdentification.Lingua/) |
| [Panlingo.LanguageCode](./README_LANGUAGE_CODE.md) |  [![NuGet Version](https://img.shields.io/nuget/vpre/Panlingo.LanguageCode)](https://www.nuget.org/packages/Panlingo.LanguageCode/) |

## Contents

1. [Models](#models)
2. [Features](#features)
3. [Platform Support](#platform-support)
4. [Key Concerns](#key-concerns)
5. [TODO](#todo)

## Models

| Model     | Authors              | Original source code | Wrapper docs         |
| :-------- | :------------------- | :------------------- | :------------------- |
| CLD2      | Google, Inc.         | [@CLD2Owners/cld2](https://github.com/CLD2Owners/cld2) | [link](./README_CLD2.md) |
| CLD3      | Google, Inc.         | [@google/cld3](https://github.com/google/cld3) | [link](./README_CLD3.md) |
| FastText  | Meta Platforms, Inc. | [@facebookresearch/fastText](https://github.com/facebookresearch/fastText) | [link](./README_FASTTEXT.md) |
| Whatlang  | Serhii Potapov       | [@greyblake/whatlang-rs](https://github.com/greyblake/whatlang-rs) | [link](./README_WHATLANG.md) |
| MediaPipe | Google, Inc.         | [@google-ai-edge/mediapipe](https://github.com/google-ai-edge/mediapipe) | [link](./README_MEDIAPIPE.md) |
| Lingua    | Peter M. Stahl       | [@pemistahl/lingua-rs](https://github.com/pemistahl/lingua-rs) | [link](./README_LINGUA.md) |

## Key concerns

- Zero-dependency development. 
- The original code of libraries (CLD2, CLD3, FastText, MediaPipe) is used as submodules without additional significant modifications or improvements (except for a small monkey-patching 😂). Third-party code is not included into this repository.
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
| Written in                 | C++       | C++            | C++                | Rust     | C++            | Rust     |

\* When using these models: 
[lid176](https://fasttext.cc/docs/en/language-identification.html), 
[lid218e](https://huggingface.co/facebook/fasttext-language-identification)

\*\* When using [MediaPipe Language Detector](https://storage.googleapis.com/mediapipe-assets/LanguageDetector%20Model%20Card.pdf)

## Platform support

| Model     |  Linux             | Windows            |  macOS             |
| :-------- | :----------------: | :----------------: | :----------------: |
| CLD2      | :white_check_mark: | :white_check_mark: | :white_check_mark: |
| CLD3      | :white_check_mark: | :white_check_mark: | :construction:     |
| FastText  | :white_check_mark: | :white_check_mark: | :white_check_mark: |
| Whatlang  | :white_check_mark: | :white_check_mark: | :white_check_mark: |
| MediaPipe | :white_check_mark: | :x:                | :x:                |
| Lingua    | :white_check_mark: | :white_check_mark: | :white_check_mark:\* |

:white_check_mark: — Full support |
:x: — No support |
:construction: — Under research

\* **arm64** CPU only (Apple silicon M series)

## TODO

- [x] Research support for other platforms (Windows, macOS).
- [ ] Increase unit testing coverage.
- [ ] Implement more native methods (FastText).
- [x] Self-contained models (FastText + MediaPipe).
- [x] Remove protobuf dependency (CLD3).

---

Feel free to open issues or contribute to the repository. Together, let's enhance the .NET language identification capabilities! 🌐

---

Happy hacking! 👩‍💻👨‍💻
