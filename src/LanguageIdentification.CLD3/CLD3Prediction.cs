using Panlingo.LanguageIdentification.CLD3.Internal;

namespace Panlingo.LanguageIdentification.CLD3
{
    public class CLD3Prediction
    {
        public string Language { get; private set; }
        public double Probability { get; private set; }
        public bool IsReliable { get; private set; }
        public double Proportion { get; private set; }

        internal CLD3Prediction(CLD3PredictionResult item)
        {
            Language = item.Language;
            Probability = item.Probability;
            IsReliable = item.IsReliable;
            Proportion = item.Proportion;
        }

        public bool IsUnknown()
        {
            return Language.Equals("und", System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
