using Panlingo.LangaugeCode.Core;

namespace LangaugeCode.ConsoleTest
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var extractor = new ISOExtractor(new HttpClient());

            var a = await extractor.ExtractLangaugeCodesSetTwoAsync();
            var b = await extractor.ExtractLangaugeCodesSetThreeAsync();
            var c = await extractor.ExtractLanguageCodeChangesSetTwoAsync();
            
            ;
        }
    }
}
