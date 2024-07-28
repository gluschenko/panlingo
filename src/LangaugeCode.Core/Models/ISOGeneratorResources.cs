using System;
using System.Collections.Generic;
using System.Text.Json;
using Panlingo.LangaugeCode.Core.Models;
using Panlingo.LanguageCode.Core.Models;

namespace Panlingo.LangaugeCode.Core.Models
{
    public class ISOGeneratorResources
    {
        public IEnumerable<SetTwoLanguageDescriptor> A { get; set; }
        public IEnumerable<SetThreeLanguageDescriptor> B { get; set; }
        public IEnumerable<SetTwoLanguageDeprecationDescriptor> C { get; set; }

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
            return System.Text.Json.JsonSerializer.Deserialize<ISOGeneratorResources>(json)
                ?? throw new Exception($"Failed to decode {nameof(ISOGeneratorResources)}");
        }
    }
}
