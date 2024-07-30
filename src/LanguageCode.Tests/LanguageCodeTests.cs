using Panlingo.LanguageCode;
using Panlingo.LanguageCode.Models;

namespace LanguageCode.Tests
{
    public class LanguageCodeTests
    {
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
            var options = new LanguageCodeHelper.LanguageCodeResolver()
                .ToLowerAndTrim()
                .ConvertFromIETF()
                .ConvertFromDeprecatedCode()
                .ResolveUnknownCode(x =>
                {
                    if (x == "mo")
                    {
                        return "ro";
                    }

                    return x;
                })
                .ConvertTo(LanguageCodeEntity.Alpha3);

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
            var options = new LanguageCodeHelper.LanguageCodeResolver()
                .ToLowerAndTrim()
                .ConvertFromIETF()
                .ConvertFromDeprecatedCode()
                .ResolveUnknownCode(x =>
                {
                    if (x == "mo")
                    {
                        return "ro";
                    }

                    return x;
                })
                .ConvertTo(LanguageCodeEntity.Alpha3);

            var code = LanguageCodeHelper.Resolve(code: source, options: options);
            Assert.Equal(target, code);
        }

        [Theory]
        [InlineData("azb", "aze")]
        [InlineData("azj", "aze")]
        [InlineData("cmn", "zho")]
        [InlineData("nan", "zho")]
        [InlineData("hji", "msa")]
        [InlineData("ind", "msa")]
        public void ResolveMacrolanguage(string source, string target)
        {
            var options = new LanguageCodeHelper.LanguageCodeResolver()
                .ToLowerAndTrim()
                .ConvertFromIETF()
                .ConvertFromDeprecatedCode()
                .ReduceToMacrolanguage()
                .ConvertTo(LanguageCodeEntity.Alpha3);

            var code = LanguageCodeHelper.Resolve(code: source, options: options);
            Assert.Equal(target, code);
        }

        [Theory]
        [InlineData("ru", "Russian")]
        [InlineData("uk", "Ukrainian")]
        [InlineData("en", "English")]
        [InlineData("ro", "Romanian")]
        [InlineData("sr", "Serbian")]
        [InlineData("he", "Hebrew")]
        public void ResolveEnglishName(string source, string target)
        {
            var options = new LanguageCodeHelper.LanguageCodeResolver()
                .ToLowerAndTrim()
                .ConvertFromIETF()
                .ConvertFromDeprecatedCode()
                .ConvertTo(LanguageCodeEntity.EnglishName);

            var code = LanguageCodeHelper.Resolve(code: source, options: options);
            Assert.Equal(target, code);
        }

        [Theory]
        [InlineData("ru-RU", "ru")]
        [InlineData("uk-UA", "uk")]
        [InlineData("en-US", "en")]
        public void ResolveIETF(string source, string target)
        {
            var options = new LanguageCodeHelper.LanguageCodeResolver().ConvertFromIETF();
            var code = LanguageCodeHelper.Resolve(code: source, options: options);
            Assert.Equal(target, code);
        }

        [Theory]
        [InlineData("RU", "ru")]
        [InlineData("UK  ", "uk")]
        [InlineData("en", "en")]
        public void ResolveToLowerAndTrim(string source, string target)
        {
            var options = new LanguageCodeHelper.LanguageCodeResolver().ToLowerAndTrim();
            var code = LanguageCodeHelper.Resolve(code: source, options: options);
            Assert.Equal(target, code);
        }
    }
}
