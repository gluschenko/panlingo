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
                SetThreeLanguageDescriptorList = await extractor.ExtractLanguageCodesSetThreeAsync(),
                LegacyLanguageAlphaTwoDescriptorList = await extractor.ExtractLanguageCodeDeprecationsSetTwoAsync(),
                MarcolanguageDescriptorList = await extractor.ExtractMarcolanguagesAsync(),
                LegacyLanguageAlphaThreeDescriptorList = await extractor.ExtractLegacyLanguagesAsync(),
            };

            var json = resources.ToJson();

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../LanguageCode/resources.json");

            File.WriteAllText(path, json);
        }
    }
}
