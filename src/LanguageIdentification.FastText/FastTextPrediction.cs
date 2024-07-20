using Panlingo.LanguageIdentification.FastText.Internal;

namespace Panlingo.LanguageIdentification.FastText
{
    public class FastTextPrediction
    {
        public float Probability { get; private set; }
        public string Label { get; private set; }

        internal FastTextPrediction(float probability, string label)
        {
            Probability = probability;
            Label = label;
        }
    }
}
