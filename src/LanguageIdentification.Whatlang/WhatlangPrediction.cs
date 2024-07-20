using Panlingo.LanguageIdentification.Whatlang.Internal;

namespace Panlingo.LanguageIdentification.Whatlang
{
    public class WhatlangPrediction
    {
        public WhatlangLanguage Language { get; private set; }
        public WhatlangScript Script { get; private set; }
        public double Confidence { get; private set; }
        public bool IsReliable { get; private set; }

        internal WhatlangPrediction(WhatlangPredictionResult item)
        {
            Language = item.Lang;
            Script = item.Script;
            Confidence = item.Confidence;
            IsReliable = item.IsReliable;
        }
    }
}
