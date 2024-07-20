# Language Identification


Collection of language identification libraries for .NET

[![GitHub CI](https://github.com/gluschenko/language-identification/actions/workflows/github-ci.yml/badge.svg)](https://github.com/gluschenko/language-identification/actions/workflows/github-ci.yml)


## Features

| # | CLD2 | CLD3 | FastText* | Whatlang |
| - | ---- | ---- | -------- | -------- |
| Single language prediction | Yes | Yes | Yes | Yes |
| Multi language prediction | Yes | Yes | Yes | No |
| Supported langauges | ? | ? | ? | ? |

\* When using these models: [1](https://fasttext.cc/docs/en/language-identification.html), [2](https://huggingface.co/facebook/fasttext-language-identification)

## Platform support

| Model    | Linux | Windows | macOS | WASM |
| -------- | ----- | ------- | ----- | ---- |
| CLD2     | ✅   | ❌ *   | ❌    | ❌ * |
| CLD3     | ✅   | ❌ *   | ❌    | ❌   |
| FastText | ✅   | ❌ *   | ❌    | ❌   |
| Whatlang | ✅   | ❌ *   | ❌    | ❌ * |

\* Ports for another planforms are planned

## Project structure



## TODO



### CLD3

* [GitHub](https://github.com/google/cld3)
* [Python bindings](https://github.com/google/cld3/tree/master/gcld3)

### CLD2

* [GitHub](https://github.com/CLD2Owners/cld2)
* [Python bindings](https://pypi.org/project/pycld2/)

### FastText

* [GitHub](https://github.com/facebookresearch/fastText)
* [Python bindings](https://pypi.org/project/fasttext/)
* [Model](https://huggingface.co/facebook/fasttext-language-identification)

### WhatLang

* [GitHub](https://github.com/greyblake/whatlang-rs)
* [Python bindings](https://github.com/cathalgarvey/whatlang-py)
