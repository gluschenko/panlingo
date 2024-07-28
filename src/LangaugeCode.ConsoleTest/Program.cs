using Panlingo.LangaugeCode.Generator;
using Panlingo.LangaugeCode.Core.Models;

namespace LangaugeCode.ConsoleTest
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var extractor = new ISOExtractor(new HttpClient());

            var a = await extractor.ExtractLangaugeCodesSetTwoAsync();
            var b = await extractor.ExtractLangaugeCodesSetThreeAsync();
            var c = await extractor.ExtractLanguageCodeDeprecationsSetTwoAsync();

            var aaa = new ISOGeneratorResources 
            {
                A = a,
                B = b,
                C = c,
            };

            var json = aaa.ToJson();

            var bbb = ISOGeneratorResources.FromJson(json);
            
            ;
        }
    }
}
