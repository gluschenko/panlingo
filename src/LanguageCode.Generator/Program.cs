using Panlingo.LanguageCode.Models;

namespace Panlingo.LanguageCode.Generator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var extractor = new ISOExtractor(new HttpClient());

            var languageSet3 = await extractor.ExtractLanguageCodesSetThreeAsync();
            var languageSet2 = await extractor.ExtractLanguageCodesSetTwoAsync();

            var languageSetDiffQuery =
                from x in languageSet2
                join y in languageSet3 on x.Part2b equals y.Part2b into left
                from y in left.DefaultIfEmpty()
                where y is null
                select x;

            var languageSetDiff = languageSetDiffQuery.ToArray();

            var languageDescriptorList = languageSet3.Union(languageSetDiff).ToArray();

            var resources = new ISOGeneratorResources
            {
                LanguageDescriptorList = languageDescriptorList,
                MarcolanguageDescriptorList = await extractor.ExtractMarcolanguagesAsync(),
                LegacyLanguageAlphaTwoDescriptorList = await extractor.ExtractLanguageCodeDeprecationsSetTwoAsync(),
                LegacyLanguageAlphaThreeDescriptorList = await extractor.ExtractLegacyLanguagesAsync(),
            };

            var json = resources.ToJson();

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../LanguageCode/resources.json");

            File.WriteAllText(path, json);
        }
    }
}
