using System.Diagnostics;

namespace Panlingo.LanguageIdentification.FastText
{
    [DebuggerDisplay("{Label} ({Frequency})")]
    internal readonly struct FastTextLabelResult
    {
        public readonly string Label;
        public readonly long Frequency;

        public FastTextLabelResult(string label, long frequency)
        {
            Label = label;
            Frequency = frequency;
        }
    }
}
