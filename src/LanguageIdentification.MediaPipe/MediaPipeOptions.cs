using System.IO;

namespace Panlingo.LanguageIdentification.MediaPipe
{
    public class MediaPipeOptions
    {
        public int ResultCount { get; private set; } = 10;
        public float ScoreThreshold { get; private set; } = 0.0f;
        public string? ModelPath { get; private set; } = null;
        public Stream? ModelStream { get; private set; } = null;

        public static MediaPipeOptions FromFile(string path)
        {
            return new MediaPipeOptions
            {
                ModelPath = path,
            };
        }

        public static MediaPipeOptions FromStream(Stream stream)
        {
            return new MediaPipeOptions
            {
                ModelStream = stream,
            };
        }

        public MediaPipeOptions WithResultCount(int resultCount)
        {
            ResultCount = resultCount;
            return this;
        }

        public MediaPipeOptions WithScoreThreshold(float scoreThreshold)
        {
            ScoreThreshold = scoreThreshold;
            return this;
        }
    }
}
