using Microsoft.Build.Construction;

namespace Panlingo.LanguageIdentification.Tests;

public class MainTests
{
    [Fact]
    public void CheckPackageVersion()
    {
        var types = new Type[] 
        {
            typeof(Panlingo.LanguageIdentification.CLD2.CLD2Detector),
            typeof(Panlingo.LanguageIdentification.CLD3.CLD3Detector),
            typeof(Panlingo.LanguageIdentification.FastText.FastTextDetector),
            typeof(Panlingo.LanguageIdentification.Lingua.LinguaDetector),
            typeof(Panlingo.LanguageIdentification.MediaPipe.MediaPipeDetector),
            typeof(Panlingo.LanguageIdentification.Whatlang.WhatlangDetector),
            typeof(Panlingo.LanguageIdentification.CLD2.Native.CLD2NativeLibrary),
            typeof(Panlingo.LanguageIdentification.CLD3.Native.CLD3NativeLibrary),
            typeof(Panlingo.LanguageIdentification.FastText.Native.FastTextNativeLibrary),
            typeof(Panlingo.LanguageIdentification.Lingua.Native.LinguaNativeLibrary),
            typeof(Panlingo.LanguageIdentification.MediaPipe.Native.MediaPipeNativeLibrary),
            typeof(Panlingo.LanguageIdentification.Whatlang.Native.WhatlangNativeLibrary),
        };

        var root = AppDomain.CurrentDomain.BaseDirectory;

        var src = root;
        while (src != "/")
        {
            if (Path.GetFileName(src) == "src")
            {
                break;
            }

            src = Path.GetDirectoryName(src) ?? "/";
        }

        var projectFiles = Directory.GetFiles(src, "*.csproj", SearchOption.AllDirectories);

        var packageProjects = new Dictionary<string, string>();

        foreach (var projectFile in projectFiles)
        {
            var projectRootElement = ProjectRootElement.Open(projectFile);
            var assemblyName = projectRootElement.Properties.FirstOrDefault(x => x.Name == "AssemblyName");
            var version = projectRootElement.Properties.FirstOrDefault(x => x.Name == "Version");

            if (assemblyName is null || version is null)
            {
                continue;
            }

            if (string.IsNullOrEmpty(assemblyName.Value) || string.IsNullOrEmpty(version.Value))
            {
                continue;
            }

            packageProjects[assemblyName.Value] = version.Value;
        }

        if (packageProjects.Count == 0)
        {
            throw new Exception("Projects are not found");
        }

        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x =>
            {
                foreach (var type in x.GetTypes())
                {
                    if (types.Contains(type))
                    {
                        return true;
                    }
                }

                return false;
            })
            .ToArray();

        var assemblyNames = new List<string>();
        var packageNames = new List<string>();

        foreach (var assembly in assemblies)
        {
            var assemblyName = assembly.GetName();

            assemblyNames.Add($"{assemblyName.Name} {assemblyName.Version}");
            if (assemblyName.Name != null && packageProjects.TryGetValue(assemblyName.Name, out var packageVersion))
            {
                var a = Version.Parse(packageVersion);
                var b = new Version(
                    major: a.Major != -1 ? a.Major : 0,
                    minor: a.Minor != -1 ? a.Minor : 0,
                    build: a.Build != -1 ? a.Build : 0,
                    revision: a.Revision != -1 ? a.Revision : 0
                );

                packageNames.Add($"{assemblyName.Name} {b}");
            }
        }

        Assert.Equal(packageNames, assemblyNames);
    }
}
