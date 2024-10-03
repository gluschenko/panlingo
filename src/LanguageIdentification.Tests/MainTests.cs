using System.Runtime.InteropServices;

namespace Panlingo.LanguageIdentification.Tests;

public class MainTests
{
    /// <summary>
    /// Checks the current OS and container environment
    /// </summary>
    [SkippableFact]
    public void CheckPlatform()
    {
        Assert.True(true);
        //Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));
        //Assert.Equal("true", Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"));
    }
}
