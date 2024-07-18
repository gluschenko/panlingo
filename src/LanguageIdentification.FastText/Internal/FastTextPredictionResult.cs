using System.Diagnostics;

namespace Panlingo.LanguageIdentification.FastText.Internal
{
    [DebuggerDisplay("{Label} ({Probability})")]
    internal readonly struct FastTextPredictionResult
    {
        public readonly float Probability;
        public readonly string Label;

        public FastTextPredictionResult(float probability, string label)
        {
            Probability = probability;
            Label = label;
        }
    }
}
