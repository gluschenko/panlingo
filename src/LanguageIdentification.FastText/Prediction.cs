using System.Diagnostics;

namespace Panlingo.LanguageIdentification.FastText
{
    [DebuggerDisplay("{Label} ({Probability})")]
    public readonly struct Prediction
    {
        public readonly float Probability;
        public readonly string Label;

        public Prediction(float probability, string label)
        {
            Probability = probability;
            Label = label;
        }
    }
}
