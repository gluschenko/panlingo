using Panlingo.LanguageCode;
using Panlingo.LanguageCode.Models;

namespace LanguageCode.Tests
{
    public class LanguageCodeTests
    {
        private static LanguageCodeResolver BasicResolver => new LanguageCodeResolver()
            .ToLowerAndTrim()
            .ConvertFromIETF()
            .ConvertFromDeprecatedCode()
            .ReduceToMacrolanguage();

        [Theory]
        [InlineData("ru", "ru")]
        [InlineData("rus", "ru")]
        [InlineData("uk", "uk")]
        [InlineData("ukr", "uk")]
        public void GetTwoLetterISOCode(string source, string target)
        {
            var code = LanguageCodeHelper.GetTwoLetterISOCode(source);
            Assert.Equal(target, code);
        }

        [Theory]
        [InlineData("ru", "rus")]
        [InlineData("rus", "rus")]
        [InlineData("uk", "ukr")]
        [InlineData("ukr", "ukr")]
        public void GetThreeLetterISOCode(string source, string target)
        {
            var code = LanguageCodeHelper.GetThreeLetterISOCode(source);
            Assert.Equal(target, code);
        }

        [Theory]
        [InlineData("ru", "rus")]
        [InlineData("uk", "ukr")]
        [InlineData("en", "eng")]
        [InlineData("ro", "ron")]
        [InlineData("mo", "ron")]
        [InlineData("sr", "srp")]
        [InlineData("sh", "hbs")]
        [InlineData("iw", "heb")]
        [InlineData("he", "heb")]
        public void ResolveGeneral(string source, string target)
        {
            var options = BasicResolver
                .Select(LanguageCodeEntity.Alpha3);

            var code = LanguageCodeHelper.Resolve(code: source, options: options);
            Assert.Equal(target, code);
        }

        [Theory]
        [InlineData("mo", "ron")]
        [InlineData("iw", "heb")]
        [InlineData("ji", "yid")]
        [InlineData("in", "ind")]
        [InlineData("yib", "eng")]
        [InlineData("aex", "eng")]
        [InlineData("yuu", "yug")]
        [InlineData("hr", "hrv")]
        [InlineData("scr", "hrv")]
        [InlineData("sr", "srp")]
        [InlineData("scc", "srp")]
        public void ResolveLegacy(string source, string target)
        {
            var options = BasicResolver
                .Select(LanguageCodeEntity.Alpha3);

            var code = LanguageCodeHelper.Resolve(code: source, options: options);
            Assert.Equal(target, code);
        }

        [Theory]
        [InlineData("mof", "xpq")]
        [InlineData("rmr", "rmq")]
        [InlineData("gio", "gqu")]
        [InlineData("pgy", "pgy")] // extinct
        [InlineData("emo", "emo")] // extinct
        public void ResolveLegacyConflict(string source, string target)
        {
            var options = BasicResolver
                .ConvertFromDeprecatedCode((sourceCode, candidates) =>
                {
                    return candidates.Any() ? candidates.First() : sourceCode;
                })
                .Select(LanguageCodeEntity.Alpha3);

            var code = LanguageCodeHelper.Resolve(code: source, options: options);
            Assert.Equal(target, code);
        }

        [Theory]
        [InlineData("azb", "aze")]
        [InlineData("azj", "aze")]
        [InlineData("hji", "msa")]
        [InlineData("ind", "msa")]
        [InlineData("nan", "zho")]
        [InlineData("cmn", "zho")]
        public void ResolveMacrolanguage(string source, string target)
        {
            var options = BasicResolver
                .ReduceToMacrolanguage()
                .Select(LanguageCodeEntity.Alpha3);

            var code = LanguageCodeHelper.Resolve(code: source, options: options);
            Assert.Equal(target, code);
        }

        [Theory]
        [InlineData("ru", "Russian")]
        [InlineData("uk", "Ukrainian")]
        [InlineData("en", "English")]
        [InlineData("ro", "Romanian")]
        [InlineData("mo", "Moldavian")]     // legacy code for [ro]
        [InlineData("sr", "Serbian")]
        [InlineData("sh", "Serbo-Croatian")]
        [InlineData("he", "Hebrew")]
        [InlineData("iw", "Hebrew")]
        [InlineData("in", "Indonesian")]
        [InlineData("id", "Indonesian")]
        [InlineData("cmn", "Mandarin Chinese")]
        [InlineData("zho", "Chinese")]
        [InlineData("emo", "Emok")]         // extinct
        [InlineData("kxl", "Nepali Kurux")] // legacy code for [kru]
        [InlineData("kxu", "Kui (India)")]  // splitted for [dwk] and [uki]
        public void ResolveEnglishName(string source, string target)
        {
            var options = BasicResolver
                .RemoveRule(LanguageCodeRule.ConvertFromDeprecatedCode)
                .RemoveRule(LanguageCodeRule.ReduceToMacrolanguage)
                .Select(LanguageCodeEntity.EnglishName);

            var code = LanguageCodeHelper.Resolve(code: source, options: options);
            Assert.Equal(target, code);
        }

        [Theory]
        [InlineData("ru-RU", "ru")]
        [InlineData("uk-UA", "uk")]
        [InlineData("en-US", "en")]
        public void ResolveIETF(string source, string target)
        {
            var options = new LanguageCodeResolver().ConvertFromIETF();
            var code = LanguageCodeHelper.Resolve(code: source, options: options);
            Assert.Equal(target, code);
        }

        [Theory]
        [InlineData("RU", "ru")]
        [InlineData("UK  ", "uk")]
        [InlineData("en", "en")]
        public void ResolveToLowerAndTrim(string source, string target)
        {
            var options = new LanguageCodeResolver().ToLowerAndTrim();
            var code = LanguageCodeHelper.Resolve(code: source, options: options);
            Assert.Equal(target, code);
        }

        [Theory]
        [InlineData("aaaa")]
        [InlineData("bbbb")]
        [InlineData("cccc")]
        [InlineData("eeee")]
        [InlineData("dddd")]
        public void ResolveMissingWithException(string source)
        {
            Assert.Throws<LanguageCodeException>(() => 
            {
                var options = BasicResolver.Select(LanguageCodeEntity.Alpha3);
                var code = LanguageCodeHelper.Resolve(code: source, options: options);
            });
        }

        [Theory]
        [InlineData("aaaa")]
        [InlineData("bbbb")]
        [InlineData("cccc")]
        [InlineData("eeee")]
        [InlineData("dddd")]
        public void ResolveMissing(string source)
        {
            var options = BasicResolver;
            var code = LanguageCodeHelper.Resolve(code: source, options: options);
            Assert.Equal(source, code);
        }
    }
}
