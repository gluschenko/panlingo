using Panlingo.LanguageCode;

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
        [InlineData("he", "heb")]
        public void NormalizeGeneral(string source, string target)
        {
            var options = new LanguageCodeHelper.NormalizationOptions()
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
                .ConvertTo(Panlingo.LanguageCode.Models.LanguageCodeType.Alpha3);

            var code = LanguageCodeHelper.Normalize(code: source, options: options);
            Assert.Equal(target, code);
        }

        [Theory]
        [InlineData("ru-RU", "ru")]
        [InlineData("uk-UA", "uk")]
        [InlineData("en-US", "en")]
        public void NormalizeIETF(string source, string target)
        {
            var options = new LanguageCodeHelper.NormalizationOptions().ConvertFromIETF();
            var code = LanguageCodeHelper.Normalize(code: source, options: options);
            Assert.Equal(target, code);
        }

        [Theory]
        [InlineData("RU", "ru")]
        [InlineData("UK  ", "uk")]
        [InlineData("en", "en")]
        public void NormalizeToLowerAndTrim(string source, string target)
        {
            var options = new LanguageCodeHelper.NormalizationOptions().ToLowerAndTrim();
            var code = LanguageCodeHelper.Normalize(code: source, options: options);
            Assert.Equal(target, code);
        }
    }
}
