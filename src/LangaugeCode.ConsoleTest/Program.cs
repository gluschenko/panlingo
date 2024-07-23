using LangaugeCode.Core;

namespace LangaugeCode.ConsoleTest
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            LanguageCodeHelper.A();

            var extractor = new ISOExtractor(new HttpClient());

            var a = await extractor.ExtractAsync();

            ;
        }
    }
}
