namespace Panlingo.LanguageIdentification.Tests.Helpers;

public class FileHelper
{
    public static async Task DownloadAsync(string path, string url)
    {
        using var client = new HttpClient();
        using var stream = await client.GetStreamAsync(url);

        var directory = Path.GetDirectoryName(path) ?? throw new Exception("No directory");
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var file = new FileStream(path, FileMode.OpenOrCreate);
        await stream.CopyToAsync(file);
    }
}
