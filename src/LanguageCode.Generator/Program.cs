using Panlingo.LanguageCode.Models;

namespace LanguageCode.Generator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var extractor = new ISOExtractor(new HttpClient());

            var resources = new ISOGeneratorResources
            {
                SetTwoLanguageDescriptorList = await extractor.ExtractLanguageCodesSetTwoAsync(),
                SetThreeLanguageDescriptorList = await extractor.ExtractLanguageCodesSetThreeAsync(),
                SetTwoLanguageDeprecationDescriptorList = await extractor.ExtractLanguageCodeDeprecationsSetTwoAsync(),
                MarcolanguageDescriptorList = await extractor.ExtractMarcolanguagesAsync(),
                LegacyLanguageDescriptorList = await extractor.ExtractLegacyLanguagesAsync(),
            };

            var json = resources.ToJson();

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../LanguageCode/resources.json");

            File.WriteAllText(path, json);
        }
    }
}
