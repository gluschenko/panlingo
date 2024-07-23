using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LangaugeCode.Core.Models;

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
    }
}
