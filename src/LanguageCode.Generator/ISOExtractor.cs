using System;
using System.Text;
using HtmlAgilityPack;
using Panlingo.LanguageCode.Models;

namespace LanguageCode.Generator
{
    /// <summary>
    /// # ISO Home: 
    /// https://www.iso.org/iso-639-language-code
    /// 
    /// # ISO 639-2:
    /// https://www.loc.gov/standards/iso639-2/langhome.html
    /// How to get code dataset: find the link "Code list for downloading" -> "Character encoding in UTF-8"
    /// https://www.loc.gov/standards/iso639-2/ISO-639-2_utf-8.txt
    ///
    /// # ISO 639-1 vs ISO 639-2
    /// https://www.loc.gov/standards/iso639-2/php/code_changes.php
    ///
    /// # ISO 639-3
    /// https://iso639-3.sil.org/code_tables/download_tables#639-3%20Code%20Set
    /// 
    /// https://iso639-3.sil.org/sites/iso639-3/files/downloads/iso-639-3.tab
    /// https://iso639-3.sil.org/sites/iso639-3/files/downloads/iso-639-3_Name_Index.tab
    /// https://iso639-3.sil.org/sites/iso639-3/files/downloads/iso-639-3_Retirements.tab
    /// https://iso639-3.sil.org/sites/iso639-3/files/downloads/iso-639-3-macrolanguages.tab
    /// 
    /// </summary>
    public class ISOExtractor
    {
        private readonly HttpClient _httpClient;

        public ISOExtractor(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Source: https://iso639-3.sil.org/code_tables/download_tables
        /// 
        /// Format:
        /// <code>
        /// CREATE TABLE [ISO_639-3] (
        /// Id char (3) NOT NULL,           -- The three-letter 639-3 identifier
        /// Part2B char (3) NULL,           -- Equivalent 639-2 identifier of the bibliographic applications 
        ///                                 -- code set, if there is one
        /// Part2T char (3) NULL,           -- Equivalent 639-2 identifier of the terminology applications code 
        ///                                 -- set, if there is one
        /// Part1 char (2) NULL,            -- Equivalent 639-1 identifier, if there is one
        /// Scope char (1) NOT NULL,        -- I(ndividual), M(acrolanguage), S(pecial)
        /// Type char (1) NOT NULL,         -- A(ncient), C(onstructed),  
        ///                                 -- E(xtinct), H(istorical), L(iving), S(pecial)
        /// Ref_Name varchar(150) NOT NULL, -- Reference language name
        /// Comment varchar(150) NULL)      -- Comment relating to one or more of the columns
        /// </code>
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<LanguageDescriptor>> ExtractLanguageCodesSetThreeAsync(
            string baseUrl = "https://iso639-3.sil.org/sites/iso639-3/files/downloads/iso-639-3.tab",
            CancellationToken token = default
        )
        {
            var result = new List<LanguageDescriptor>();

            var response = await _httpClient.GetStringAsync(baseUrl, token);
            response = Encoding.UTF8.GetString(Encoding.Default.GetBytes(response));

            var lines = response.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0)
                .ToArray();

            foreach (var line in lines)
            {
                var lineArray = line.Split(new[] { '\t' });

                // Skip file header
                if (lineArray[0] == "Id" && lineArray[1] == "Part2b")
                {
                    continue;
                }

                result.Add(new LanguageDescriptor
                {
                    Id = lineArray.Length > 0 ? lineArray[0].Trim() : string.Empty,
                    Part2b = lineArray.Length > 1 ? lineArray[1].Trim() : string.Empty,
                    Part2t = lineArray.Length > 2 ? lineArray[2].Trim() : string.Empty,
                    Part1 = lineArray.Length > 3 ? lineArray[3].Trim() : string.Empty,
                    Scope = lineArray.Length > 4 ? lineArray[4].Trim() : string.Empty,
                    LanguageType = lineArray.Length > 5 ? lineArray[5].Trim() : string.Empty,
                    RefName = lineArray.Length > 6 ? lineArray[6].Trim() : string.Empty,
                    Comment = lineArray.Length > 7 ? lineArray[7].Trim() : string.Empty,
                });
            }

            return result;
        }

        /// <summary>
        /// Source: https://www.loc.gov/standards/iso639-2/php/code_changes.php
        /// 
        /// Deprecated codes are listed inside brackets [] with a hyphen preceding the code.
        /// 
        /// Categories of change key: Add = Newly added; Dep = Deprecated; CC = Code change; NC = Name change; 
        /// NA = Variant name(s) added.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LegacyLanguageAlphaTwoDescriptor>> ExtractLanguageCodeDeprecationsSetTwoAsync(
            string baseUrl = "https://www.loc.gov/standards/iso639-2/php/code_changes.php",
            CancellationToken token = default
        )
        {
            KeyValuePair<string, string> ParseLangaugePair(string text)
            {
                text = text.Replace("&nbsp;", "").Trim();

                var actual = "";
                var deprecated = "";

                var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var word in words)
                {
                    if (word.StartsWith("[-") && word.EndsWith("]"))
                    {
                        deprecated = word.Replace("[", "").Replace("]", "").Replace("-", "");
                    }
                    else
                    {
                        actual = word;
                    }
                }

                return new KeyValuePair<string, string>(actual, deprecated);
            }

            var result = new List<LegacyLanguageAlphaTwoDescriptor>();

            var response = await _httpClient.GetStringAsync(baseUrl, token);
            response = Encoding.UTF8.GetString(Encoding.Default.GetBytes(response));

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            var tables = htmlDoc.DocumentNode.Descendants("table");

