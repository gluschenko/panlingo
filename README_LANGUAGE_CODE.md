# Panlingo.LanguageCode

Panlingo.LanguageCode is a comprehensive .NET library designed for managing and converting language codes using various normalization and conversion rules. The library supports multiple language code entities, including two-letter ISO codes, three-letter ISO codes, and English names.

## What are Language Codes?

Language codes are standardized codes used to identify languages. The ISO 639 standard defines sets of codes for the representation of names of languages. There are several parts to the ISO 639 standard:
- **ISO 639-1**: Defines two-letter codes (e.g., "en" for English, "uz" for Uzbek);
- **ISO 639-2**: Defines three-letter codes (e.g., "eng" for English, "uzb" for Uzbek);
- **ISO 639-3**: Extends the code set to cover all known languages (e.g., "en" for English, "ru" for Russian).

These codes are essential in various applications, including software localization, data exchange between systems, and linguistic research.

## Features

- Convert language codes to their normalized forms.
- Resolve both two-letter and three-letter ISO codes.
- Handle deprecated and legacy language codes.
- Reduce specific language codes to their macrolanguage equivalents.
- Retrieve English names for language codes.
- Handle IETF language tags.
- Normalize inputs by converting to lowercase and trimming spaces.

## Installation

To install the Panlingo.LanguageCode library, you can use the .NET CLI:

```sh
dotnet add package Panlingo.LanguageCode
```

## Usage

You can use the library to convert and resolve language codes. Below are some examples to illustrate how to use the provided functionalities.

### Basic Configuration

Before using the library, set up a basic resolver with your desired rules:

```csharp
var resolver = new LanguageCodeResolver()
    .ToLowerAndTrim()
    .ConvertFromIETF()
    .ConvertFromDeprecatedCode()
    .ReduceToMacrolanguage();
```

### Convert a Single Language Code

You can convert a single language code to its two-letter or three-letter ISO equivalent:

```csharp
string twoLetterCode = LanguageCodeHelper.GetTwoLetterISOCode("rus");
// twoLetterCode => "ru"

string threeLetterCode = LanguageCodeHelper.GetThreeLetterISOCode("en");
// threeLetterCode => "eng"
```

### Resolve Language Codes with Options

You can resolve language codes using specific options, such as converting deprecated codes or reducing to macrolanguages:

```csharp
string resolvedCode = LanguageCodeHelper.Resolve("iw", resolver);
// resolvedCode => "heb"
```

### Handle Legacy Codes with Conflicts

For resolving legacy codes that may have conflicts, you can specify how to handle such conflicts:

```csharp
var options = resolver
    .ConvertFromDeprecatedCode((sourceCode, candidates) =>
    {
        return candidates.Any() ? candidates.First() : sourceCode;
    })
    .Select(LanguageCodeEntity.Alpha3);

string resolvedLegacyCode = LanguageCodeHelper.Resolve("mof", options);
// resolvedLegacyCode => "xpq"
```

### Retrieve English Names for Language Codes

You can also get the English names for language codes:

```csharp
string englishName = LanguageCodeHelper.GetLanguageEnglishName("ru");
// englishName => "Russian"
```

### Handle IETF Language Tags

You can resolve language codes from IETF language tags like "en-US":

```csharp
var options = new LanguageCodeResolver().ConvertFromIETF();
string resolvedIETFCode = LanguageCodeHelper.Resolve("en-US", options);
// resolvedIETFCode => "en"
```

### Normalize Input by Converting to Lowercase and Trimming Spaces

You can normalize inputs by converting to lowercase and trimming spaces:

```csharp
var options = new LanguageCodeResolver().ToLowerAndTrim();
string normalizedCode = LanguageCodeHelper.Resolve(" UK  ", options);
// normalizedCode => "uk"
```

## Contributing

Contributions and feedback are welcome! Feel free to submit issues, fork the repository, and send pull requests.
