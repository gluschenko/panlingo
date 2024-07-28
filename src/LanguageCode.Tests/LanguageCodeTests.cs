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
    }
}
