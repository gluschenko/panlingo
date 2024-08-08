using System;
using System.Collections.Generic;
using System.Linq;
using Panlingo.LanguageCode.Models;

namespace Panlingo.LanguageCode
{
    internal static class LanguageCodeSearchIndex
    {
        public static readonly Dictionary<string, LanguageDescriptor> Languages =
            new Dictionary<string, LanguageDescriptor>(StringComparer.InvariantCultureIgnoreCase);

        public static readonly Dictionary<string, IEnumerable<string>> LegacyCodes =
            new Dictionary<string, IEnumerable<string>>(StringComparer.InvariantCultureIgnoreCase);

        public static readonly Dictionary<string, string> MacrolanguageCodes =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        static LanguageCodeSearchIndex()
        {
            // Main language data
            foreach (var item in ISOGeneratorResourceProvider.ISOGeneratorResources.SetThreeLanguageDescriptorList)
            {
                if (!string.IsNullOrWhiteSpace(item.Id))
                {
                    Languages[item.Id] = item;
                }

                if (!string.IsNullOrWhiteSpace(item.Part1))
                {
                    Languages[item.Part1] = item;
                }

                if (!string.IsNullOrWhiteSpace(item.Part2t))
                {
                    Languages[item.Part2t] = item;
                }

                if (!string.IsNullOrWhiteSpace(item.Part2b))
                {
                    Languages[item.Part2b] = item;
                }
            }

            // Legacy languages from ISO 639-1 & ISO 639-2
            foreach (var item in ISOGeneratorResourceProvider.ISOGeneratorResources.LegacyLanguageAlphaTwoDescriptorList)
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

            // Legacy languages from ISO 639-3
            foreach (var item in ISOGeneratorResourceProvider.ISOGeneratorResources.LegacyLanguageAlphaThreeDescriptorList)
            {
                if (!string.IsNullOrWhiteSpace(item.Id) && item.ChangeTo.Any())
                {
                    LegacyCodes[item.Id] = item.ChangeTo;
                }
            }

            // Macrolanguages from ISO 639-3 (small groups of dialects)
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

            foreach (var item in ISOGeneratorResourceProvider.ISOGeneratorResources.LegacyLanguageAlphaThreeDescriptorList)
            {
                if (!string.IsNullOrWhiteSpace(item.Id) && !Languages.ContainsKey(item.Id))
                {
                    Languages[item.Id] = new LanguageDescriptor
                    {
                        Id = item.Id,
                        Part2b = string.Empty,
                        Part2t = string.Empty,
                        Part1 = string.Empty,
                        LanguageType = "SYNTHETIC",
                        RefName = item.RefName,
                        Comment = string.Empty,
                    };

                    if (item.Id == "mol")
                    {
                        Languages["mo"] = Languages[item.Id];
                    }
                }
            }

            foreach (var item in ISOGeneratorResourceProvider.ISOGeneratorResources.LegacyLanguageAlphaTwoDescriptorList)
            {
                if (!string.IsNullOrWhiteSpace(item.CodeAlpha2Deprecated) && !string.IsNullOrWhiteSpace(item.CodeAlpha3))
                {
                    var actualItem = Languages[item.CodeAlpha3];

                    if (!Languages.ContainsKey(item.CodeAlpha2Deprecated))
                    {
                        Languages[item.CodeAlpha2Deprecated] = actualItem;
                    }
                }
            }

            Languages.TrimExcess();
            LegacyCodes.TrimExcess();
            MacrolanguageCodes.TrimExcess();
        }
    }
}
