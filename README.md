# Language Identification

[![GitHub CI](https://github.com/gluschenko/language-identification/actions/workflows/github-ci.yml/badge.svg)](https://github.com/gluschenko/language-identification/actions/workflows/github-ci.yml)

## Overview

Welcome to the **Language Identification** repository! 🚀

This project presents a comprehensive collection of language identification 
libraries for .NET. Its primary purpose is to bring popular 
language identification models to the .NET ecosystem, 
allowing developers to seamlessly integrate language detection 
functionality into their applications.

## Contents

1. [Models](#models)
    1. [CLD2](#cld2)
    2. [CLD3](#cld3)
    3. [FastText](#fasttext)
    4. [Whatlang](#whatlang)
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

## Key concerns

- ❌ Avoid forks or modifications of the original code.
- ✅ Extend the original code through wrappers.
- ❌ Preserve the original library behavior without breaking changes.

## Features

| # | CLD2 | CLD3 | FastText* | Whatlang |
| - | ---- | ---- | -------- | -------- |
| Single language prediction | Yes | Yes | Yes | Yes |
| Multi language prediction | Yes | Yes | Yes | No |
| Supported langauges | 80 | 107 | 176 or 217 | 69 |
| Unknown language detection | Yes | Yes | No | No |
| Algorithm | quadgrams | neural network | neural network | trigrams |

\* When using these models: 
[lid176](https://fasttext.cc/docs/en/language-identification.html), 
[lid218e](https://huggingface.co/facebook/fasttext-language-identification)

## Platform support

| Model    | Linux | Windows | macOS | WASM |
| -------- | ----- | ------- | ----- | ---- |
| CLD2     | ✅   | ❌ *   | ❌    | ❌ * |
| CLD3     | ✅   | ❌ *   | ❌    | ❌   |
| FastText | ✅   | ❌ *   | ❌    | ❌   |
| Whatlang | ✅   | ❌ *   | ❌    | ❌ * |

\* Ports for another platforms are planned

## TODO

- [ ] Research support for additional platforms.
- [ ] Enhance the suite of unit tests.
- [ ] Introduce more native methods.

---

Feel free to open issues or contribute to the repository. Together, let's enhance the .NET language identification capabilities! 🌐

---

Happy hacking! 👩‍💻👨‍💻