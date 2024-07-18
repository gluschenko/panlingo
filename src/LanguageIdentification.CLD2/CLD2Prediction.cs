using Panlingo.LanguageIdentification.CLD2.Internal;

namespace Panlingo.LanguageIdentification.CLD2
{
    public class CLD2Prediction
    {
        public string Language { get; private set; }
        public string Script { get; private set; }
        public double Probability { get; private set; }
        public bool IsReliable { get; private set; }
        public double Proportion { get; private set; }

        internal CLD2Prediction(CLD2PredictionResult item)
        {
            Language = item.Language;
            Script = item.Script;
            Probability = item.Probability;
            IsReliable = item.IsReliable;
            Proportion = item.Proportion;
        }
    }
}
