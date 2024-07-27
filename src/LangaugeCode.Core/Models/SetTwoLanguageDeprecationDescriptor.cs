﻿namespace Panlingo.LanguageCode.Core.Models
{
    public class SetTwoLanguageDeprecationDescriptor
    {
        public string CodeAlpha2 { get; set; }
        public string CodeAlpha2Deprecated { get; set; }
        public string CodeAlpha3 { get; set; }
        public string CodeAlpha3Deprecated { get; set; }
        public string CategoryOfChange { get; set; }
        public string EnglishName { get; set; }
        public string Comment { get; set; }

        public override string ToString()
        {
            return $"{CodeAlpha2Deprecated} -> {CodeAlpha2}; {CodeAlpha3Deprecated} -> {CodeAlpha3}; {CategoryOfChange}; {EnglishName}; {Comment}";
        }
    }
}
