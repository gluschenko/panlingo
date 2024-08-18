using Panlingo.LanguageIdentification.Lingua.Internal;

namespace Panlingo.LanguageIdentification.Lingua
{
    public class LinguaPrediction
    {
        public LinguaLanguage Language { get; private set; }
        public double Confidence { get; private set; }

        internal LinguaPrediction(LinguaPredictionResult item)
        {
            Language = item.Language;
            Confidence = item.Confidence;
        }
    }
}
