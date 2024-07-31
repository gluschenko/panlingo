using System.Collections.Generic;

namespace Panlingo.LanguageCode.Models
{
    public class LegacyLanguageAlphaThreeDescriptor
    {
        public string Id { get; set; }
        public string RefName { get; set; }
        public string RetReason { get; set; }
        public IEnumerable<string> ChangeTo { get; set; }
        public string RetRemedy { get; set; }
        public string Effective { get; set; }
    }
}
