namespace LangaugeCode.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void GetTwoLetterISOCode()
        {
            var ru = LanguageCodeHelper.GetTwoLetterISOCode("ru");
            Assert.Equal("ru", ru);

            var rus = LanguageCodeHelper.GetTwoLetterISOCode("rus");
            Assert.Equal("ru", rus);

            var uk = LanguageCodeHelper.GetTwoLetterISOCode("uk");
            Assert.Equal("uk", uk);

            var ukr = LanguageCodeHelper.GetTwoLetterISOCode("ukr");
            Assert.Equal("uk", ukr);
        }

        [Fact]
        public void GetThreeLetterISOCode()
        {
            var ru = LanguageCodeHelper.GetThreeLetterISOCode("ru");
            Assert.Equal("rus", ru);

            var rus = LanguageCodeHelper.GetThreeLetterISOCode("rus");
            Assert.Equal("rus", rus);

            var uk = LanguageCodeHelper.GetThreeLetterISOCode("uk");
            Assert.Equal("ukr", uk);

            var ukr = LanguageCodeHelper.GetThreeLetterISOCode("ukr");
            Assert.Equal("ukr", ukr);
        }
    }
}
