namespace Panlingo.LanguageIdentification.Tests;

public class MainTests
{
    /// <summary>
    /// Checks the current OS and container environment
    /// </summary>
    [SkippableFact]
    public void CheckPlatform()
    {
        Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Unix);
        Assert.Equal("true", Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"));
    }
}
