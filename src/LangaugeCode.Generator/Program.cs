using Panlingo.LangaugeCode.Generator;
using Panlingo.LanguageCode.Core.Models;

namespace LangaugeCode.Generator
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
                SetTwoLanguageDescriptorList = a,
                SetThreeLanguageDescriptorList = b,
                SetTwoLanguageDeprecationDescriptorList = c,
            };

            var json = aaa.ToJson();

            var bbb = ISOGeneratorResources.FromJson(json);

            ;
        }
    }
}
