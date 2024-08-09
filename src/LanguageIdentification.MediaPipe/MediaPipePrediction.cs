using Panlingo.LanguageIdentification.MediaPipe.Internal;

namespace Panlingo.LanguageIdentification.MediaPipe
{
    public class MediaPipePrediction
    {
        public string Language { get; private set; }
        public double Probability { get; private set; }

        internal MediaPipePrediction(LanguageDetectorPrediction item)
        {
            Language = item.LanguageCode;
            Probability = item.Probability;
        }

        public bool IsUnknown()
        {
            return Language.Equals("unknown", System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
