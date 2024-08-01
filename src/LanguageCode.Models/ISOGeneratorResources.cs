using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Panlingo.LanguageCode.Models
{
    public class ISOGeneratorResources
    {
        public IEnumerable<LanguageDescriptor> SetThreeLanguageDescriptorList { get; set; }
        public IEnumerable<MarcolanguageDescriptor> MarcolanguageDescriptorList { get; set; }
        public IEnumerable<LegacyLanguageAlphaTwoDescriptor> LegacyLanguageAlphaTwoDescriptorList { get; set; }
        public IEnumerable<LegacyLanguageAlphaThreeDescriptor> LegacyLanguageAlphaThreeDescriptorList { get; set; }

        public string ToJson()
        {
            var json = JsonSerializer.Serialize(
                value: this,
                options: new JsonSerializerOptions
                {
                    WriteIndented = true,
                }
            );

            return json;
        }

        public static ISOGeneratorResources FromJson(string json)
        {
            return JsonSerializer.Deserialize<ISOGeneratorResources>(json)
                ?? throw new Exception($"Failed to decode {nameof(ISOGeneratorResources)}");
        }
    }
}
