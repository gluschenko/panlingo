namespace Panlingo.LanguageIdentification.FastText
{
    public class FastTextLabel
    {
        public string Label { get; private set; }
        public long Frequency { get; private set; }

        internal FastTextLabel(string label, long frequency)
        {
            Label = label;
            Frequency = frequency;
        }
    }
}
