using System.Diagnostics;

namespace Panlingo.LanguageIdentification.FastText
{
    [DebuggerDisplay("{Label} ({Frequency})")]
    public readonly struct PredictionLabel
    {
        public readonly string Label;
        public readonly long Frequency;

        public PredictionLabel(string label, long frequency)
        {
            Label = label;
            Frequency = frequency;
        }
    }
}
