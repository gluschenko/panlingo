using System.IO;

namespace Panlingo.LanguageIdentification.MediaPipe
{
    public class MediaPipeOptions
    {
        public int ResultCount { get; private set; } = 10;
        public float ScoreThreshold { get; private set; } = 0.0f;
        public string? ModelPath { get; private set; } = null;
        public Stream? ModelStream { get; private set; } = null;
        public byte[]? ModelData { get; private set; } = null;

        internal MediaPipeOptions()
        {
        }

        /// <summary>
        /// Creates MediaPipe options with model from file
        /// </summary>
        /// <param name="path">Path to *.tflite model</param>
        /// <returns></returns>
        public static MediaPipeOptions FromFile(string path)
        {
            return new MediaPipeOptions
            {
                ModelPath = path,
            };
        }

        /// <summary>
        /// Creates MediaPipe options with model from stream
        /// </summary>
        /// <param name="stream">Stream to *.tflite model</param>
        /// <returns></returns>
        public static MediaPipeOptions FromStream(Stream stream)
        {
            return new MediaPipeOptions
            {
                ModelStream = stream,
            };
        }

        /// <summary>
        /// <para>Creates MediaPipe options with self-contained model located in package.</para>
        /// <para>Original file: https://storage.googleapis.com/mediapipe-models/language_detector/language_detector/float32/1/language_detector.tflite</para>
        /// </summary>
        /// <returns></returns>
        public static MediaPipeOptions FromDefault()
        {
            return new MediaPipeOptions 
            {
                ModelData = MediaPipeResourceProvider.DefaultModel,
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
