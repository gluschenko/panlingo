# Panlingo.LanguageCode

Panlingo.LanguageCode is a comprehensive .NET library designed for managing and converting language codes using various normalization and conversion rules. The library supports multiple language code entities, including two-letter ISO codes, three-letter ISO codes, and English names.

## What are Language Codes?

Language codes are standardized codes used to identify languages. The ISO 639 standard defines sets of codes for the representation of names of languages. There are several parts to the ISO 639 standard:
- **ISO 639-1**: Defines two-letter codes (e.g., "en" for English, "uz" for Uzbek); [more](https://en.wikipedia.org/wiki/ISO_639-1).
- **ISO 639-2**: Defines three-letter codes (e.g., "eng" for English, "uzb" for Uzbek); [more](https://en.wikipedia.org/wiki/ISO_639-2).
- **ISO 639-3**: Extends the code set to cover all known languages and their [macrolanguages](https://en.wikipedia.org/wiki/ISO_639_macrolanguage) (e.g., "zho" is macrolangauge of "cmn"); [more](https://en.wikipedia.org/wiki/ISO_639-3).

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

### Language Code Resolver

You can use the library to convert and resolve language codes. 
Below are some examples to illustrate how to use the provided functionalities.

#### Basic Configuration

Before using the library, set up a basic resolver with your desired rules. 
Resolver is called `LanguageCodeResolver` and uses [Fluent Builder](https://en.wikipedia.org/wiki/Fluent_interface) design pattern:

```csharp
var resolver = new LanguageCodeResolver()
    .ToLowerAndTrim()            // optional
    .ConvertFromIETF()           // optional
    .ConvertFromDeprecatedCode() // optional
    .ReduceToMacrolanguage();    // optional
```

#### ISO 639-1 to ISO 639-2 or ISO 639-3 

In this code, we see how to convert a language code from the ISO 639-1 format (which uses two-letter codes) to the ISO 639-2 or ISO 639-3 format (which uses three-letter codes). Here’s the breakdown:

```csharp
var options = new LanguageCodeResolver()
    .Select(LanguageCodeEntity.Alpha3);

string result = LanguageCodeHelper.Resolve("uk", options);
// result => "ukr"
```

#### Resolve Langauge Code to it's English Name

This code demonstrates how to resolve a given language code to its corresponding English name using similar resolver configuration techniques:

```csharp
var options = new LanguageCodeResolver()
    .Select(LanguageCodeEntity.EnglishName);

string result = LanguageCodeHelper.Resolve("uk", options);
// result => "Ukrainian"
```

#### Resolve Dialect Language to it's Macrolanguage

The resolver is used to map a dialect language code to its corresponding macrolanguage code, then convert it to the Alpha-3 format:

```csharp
var options = new LanguageCodeResolver()
    .ReduceToMacrolanguage()
    .Select(LanguageCodeEntity.Alpha3);

string result = LanguageCodeHelper.Resolve("cmn", options);
// result => "zho"
```

#### Handle Legacy Codes with Conflicts

For resolving legacy codes that may have conflicts, you can specify how to handle such conflicts:

```csharp
var options = new LanguageCodeResolver()
    .ConvertFromDeprecatedCode((sourceCode, candidates) =>
    {
        return candidates.Any() ? candidates.First() : sourceCode;
    })
    .Select(LanguageCodeEntity.Alpha3);

string result = LanguageCodeHelper.Resolve("mof", options);
// result => "xpq"
```

#### Handle IETF Language Tags

You can resolve language codes from IETF language tags like "en-US":

```csharp
var options = new LanguageCodeResolver().ConvertFromIETF();

string result = LanguageCodeHelper.Resolve("en-US", options);
// result => "en"
```

#### Normalize Input by Converting to Lowercase and Trimming Spaces

You can normalize inputs by converting to lowercase and trimming spaces:

```csharp
var options = new LanguageCodeResolver().ToLowerAndTrim();

string result = LanguageCodeHelper.Resolve(" UK  ", options);
// result => "uk"
```

### Basic Methods

You can use these methods without any resolver configuration.

#### Convert a Single Language Code

You can convert a single language code to its two-letter or three-letter ISO equivalent:

```csharp
string result = LanguageCodeHelper.GetTwoLetterISOCode("rus");
// result => "ru"

string result = LanguageCodeHelper.GetThreeLetterISOCode("en");
// result => "eng"
```

#### Retrieve English Names for Language Codes

You can also get the English names for language codes:

```csharp
string result = LanguageCodeHelper.GetLanguageEnglishName("uk");
// result => "Ukrainian"
```

### Resolve Language Codes with Options

You can resolve language codes using specific options, such as converting deprecated codes or reducing to macrolanguages:

```csharp
string result = LanguageCodeHelper.Resolve("iw", resolver);
// result => "heb"
```

## Sources

 * [ISO 639 Home](https://www.iso.org/iso-639-language-code)
 * [ISO 639-2](https://www.loc.gov/standards/iso639-2/langhome.html)
 * [ISO 639-1 vs ISO 639-2](https://www.loc.gov/standards/iso639-2/php/code_changes.php)
 * [ISO 639-3](https://iso639-3.sil.org/code_tables/download_tables#639-3%20Code%20Set)

## Contributing

Contributions and feedback are welcome! Feel free to submit issues, fork the repository, and send pull requests.
