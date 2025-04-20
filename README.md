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
5. [Contributing](#contributing)

## Models

| Model         | Authors              | License    | Original source code | Wrapper docs         |
| :------------ | :------------------- | :--------- | :------------------- | :------------------- |
| **CLD2**      | Google, Inc.         | Apache-2.0 | [@CLD2Owners/cld2](https://github.com/CLD2Owners/cld2) | [link](./README_CLD2.md) |
| **CLD3**      | Google, Inc.         | Apache-2.0 | [@google/cld3](https://github.com/google/cld3) | [link](./README_CLD3.md) |
| **FastText**  | Meta Platforms, Inc. | MIT        | [@facebookresearch/fastText](https://github.com/facebookresearch/fastText) | [link](./README_FASTTEXT.md) |
| **Whatlang**  | Serhii Potapov       | MIT        | [@greyblake/whatlang-rs](https://github.com/greyblake/whatlang-rs) | [link](./README_WHATLANG.md) |
| **MediaPipe** | Google, Inc.         | Apache-2.0 | [@google-ai-edge/mediapipe](https://github.com/google-ai-edge/mediapipe) | [link](./README_MEDIAPIPE.md) |
| **Lingua**    | Peter M. Stahl       | Apache-2.0 | [@pemistahl/lingua-rs](https://github.com/pemistahl/lingua-rs) | [link](./README_LINGUA.md) |

## Key concerns

- Zero-dependency development. 
- The original code of libraries (CLD2, CLD3, FastText, MediaPipe) is used as submodules without additional significant modifications or improvements (except for a small monkey-patching 😂). Third-party code is not included into this repository.
- Preserve the original library behavior without breaking changes.

## Features

| Feature                        | CLD2      | CLD3           | FastText*          | Whatlang | MediaPipe**    | Lingua   |
| :----------------------------- | :-------: | :------------: | :----------------: | :------: | :------------: | :------: |
| **Single language prediction** | Yes       | Yes            | Yes                | Yes      | Yes            | Yes      |
| **Multi language prediction**  | Yes       | Yes            | Yes                | No       | Yes            | Yes      |
| **Supported languages**        | 83        | 107            | 176 or 217         | 69       | 110            | 75       |
| **Unknown language detection** | Yes       | Yes            | No                 | No       | Yes            | No       |
| **Algorithm**                  | quadgrams | neural network | neural network     | trigrams | neural network | trigrams |
| **Script detection**           | No        | No             | Yes (only lid218e) | Yes      | No             | No       |
| **Written in**                 | C++       | C++            | C++                | Rust     | C++            | Rust     |

\* When using these models: 
[lid176](https://fasttext.cc/docs/en/language-identification.html), 
[lid218e](https://huggingface.co/facebook/fasttext-language-identification)

\*\* When using [MediaPipe Language Detector](https://storage.googleapis.com/mediapipe-assets/LanguageDetector%20Model%20Card.pdf)

## Platform support

| Model         |  Linux             | Windows            |  macOS             |
| :------------ | :----------------: | :----------------: | :----------------: |
| **CLD2**      | :white_check_mark: | :white_check_mark: | :white_check_mark: |
| **CLD3**      | :white_check_mark: | :white_check_mark: | :white_check_mark: |
| **FastText**  | :white_check_mark: | :white_check_mark: | :white_check_mark: |
| **Whatlang**  | :white_check_mark: | :white_check_mark: | :white_check_mark: |
| **MediaPipe** | :white_check_mark: | :x:                | :x:                |
| **Lingua**    | :white_check_mark: | :white_check_mark: | :white_check_mark:\* |

:white_check_mark: — Full support |
:x: — No support |
:construction: — Under research

\* **arm64** CPU only (Apple silicon M series)

## Contributing

We welcome contributions from developers of all skill levels. Whether you're fixing a bug, adding a new feature, or improving documentation, we appreciate your help in making this project better.

### Getting Started

To get started with contributing, follow these simple steps:

1. **Clone the Repository**

   First, clone the repository to your local machine with the following command:

   ```bash
   git clone --recurse-submodules --remote-submodules https://github.com/gluschenko/panlingo.git
   ```

2. **Create a Branch**

   Before you start making changes, create a new branch to keep your work organized. Use a descriptive name for your branch to make it easy to understand its purpose:

   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Make Changes**

   Now, you can make changes to the codebase. Please ensure your code follows our project's coding standards and includes relevant tests if applicable.

4. **Commit Your Changes**

   Once you've made your changes, commit them with a clear and informative commit message:

   ```bash
   git add .
   git commit -m "Add description of your changes"
   ```

5. **Push Your Changes**

   Push your branch to the remote repository:

   ```bash
   git push origin feature/your-feature-name
   ```

6. **Build**
   
   ```bash
   cd src/LanguageIdentification.FastText.Native
   dotnet build -c ReleaseLinuxOnly

   cd src/LanguageIdentification.FastText
   dotnet build -c ReleaseLinuxOnly
   ```

7. **Run**

   **Lunux:**
   ```bash
   dotnet run -c ReleaseLinuxOnly
   ```

   **Windows:**
   ```bash
   wsl -d Ubuntu -e bash -c "dotnet run -c ReleaseLinuxOnly"
   ```

7. **Open a Pull Request**

   Navigate to the repository on GitHub and open a pull request. Provide a detailed description of your changes and any additional information that might help reviewers understand your contribution.

### Review Process

After opening a pull request, it will be reviewed by one of the project maintainers. Feedback and suggestions might be provided to ensure the code meets our quality standards. Once approved, your changes will be merged into the main branch.

### Code of Conduct

Please note that this project adheres to a [Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code.

---

Happy hacking! 👩‍💻👨‍💻
