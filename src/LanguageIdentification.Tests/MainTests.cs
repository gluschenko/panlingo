namespace LanguageIdentification.Tests;

public class MainTests
{
    [Fact]
    public void CheckRunningUnix()
    {
        Assert.Equal(PlatformID.Unix, Environment.OSVersion.Platform);
        Assert.Equal("true", Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"));
    }
}
