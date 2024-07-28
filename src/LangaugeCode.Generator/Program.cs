using Panlingo.LanguageCode.Core.Models;

namespace LangaugeCode.Generator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var extractor = new ISOExtractor(new HttpClient());

            var resources = new ISOGeneratorResources
            {
                SetTwoLanguageDescriptorList = await extractor.ExtractLangaugeCodesSetTwoAsync(),
                SetThreeLanguageDescriptorList = await extractor.ExtractLangaugeCodesSetThreeAsync(),
                SetTwoLanguageDeprecationDescriptorList = await extractor.ExtractLanguageCodeDeprecationsSetTwoAsync(),
            };

            var json = resources.ToJson();

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../LangaugeCode.SourceGenerator/resources.json");

            File.WriteAllText(path, json);
        }
    }
}
