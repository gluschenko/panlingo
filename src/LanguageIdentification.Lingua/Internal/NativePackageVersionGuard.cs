using System;
using System.Reflection;

namespace Panlingo.LanguageIdentification.Lingua.Internal
{
    internal static class NativePackageVersionGuard
    {
        public static void EnsureMatches(Assembly managedAssembly, Assembly nativeAssembly)
        {
            var managedVersion = GetPackageVersion(managedAssembly);
            var nativeVersion = GetPackageVersion(nativeAssembly);

            if (!StringComparer.Ordinal.Equals(managedVersion, nativeVersion))
            {
                throw new InvalidOperationException(
                    $"Native package version mismatch: {managedAssembly.GetName().Name} {managedVersion} requires {nativeAssembly.GetName().Name} {managedVersion}, but {nativeVersion} is loaded."
                );
            }
        }

        private static string GetPackageVersion(Assembly assembly)
        {
            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? assembly.GetName().Version?.ToString()
                ?? string.Empty;

            var metadataIndex = version.IndexOf('+');
            return metadataIndex >= 0 ? version.Substring(0, metadataIndex) : version;
        }
    }
}
