using System;
using System.Collections.Generic;
using System.Linq;
using Panlingo.LanguageCode.Models;

namespace Panlingo.LanguageCode
{
    internal static class LanguageCodeSearchIndex
    {
        public static readonly Dictionary<string, LanguageDescriptor> Langauges =
            new Dictionary<string, LanguageDescriptor>(StringComparer.InvariantCultureIgnoreCase);

        public static readonly Dictionary<string, IEnumerable<string>> LegacyCodes =
            new Dictionary<string, IEnumerable<string>>(StringComparer.InvariantCultureIgnoreCase);

        public static readonly Dictionary<string, string> MacrolanguageCodes =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        static LanguageCodeSearchIndex()
        {
            // Main langauge data
            foreach (var item in ISOGeneratorResourceProvider.ISOGeneratorResources.SetThreeLanguageDescriptorList)
            {
                if (!string.IsNullOrWhiteSpace(item.Id))
                {
                    Langauges[item.Id] = item;
                }

                if (!string.IsNullOrWhiteSpace(item.Part1))
                {
                    Langauges[item.Part1] = item;
                }

                if (!string.IsNullOrWhiteSpace(item.Part2t))
                {
                    Langauges[item.Part2t] = item;
                }

                if (!string.IsNullOrWhiteSpace(item.Part2b))
                {
                    Langauges[item.Part2b] = item;
                }
            }

            // Legacy langauges from ISO 639-1 & ISO 639-2
            foreach (var item in ISOGeneratorResourceProvider.ISOGeneratorResources.SetTwoLanguageDeprecationDescriptorList)
            {
                if (!string.IsNullOrWhiteSpace(item.CodeAlpha2) && !string.IsNullOrWhiteSpace(item.CodeAlpha2Deprecated))
                {
                    LegacyCodes[item.CodeAlpha2Deprecated] = new[] { item.CodeAlpha2 };
                }

                if (!string.IsNullOrWhiteSpace(item.CodeAlpha3) && !string.IsNullOrWhiteSpace(item.CodeAlpha3Deprecated))
                {
                    LegacyCodes[item.CodeAlpha3Deprecated] = new[] { item.CodeAlpha3 };
                }
            }

            // Legacy langauges from ISO 639-3
            foreach (var item in ISOGeneratorResourceProvider.ISOGeneratorResources.LegacyLanguageDescriptorList)
            {
                if (!string.IsNullOrWhiteSpace(item.Id) && item.ChangeTo.Any())
                {
                    LegacyCodes[item.Id] = item.ChangeTo;
                }
            }

            // Macrolangauges from ISO 639-3 (small groups of dialects)
            foreach (var item in ISOGeneratorResourceProvider.ISOGeneratorResources.MarcolanguageDescriptorList)
            {
                if (!string.IsNullOrWhiteSpace(item.Target) && !string.IsNullOrWhiteSpace(item.Source))
                {
                    MacrolanguageCodes[item.Target] = item.Source;
                }
            }

            // SYNTHETIC DATA

            // Moldovan is now Romanian:
            //
            // "The identifiers mo and mol are deprecated, leaving ro and ron (639-2/T)
            // and rum (639-2/B) the current language identifiers to be used for the variant
            // of the Romanian language also known as Moldavian and Moldovan in English
            // and moldave in French. The identifiers mo and mol will not be assigned to
            // different items, and recordings using these identifiers will not be invalid."
            //
            // source: https://www.loc.gov/standards/iso639-2/php/code_changes.php
            LegacyCodes["mo"] = new[] { "ro" };

            
        }
    }
}
