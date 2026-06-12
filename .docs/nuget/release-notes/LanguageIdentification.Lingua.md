0.8.0
- Add managed/native package version validation at detector builder startup
- Improve UTF-8 text handling by passing explicit byte lengths to the native layer
- Validate requested language enum values before creating a detector
- Serialize detector operations to avoid races with detection and disposal
- Improve native status handling for invalid detector pointers and enum values
- Add NuGet package icon and expanded package tags


0.6.3
- Fix potential SEGFAULT on LinguaDetector.PredictMixedLanguages
      
0.6.1
- Windows: static linking of Visual C++ Runtime
      
0.4.0.0
- Add GetLanguages method
      
0.2.0.0
- Windows and MacOS support
      
0.0.0.10:
- Initial release
