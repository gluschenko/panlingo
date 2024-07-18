using Panlingo.LanguageIdentification.Whatlang.Internal;

namespace Panlingo.LanguageIdentification.Whatlang
{
    public class WhatlangPrediction
    {
        public WhatlangLanguage Lang { get; private set; }
        public WhatlangScript Script { get; private set; }
        public double Confidence { get; private set; }
        public bool IsReliable { get; private set; }

        internal WhatlangPrediction(WhatlangPredictionResult item)
        {
            Lang = item.Lang;
            Script = item.Script;
            Confidence = item.Confidence;
            IsReliable = item.IsReliable;
        }
    }
}
