using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using LangaugeCode.Core.Models;
using static System.Net.Mime.MediaTypeNames;

namespace LangaugeCode.Core
{
    public class ISOExtractor
    {
        private readonly HttpClient _httpClient;

        public ISOExtractor(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Source: https://www.loc.gov/standards/iso639-2/ascii_8bits.html
        /// 
        /// Format:
        /// An alpha-3 (bibliographic) code, an alpha-3 (terminologic) code (when given), 
        /// an alpha-2 code (when given), an English name, and a French name of a language 
        /// are all separated by pipe (|) characters. If one of these elements is not applicable 
        /// to the entry, the field is left empty, i.e., a pipe (|) character immediately 
        /// follows the preceding entry. The Line terminator is the LF character.
        /// 
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LanguageDescriptor>> ExtractAsync(
            string baseUrl = "https://www.loc.gov/standards/iso639-2/ISO-639-2_8859-1.txt", 
            CancellationToken token = default
        )
        {
            var result = new List<LanguageDescriptor>();

            var response = await _httpClient.GetStringAsync(baseUrl);
            response = Encoding.UTF8.GetString(Encoding.Default.GetBytes(response));

            var lines = response.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0)
                .ToArray();

            foreach (var line in lines)
            {
                var lineArray = line.Split(new[] { '|' });
                if (lineArray.Length == 5)
                {
                    result.Add(new LanguageDescriptor
                    {
                        CodeAlpha3Bibliographic = lineArray[0],
                        CodeAlpha3Terminologic = lineArray[1],
                        CodeAlpha2 = lineArray[2],
                        EnglishName = lineArray[3],
                        FrenchName = lineArray[4],
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Source: https://iso639-3.sil.org/code_tables/download_tables
        /// 
        /// Format:
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
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<LanguageDescriptor2>> Extract2Async(
            string baseUrl = "https://iso639-3.sil.org/sites/iso639-3/files/downloads/iso-639-3.tab",
            CancellationToken token = default
        )
        {
            var result = new List<LanguageDescriptor2>();

            var response = await _httpClient.GetStringAsync(baseUrl);
            response = Encoding.UTF8.GetString(Encoding.Default.GetBytes(response));

            var lines = response.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0)
                .ToArray();

            foreach (var line in lines)
            {
                var lineArray = line.Split(new[] { '\t' });

                if (lineArray[0] == "Id")
                {
                    continue;
                }

                result.Add(new LanguageDescriptor2
                {
                    CodeAlpha3Bibliographic = lineArray[0],
                    CodeAlpha3Terminologic = lineArray[1],
                    CodeAlpha2 = lineArray[2],
                    EnglishName = lineArray[3],
                    FrenchName = lineArray[4],
                });
            }

            return result;
        }
    }
}
