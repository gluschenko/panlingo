0.8.0
- Add managed/native package version validation at detector startup
- Improve UTF-8 text handling by passing explicit byte lengths to the native layer
- Serialize detector operations to avoid races with model access and disposal
- Return an empty result for zero-count predictions
- Add NuGet package icon and expanded package tags


0.7.0
- Windows and Linux: ARM64 support      
      
0.6.1
- Windows: static linking of Visual C++ Runtime

0.1.0.0
- Windows and MacOS support

0.0.0.21:
- Default FastText model is included in NuGet package

0.0.0.10:
- Initial release
