using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Panlingo.LanguageCode.Models;

namespace Panlingo.LanguageCode.Generator
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
        private const string ISO_639_2_SEARCH_ENDPOINT = "https://id.loc.gov/search/";
        private const string ISO_639_1_PAST_PRESENT_COLLECTION = "http://id.loc.gov/vocabulary/iso639-1/collection_PastPresentISO639-1Entries";
        private const string ISO_639_2_PAST_PRESENT_COLLECTION = "http://id.loc.gov/vocabulary/iso639-2/collection_PastPresentISO639-2Entries";

        private readonly HttpClient _httpClient;

        public ISOExtractor(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task<string> GetStringAsync(string url, CancellationToken token)
        {
            using var response = await _httpClient.GetAsync(url, token);
            var content = await response.Content.ReadAsStringAsync(token);

            if (!response.IsSuccessStatusCode)
            {
                var challenge = response.Headers.TryGetValues("cf-mitigated", out var values) &&
                    values.Any(x => x.Equals("challenge", StringComparison.OrdinalIgnoreCase));

                throw new HttpRequestException(
                    $"Request to '{url}' failed with {(int)response.StatusCode} ({response.ReasonPhrase})" +
                    (challenge ? ". Cloudflare challenge detected." : "."));
            }

            return content;
        }

        private static IEnumerable<string> GetJsonLdValues(JsonElement resource, string propertyName)
        {
            if (!resource.TryGetProperty(propertyName, out var values) || values.ValueKind != JsonValueKind.Array)
            {
                return Enumerable.Empty<string>();
            }

            return values.EnumerateArray()
                .Where(x => x.TryGetProperty("@value", out _))
                .Select(x => x.GetProperty("@value").GetString() ?? string.Empty)
                .Where(x => !string.IsNullOrWhiteSpace(x));
        }

        private static string? GetJsonLdLink(JsonElement resource, string propertyName)
        {
            if (!resource.TryGetProperty(propertyName, out var values) || values.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            return values.EnumerateArray()
                .Where(x => x.TryGetProperty("@id", out _))
                .Select(x => x.GetProperty("@id").GetString())
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
        }

        private static JsonElement? FindJsonLdResource(string json, string resourceUri)
        {
            using var document = JsonDocument.Parse(json);
            var normalizedResourceUri = resourceUri.Replace("https://", "http://");

            return document.RootElement.EnumerateArray()
                .Where(x => x.TryGetProperty("@id", out _))
                .Where(x => x.GetProperty("@id").GetString()?.Replace("https://", "http://") == normalizedResourceUri)
                .Select(x => (JsonElement?)x.Clone())
                .FirstOrDefault();
        }

        private async Task<IReadOnlyList<string>> GetDeprecatedUrisAsync(
            string searchEndpoint,
            string collectionUri,
            CancellationToken token)
        {
            var typeQuery = Uri.EscapeDataString("rdftype:DeprecatedAuthority");
            var collectionQuery = Uri.EscapeDataString($"memberOf:{collectionUri}");
            var url = $"{searchEndpoint.TrimEnd('/')}/?q={typeQuery}&q={collectionQuery}&format=atom-xml";
            var xml = await GetStringAsync(url, token);

            var document = XDocument.Parse(xml);
            XNamespace atom = "http://www.w3.org/2005/Atom";

            return document.Root?
                .Elements(atom + "entry")
                .SelectMany(x => x.Elements(atom + "link"))
                .Where(x => (string?)x.Attribute("type") == "application/json")
                .Select(x => ((string?)x.Attribute("href") ?? string.Empty).Replace("http://", "https://"))
                .Where(x => x.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                .Select(x => x[..^5])
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray() ?? Array.Empty<string>();
        }

        private async Task<DeprecatedLanguageCode?> GetDeprecatedLanguageCodeAsync(
            string resourceUri,
            CancellationToken token)
        {
            var json = await GetStringAsync($"{resourceUri}.json", token);
            var resource = FindJsonLdResource(json, resourceUri);

            if (resource is null)
            {
                return null;
            }

            var code = GetJsonLdValues(resource.Value, "http://www.loc.gov/mads/rdf/v1#code").FirstOrDefault();

            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            var label = GetJsonLdValues(resource.Value, "http://www.loc.gov/mads/rdf/v1#deprecatedLabel")
                .FirstOrDefault() ?? string.Empty;
            var note = GetJsonLdValues(resource.Value, "http://www.loc.gov/mads/rdf/v1#historyNote")
                .FirstOrDefault() ?? string.Empty;

            return new DeprecatedLanguageCode(
                code,
                NormalizeText(label),
                NormalizeText(note),
                GetJsonLdLink(resource.Value, "http://www.loc.gov/mads/rdf/v1#useInstead"));
        }

        private static string NormalizeText(string value)
        {
            return string.Join(" ", value
                .Replace(" | ", "; ")
                .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        }

        private sealed record DeprecatedLanguageCode(
            string Code,
            string EnglishName,
            string Comment,
            string? UseInstead);

        /// <summary>
        /// Source: https://www.loc.gov/standards/iso639-2/ascii_8bits.html
        /// 
        /// Format:
        /// <code>
        /// An alpha-3 (bibliographic) code, an alpha-3 (terminologic) code (when given), 
        /// an alpha-2 code (when given), an English name, and a French name of a language 
        /// are all separated by pipe (|) characters. If one of these elements is not applicable 
        /// to the entry, the field is left empty, i.e., a pipe (|) character immediately 
        /// follows the preceding entry. The Line terminator is the LF character.
        /// </code>
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<LanguageDescriptor>> ExtractLanguageCodesSetTwoAsync(
            string baseUrl = "https://id.loc.gov/vocabulary/iso639-2.tsv",
            CancellationToken token = default
        )
        {
            var result = new List<LanguageDescriptor>();

            var response = await GetStringAsync(baseUrl, token);

            var lines = response.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0)
                .ToArray();

            foreach (var line in lines)
            {
                var lineArray = line.Split(new[] { '\t' }, StringSplitOptions.None);

                if (lineArray.Length < 3 || lineArray[0].Equals("URI", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                result.Add(new LanguageDescriptor
                {
                    Id = string.Empty,
                    Part2b = lineArray[1].Trim(),
                    Part2t = string.Empty,
                    Part1 = string.Empty,
                    Scope = string.Empty,
                    LanguageType = string.Empty,
                    RefName = NormalizeText(lineArray[2]),
                    Comment = string.Empty,
                });
            }

            return result;
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

            var response = await GetStringAsync(baseUrl, token);

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
        /// Source: https://id.loc.gov/search/
        /// 
        /// Deprecated codes are listed inside brackets [] with a hyphen preceding the code.
        /// 
        /// Categories of change key: Add = Newly added; Dep = Deprecated; CC = Code change; NC = Name change; 
        /// NA = Variant name(s) added.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="token"></param>
        /// <param name="currentLanguages">Current ISO 639-2/639-3 entries used to resolve replacement codes.</param>
        /// <returns></returns>
        public async Task<IEnumerable<LegacyLanguageAlphaTwoDescriptor>> ExtractLanguageCodeDeprecationsSetTwoAsync(
            string baseUrl = ISO_639_2_SEARCH_ENDPOINT,
            CancellationToken token = default,
            IEnumerable<LanguageDescriptor>? currentLanguages = null
        )
        {
            static string GetCodeFromUri(string? uri)
            {
                return string.IsNullOrWhiteSpace(uri) ? string.Empty : uri.TrimEnd('/').Split('/').Last();
            }

            var current = (currentLanguages ?? Enumerable.Empty<LanguageDescriptor>()).ToArray();

            LanguageDescriptor? FindCurrentByCode(string code)
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return null;
                }

                return current.FirstOrDefault(x =>
                    x.Id.Equals(code, StringComparison.OrdinalIgnoreCase) ||
                    x.Part2b.Equals(code, StringComparison.OrdinalIgnoreCase) ||
                    x.Part2t.Equals(code, StringComparison.OrdinalIgnoreCase));
            }

            LanguageDescriptor? FindCurrentByAlphaTwo(string code)
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return null;
                }

                return current.FirstOrDefault(x => x.Part1.Equals(code, StringComparison.OrdinalIgnoreCase));
            }

            static string GetCurrentAlphaThree(LanguageDescriptor? descriptor)
            {
                return descriptor is null
                    ? string.Empty
                    : !string.IsNullOrWhiteSpace(descriptor.Id) ? descriptor.Id : descriptor.Part2b;
            }

            string? FindCodeInComment(string comment, Func<string, bool> predicate)
            {
                return comment
                    .Split(new[] { ' ', '\t', '\r', '\n', ',', '.', ';', ':', '(', ')', '[', ']' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .FirstOrDefault(x => predicate(x));
            }

            string? FindExplicitAlphaThreeReplacement(string comment)
            {
                const string marker = "three-letter identifier ";
                var markerIndex = comment.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

                if (markerIndex < 0)
                {
                    return null;
                }

                var candidate = comment[(markerIndex + marker.Length)..]
                    .Split(new[] { ' ', '\t', '\r', '\n', ',', '.', ';', ':', '(', ')', '[', ']' }, StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault();

                return candidate?.Length == 3 && FindCurrentByCode(candidate) is not null
                    ? candidate
                    : null;
            }

            async Task<IReadOnlyList<DeprecatedLanguageCode>> LoadDeprecatedCodesAsync(
                string collectionUri)
            {
                var uris = await GetDeprecatedUrisAsync(baseUrl, collectionUri, token);
                var codes = new List<DeprecatedLanguageCode>();

                foreach (var uri in uris)
                {
                    var code = await GetDeprecatedLanguageCodeAsync(uri, token);

                    if (code is not null)
                    {
                        codes.Add(code);
                    }
                }

                return codes;
            }

            var alphaThreeDeprecated = await LoadDeprecatedCodesAsync(ISO_639_2_PAST_PRESENT_COLLECTION);
            var alphaTwoDeprecated = await LoadDeprecatedCodesAsync(ISO_639_1_PAST_PRESENT_COLLECTION);
            var alphaThreeResults = new List<LegacyLanguageAlphaTwoDescriptor>();

            foreach (var deprecated in alphaThreeDeprecated)
            {
                var deprecatedCode = deprecated.Code;
                var targetCode = GetCodeFromUri(deprecated.UseInstead);
                var target = FindCurrentByCode(targetCode);

                // MOL is a special case: LOC describes both deprecated identifiers and
                // their replacements only in the history note, without useInstead.
                if (deprecatedCode.Equals("mol", StringComparison.OrdinalIgnoreCase))
                {
                    var oldAlphaTwo = FindCodeInComment(
                        deprecated.Comment,
                        x => x.Length == 2 && alphaTwoDeprecated.Any(y => y.Code.Equals(x, StringComparison.OrdinalIgnoreCase)));

                    alphaThreeResults.Add(new LegacyLanguageAlphaTwoDescriptor
                    {
                        CodeAlpha2 = string.Empty,
                        CodeAlpha2Deprecated = oldAlphaTwo ?? string.Empty,
                        CodeAlpha3 = string.Empty,
                        CodeAlpha3Deprecated = deprecatedCode,
                        CategoryOfChange = "Dep",
                        EnglishName = deprecated.EnglishName,
                        Comment = deprecated.Comment,
                    });

                    continue;
                }

                if (target is null)
                {
                    continue;
                }

                alphaThreeResults.Add(new LegacyLanguageAlphaTwoDescriptor
                {
                    CodeAlpha2 = target.Part1,
                    CodeAlpha2Deprecated = string.Empty,
                    CodeAlpha3 = GetCurrentAlphaThree(target),
                    CodeAlpha3Deprecated = deprecatedCode,
                    CategoryOfChange = "CC",
                    EnglishName = deprecated.EnglishName,
                    Comment = deprecated.Comment,
                });
            }

            var result = new List<LegacyLanguageAlphaTwoDescriptor>(alphaThreeResults);

            foreach (var deprecated in alphaTwoDeprecated)
            {
                // MOL and MO are represented by one combined LOC change record.
                if (deprecated.Code.Equals("mo", StringComparison.OrdinalIgnoreCase) &&
                    alphaThreeResults.Any(x => x.CodeAlpha3Deprecated.Equals("mol", StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                var targetAlphaTwo = GetCodeFromUri(deprecated.UseInstead);
                var target = FindCurrentByAlphaTwo(targetAlphaTwo);
                var targetAlphaThree = GetCurrentAlphaThree(target);

                if (target is null)
                {
                    targetAlphaThree = FindExplicitAlphaThreeReplacement(deprecated.Comment) ?? string.Empty;
                    target = FindCurrentByCode(targetAlphaThree);
                }

                result.Add(new LegacyLanguageAlphaTwoDescriptor
                {
                    CodeAlpha2 = target?.Part1 ?? string.Empty,
                    CodeAlpha2Deprecated = deprecated.Code,
                    CodeAlpha3 = targetAlphaThree,
                    CodeAlpha3Deprecated = string.Empty,
                    CategoryOfChange = "Dep",
                    EnglishName = deprecated.EnglishName,
                    Comment = deprecated.Comment,
                });

                // LOC exposes the old alpha-2 record and the replacement link. The
                // legacy change table also contains a separate current-code CC row
                // when no alpha-3 code was deprecated alongside it.
                if (!string.IsNullOrWhiteSpace(targetAlphaTwo) &&
                    target is not null &&
                    !alphaThreeResults.Any(x => x.CodeAlpha3.Equals(targetAlphaThree, StringComparison.OrdinalIgnoreCase)))
                {
                    result.Add(new LegacyLanguageAlphaTwoDescriptor
                    {
                        CodeAlpha2 = target.Part1,
                        CodeAlpha2Deprecated = string.Empty,
                        CodeAlpha3 = targetAlphaThree,
                        CodeAlpha3Deprecated = string.Empty,
                        CategoryOfChange = "CC",
                        EnglishName = deprecated.EnglishName,
                        Comment = string.Empty,
                    });
                }
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

            var response = await GetStringAsync(baseUrl, token);

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

            var response = await GetStringAsync(baseUrl, token);

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

                var id = lineArray.Length > 0 ? lineArray[0].Trim() : string.Empty;
                var refName = lineArray.Length > 1 ? lineArray[1].Trim() : string.Empty;
                var retReason = lineArray.Length > 2 ? lineArray[2].Trim() : string.Empty;
                var changeTo = lineArray.Length > 3 ? lineArray[3].Trim() : string.Empty;
                var retRemedy = lineArray.Length > 4 ? lineArray[4].Trim() : string.Empty;
                var effective = lineArray.Length > 5 ? lineArray[5].Trim() : string.Empty;

                var changes = new List<string>();

                if (!string.IsNullOrWhiteSpace(changeTo))
                {
                    changes.Add(changeTo);
                }

                var codesFromComment = retRemedy.Split('[')
                    .Select(x => x.Split(']').First())
                    .Select(x => x.Trim())
                    .Where(x => x.Length == 3)
                    .ToArray();

                changes.AddRange(codesFromComment);

                result.Add(new LegacyLanguageAlphaThreeDescriptor
                {
                    Id = id,
                    RefName = refName,
                    RetReason = retReason,
                    ChangeTo = changes,
                    RetRemedy = retRemedy,
                    Effective = effective,
                });
            }

            return result;
        }
    }
}
