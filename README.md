# Language Identification

Collection of language identification libraries for .NET

Main purpose of this repository is adaptation of 

[![GitHub CI](https://github.com/gluschenko/language-identification/actions/workflows/github-ci.yml/badge.svg)](https://github.com/gluschenko/language-identification/actions/workflows/github-ci.yml)

## Models

### CLD2

* [Wrapper docs](./README_CLD2.md)
* [Original repo](https://github.com/CLD2Owners/cld2)

### CLD3

* [Wrapper docs](./README_CLD3.md)
* [Original repo](https://github.com/google/cld3)

### FastText

* [Wrapper docs](./README_FASTTEXT.md)
* [Original repo](https://github.com/facebookresearch/fastText)

### Whatlang

* [Wrapper docs](./README_WHATLANG.md)
* [Original repo](https://github.com/greyblake/whatlang-rs)

## Features

| # | CLD2 | CLD3 | FastText* | Whatlang |
| - | ---- | ---- | -------- | -------- |
| Single language prediction | Yes | Yes | Yes | Yes |
| Multi language prediction | Yes | Yes | Yes | No |
| Supported langauges | ? | ? | ? | ? |
| Unknown language detection | Yes | Yes | No | No |

\* When using these models: 
[1](https://fasttext.cc/docs/en/language-identification.html), 
[2](https://huggingface.co/facebook/fasttext-language-identification)

## Platform support

| Model    | Linux | Windows | macOS | WASM |
| -------- | ----- | ------- | ----- | ---- |
| CLD2     | ✅   | ❌ *   | ❌    | ❌ * |
| CLD3     | ✅   | ❌ *   | ❌    | ❌   |
| FastText | ✅   | ❌ *   | ❌    | ❌   |
| Whatlang | ✅   | ❌ *   | ❌    | ❌ * |

\* Ports for another platforms are planned

## TODO

* [ ] Research hot to support another platforms 
* [ ] More unit-tests
* [ ] More native methods

