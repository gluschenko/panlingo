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
        [InlineData("mo", "ro")]
        [InlineData("mo", "ron")]
        [InlineData("sr", "srp")]
        [InlineData("sh", "srp")]
        [InlineData("he", "heb")]
        public void NormalizeGeneral(string source, string target)
        {
            var options = new LanguageCodeHelper.NormalizationOptions()
                .ToLowerAndTrim()
                .ConvertFromIETF()
                .ConvertFromDeprecatedCode()
                .ResolveUnknownCode(x =>
                {
                    if (x == "sh")
                    {
                        return "sr";
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
    }
}
