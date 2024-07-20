# Language Identification

Collection of language identification libraries for .NET

The main purpose of this repository is to provide popular language identification models to .NET

---
[![GitHub CI](https://github.com/gluschenko/language-identification/actions/workflows/github-ci.yml/badge.svg)](https://github.com/gluschenko/language-identification/actions/workflows/github-ci.yml)
---

## Models

### CLD2

* [Wrapper docs](./README_CLD2.md)
* [Original source code](https://github.com/CLD2Owners/cld2)

### CLD3

* [Wrapper docs](./README_CLD3.md)
* [Original source code](https://github.com/google/cld3)

### FastText

* [Wrapper docs](./README_FASTTEXT.md)
* [Original source code](https://github.com/facebookresearch/fastText)

### Whatlang

* [Wrapper docs](./README_WHATLANG.md)
* [Original source code](https://github.com/greyblake/whatlang-rs)

## Key concerns

❌ Do not use forks or modify the original code

✅ Make an extension for original code

❌ Do not break behaviour of original libraries



## Features

| # | CLD2 | CLD3 | FastText* | Whatlang |
| - | ---- | ---- | -------- | -------- |
| Single language prediction | Yes | Yes | Yes | Yes |
| Multi language prediction | Yes | Yes | Yes | No |
| Supported langauges | 80 | 107 | 176 or 217 | 69 |
| Unknown language detection | Yes | Yes | No | No |

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

* [ ] Research how to support another platforms 
* [ ] More unit-tests
* [ ] More native methods