            var table = tables.ElementAt(1);

            var rows = table.Descendants("tr");

            foreach (var row in rows)
            {
                var cells = row.Descendants("td");

                var rowTexts = new List<string>();

                foreach (var cell in cells)
                {
                    var text = cell.InnerText.Trim();

                    if (text == "&nbsp;")
                    {
                        text = string.Empty;
                    }

                    if (text == "(none)")
                    {
                        text = string.Empty;
                    }

                    rowTexts.Add(text);
                }

                if (!rowTexts.Any())
                {
                    continue;
                }

                var alpha2 = rowTexts[0];
                var alpha3 = rowTexts[1];
                var englishName = rowTexts[2];
                var categoryOfChange = rowTexts[5];
                var comment = rowTexts[6];

                // We only need desrecations and code changes
                if (!categoryOfChange.Equals("Dep", StringComparison.OrdinalIgnoreCase) &&
                    !categoryOfChange.Equals("CC", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var a = ParseLangaugePair(alpha2);
                var b = ParseLangaugePair(alpha3);

                result.Add(new LegacyLanguageAlphaTwoDescriptor
                {
                    CodeAlpha2 = a.Key,
                    CodeAlpha2Deprecated = a.Value,
                    CodeAlpha3 = b.Key,
                    CodeAlpha3Deprecated = b.Value,
                    CategoryOfChange = categoryOfChange,
                    EnglishName = englishName,
                    Comment = comment,
                });
            }

            return result;
        }

        /// <summary>
        /// Source: https://iso639-3.sil.org/sites/iso639-3/files/downloads/iso-639-3-macrolanguages.tab
        /// 
        /// CREATE TABLE [ISO_639-3_Macrolanguages] (
        /// M_Id char (3) NOT NULL,      -- The identifier for a macrolanguage
        /// I_Id char (3) NOT NULL,      -- The identifier for an individual language
        ///                              -- that is a member of the macrolanguage
        /// I_Status char (1) NOT NULL)  -- A(active) or R(retired) indicating the
        ///                              -- status of the individual code element
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<MarcolanguageDescriptor>> ExtractMarcolanguagesAsync(
            string baseUrl = "https://iso639-3.sil.org/sites/iso639-3/files/downloads/iso-639-3-macrolanguages.tab",
            CancellationToken token = default
        )
        {
            var result = new List<MarcolanguageDescriptor>();

            var response = await _httpClient.GetStringAsync(baseUrl, token);
            response = Encoding.UTF8.GetString(Encoding.Default.GetBytes(response));

            var lines = response.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0)
                .ToArray();

            foreach (var line in lines)
            {
                var lineArray = line.Split(new[] { '\t' });

                // Skip file header
                if (lineArray[0] == "M_Id" && lineArray[1] == "I_Id")
                {
                    continue;
                }

                result.Add(new MarcolanguageDescriptor
                {
                    Source = lineArray.Length > 0 ? lineArray[0].Trim() : string.Empty,
                    Target = lineArray.Length > 1 ? lineArray[1].Trim() : string.Empty,
                    Status = lineArray.Length > 2 ? lineArray[2].Trim() : string.Empty,
                });
            }

            return result;
        }

        /// <summary>
        /// Source: https://iso639-3.sil.org/sites/iso639-3/files/downloads/iso-639-3_Retirements.tab
        /// 
        /// CREATE TABLE [ISO_639-3_Retirements] (
        /// Id char (3)      NOT NULL,        -- The three-letter 639-3 identifier
        /// Ref_Name varchar(150) NOT NULL,   -- reference name of language
        /// Ret_Reason char (1)      NOT NULL,-- code for retirement: C(change), D(duplicate),
        ///                                   -- N(non-existent), S(split), M(merge)
        /// Change_To char (3)      NULL,     -- in the cases of C, D, and M, the identifier 
        ///                                   -- to which all instances of this Id should be changed
        /// Ret_Remedy varchar(300) NULL,     -- The instructions for updating an instance
        ///                                   -- of the retired (split) identifier
        /// Effective date         NOT NULL)  -- The date the retirement became effective
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<LegacyLanguageAlphaThreeDescriptor>> ExtractLegacyLanguagesAsync(
            string baseUrl = "https://iso639-3.sil.org/sites/iso639-3/files/downloads/iso-639-3_Retirements.tab",
            CancellationToken token = default
        )
        {
            var result = new List<LegacyLanguageAlphaThreeDescriptor>();

            var response = await _httpClient.GetStringAsync(baseUrl, token);
            response = Encoding.UTF8.GetString(Encoding.Default.GetBytes(response));

            var lines = response.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0)
                .ToArray();

            foreach (var line in lines)
            {
                var lineArray = line.Split(new[] { '\t' });

                // Skip file header
                if (lineArray[0] == "Id" && lineArray[1] == "Ref_Name")
                {
                    continue;
                }

                result.Add(new LegacyLanguageAlphaThreeDescriptor
                {
                    Id = lineArray.Length > 0 ? lineArray[0].Trim() : string.Empty,
                    RefName = lineArray.Length > 1 ? lineArray[1].Trim() : string.Empty,
                    RetReason = lineArray.Length > 2 ? lineArray[2].Trim() : string.Empty,
                    ChangeTo = lineArray.Length > 3 ? lineArray[3].Trim() : string.Empty,
                    RetRemedy = lineArray.Length > 4 ? lineArray[4].Trim() : string.Empty,
                    Effective = lineArray.Length > 5 ? lineArray[5].Trim() : string.Empty,
                });
            }

            return result;
        }
    }
}
