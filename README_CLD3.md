# Panlingo.LanguageIdentification.CLD3

This is a .NET wrapper for the Chrome Language Detection (CLD3) library by Google Inc.

This package facilitates the integration of language identification capabilities into .NET applications,
allowing for accurate and efficient recognition of over 107 languages with minimal effort.
Perfect for applications dealing with multilingual texts or requiring automatic language detection.

## Requirements

* .NET >=5.0
* Linux

## How to use?

### Installation

To use the CLD3 you have to install thos NuGet package:

```
dotnet add package Panlingo.LanguageIdentification.CLD3
```

Also requires to install Protobuf:
```
# Ubuntu/Debian
sudo apt -y update
sudo apt -y install protobuf-compiler libprotobuf-dev

# Fedora
sudo yum install protobuf-devel

# CentOS
sudo yum install epel-release
sudo yum install protobuf-devel
```

### API

TODO

## Alternatives

* [NikulovE/cld3.net](https://github.com/NikulovE/cld3.net) — unmaintained wrapper for .NET
* [uranium62/cld3-net](https://github.com/uranium62/cld3-net) — unmaintained wrapper for .NET
