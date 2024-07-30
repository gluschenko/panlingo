using System;
using System.Collections.Generic;
using Panlingo.LanguageCode.Models;

namespace Panlingo.LanguageCode
{
    internal static class LanguageCodeSearchIndex
    {
        public static readonly Dictionary<string, SetThreeLanguageDescriptor> Langauges =
            new Dictionary<string, SetThreeLanguageDescriptor>(StringComparer.InvariantCultureIgnoreCase);

        public static readonly Dictionary<string, string> LegacyCodes =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public static readonly Dictionary<string, string> MacrolanguageCodes =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        static LanguageCodeSearchIndex()
        {
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

            foreach (var item in ISOGeneratorResourceProvider.ISOGeneratorResources.SetTwoLanguageDeprecationDescriptorList)
            {
                if (!string.IsNullOrWhiteSpace(item.CodeAlpha2) && !string.IsNullOrWhiteSpace(item.CodeAlpha2Deprecated))
                {
                    LegacyCodes[item.CodeAlpha2Deprecated] = item.CodeAlpha2;
                }

                if (!string.IsNullOrWhiteSpace(item.CodeAlpha3) && !string.IsNullOrWhiteSpace(item.CodeAlpha3Deprecated))
                {
                    LegacyCodes[item.CodeAlpha3Deprecated] = item.CodeAlpha3;
                }
            }

            foreach (var item in ISOGeneratorResourceProvider.ISOGeneratorResources.LegacyLanguageDescriptorList)
            {
                if (!string.IsNullOrWhiteSpace(item.Id) && !string.IsNullOrWhiteSpace(item.ChangeTo))
                {
                    LegacyCodes[item.Id] = item.ChangeTo;
                }
            }

            // https://www.loc.gov/standards/iso639-2/php/code_changes.php
            LegacyCodes["mo"] = "ro";

            foreach (var item in ISOGeneratorResourceProvider.ISOGeneratorResources.MarcolanguageDescriptorList)
            {
                if (!string.IsNullOrWhiteSpace(item.Target) && !string.IsNullOrWhiteSpace(item.Source))
                {
                    MacrolanguageCodes[item.Target] = item.Source;
                }
            }
        }
    }
}
