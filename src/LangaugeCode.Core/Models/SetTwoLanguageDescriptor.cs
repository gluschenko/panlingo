﻿namespace Panlingo.LangaugeCode.Core.Models
{
    public class SetTwoLanguageDescriptor
    {
        public string CodeAlpha3Bibliographic { get; set; }
        public string CodeAlpha3Terminologic { get; set; }
        public string CodeAlpha2 { get; set; }
        public string EnglishName { get; set; }
        public string FrenchName { get; set; }

        public override string ToString()
        {
            return $"{CodeAlpha3Bibliographic}; {CodeAlpha3Terminologic}; {CodeAlpha2}; {EnglishName}";
        }
    }
}
