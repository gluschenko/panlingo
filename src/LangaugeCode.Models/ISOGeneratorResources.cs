using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Panlingo.LanguageCode.Core.Models
{
    public class ISOGeneratorResources
    {
        public IEnumerable<SetTwoLanguageDescriptor> SetTwoLanguageDescriptorList { get; set; }
        public IEnumerable<SetThreeLanguageDescriptor> SetThreeLanguageDescriptorList { get; set; }
        public IEnumerable<SetTwoLanguageDeprecationDescriptor> SetTwoLanguageDeprecationDescriptorList { get; set; }

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
