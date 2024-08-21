using System;
using Panlingo.LanguageIdentification.Lingua.Internal;

namespace Panlingo.LanguageIdentification.Lingua
{
    public class LinguaPredictionRange : LinguaPrediction
    {
        public int WordCount { get; private set; }
        public Range Range { get; private set; }
        public string Fragment { get; private set; }

        internal LinguaPredictionRange(LinguaPredictionRangeResult item, byte[] text) : base((LinguaPredictionResult)item)
        {
            WordCount = (int)item.WordCount;

            var start = (int)item.StartIndex; 
            var end = (int)item.EndIndex;
            if (start == 0 && end == 0)
            {
                Range = Range.All;
            }
            else if (start == 0 && end != 0)
            {
                Range = Range.EndAt(end);
            }
            else if (start != 0 && end == 0)
            {
                Range = Range.StartAt(start);
            }
            else
            {
                Range = new Range(start, end);
            }

            Fragment = System.Text.Encoding.UTF8.GetString(text[Range]);
        }

        public override string ToString()
        {
            return $"{base.ToString()} [{Range}]";
        }
    }
}
