# Panlingo.LanguageIdentification.CLD3

This is a .NET wrapper for the Chrome Language Detection (CLD3) library by Google Inc.

This package facilitates the integration of language identification capabilities into .NET applications,
allowing for accurate and efficient recognition of over 107 languages with minimal effort.
Perfect for applications dealing with multilingual texts or requiring automatic language detection.

## Requirements

* .NET >=5.0
* Linux (Debian-like)

## How to use?

### Installation

To use the CLD3 you have to install thos NuGet package:

```
dotnet add package Panlingo.LanguageIdentification.CLD3
```

Also requires to install Protobuf:
```
sudo apt -y update
sudo apt -y install protobuf-compiler libprotobuf-dev
```

### API

TODO

## Alternatives

* [diadistis/cld2.net](https://github.com/diadistis/cld2.net) — unmaintained wrapper for .NET
* [nuvi/NCLD2](https://github.com/nuvi/NCLD2) — unmaintained wrapper for .NET ([NuGet](https://www.nuget.org/packages/NCLD2))
* [curiosity-ai/catalyst](https://github.com/curiosity-ai/catalyst) — contains CLD2 ([example here](https://github.com/curiosity-ai/catalyst/blob/master/samples/LanguageDetection/Program.cs))
