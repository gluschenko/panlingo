namespace Panlingo.LanguageIdentification.Tests;

public class MainTests
{
    /// <summary>
    /// Checks the current OS and container environment
    /// </summary>
    [Fact]
    public void CheckPlatform()
    {
        Assert.Equal(PlatformID.Unix, Environment.OSVersion.Platform);
        Assert.Equal("true", Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"));
    }
}
