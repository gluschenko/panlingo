using System.Runtime.InteropServices;

namespace LanguageIdentification.FastText;

public class FastTextDetector : IDisposable
{
    private IntPtr _fastText;

    public FastTextDetector()
    {
        _fastText = FastTextDetectorWrapper.CreateFastText();
    }

    public string ModelPath { get; private set; } = string.Empty;

    public void LoadModel(string path)
    {
        var errptr = IntPtr.Zero;
        FastTextDetectorWrapper.FastTextLoadModel(_fastText, path, ref errptr);
        CheckError(errptr);

        ModelPath = path;
    }

    public IEnumerable<PredictionLabel> GetLabels()
    {
        var labelsPtr = FastTextDetectorWrapper.FastTextGetLabels(_fastText);

        if (labelsPtr == IntPtr.Zero)
        {
            return Array.Empty<PredictionLabel>();
        }

        var labelsStruct = Marshal.PtrToStructure<FastTextLabels>(labelsPtr);
        var result = new List<PredictionLabel>();

        for (ulong i = 0; i < labelsStruct.Length; i++)
        {
            var labelPtr = Marshal.ReadIntPtr(labelsStruct.Labels, (int)i * IntPtr.Size);
            var label = DecodeString(labelPtr);
            var freq = Marshal.ReadInt64(labelsStruct.Freqs, (int)i * sizeof(long));
            result.Add(new PredictionLabel(label: label, frequency: freq));
        }

        FastTextDetectorWrapper.DestroyLabels(labelsPtr);
        return result;
    }

    public IEnumerable<Prediction> Predict(string text, int k, float threshold = 0.0f)
    {
        var errptr = IntPtr.Zero;
        var predictionPtr = FastTextDetectorWrapper.FastTextPredict(
            handle: _fastText, text: text, 
            k: k, 
            threshold: threshold, 
            ref errptr
        );

        CheckError(errptr);

        if (predictionPtr == IntPtr.Zero)
        {
            return Array.Empty<Prediction>();
        }

        var predictions = Marshal.PtrToStructure<FastTextPredictions>(predictionPtr);
        var result = new List<Prediction>();

        for (ulong i = 0; i < predictions.Length; i++)
        {
            IntPtr elementPtr = new IntPtr(predictions.Predictions.ToInt64() + (long)(i * (uint)Marshal.SizeOf<FastTextPrediction>()));
            var prediction = Marshal.PtrToStructure<FastTextPrediction>(elementPtr);
            var label = DecodeString(prediction.Label);

            result.Add(new Prediction(
                label: label, 
                probability: prediction.Prob
            ));
        }

        FastTextDetectorWrapper.DestroyPredictions(predictionPtr);
        return result;
    }

    public int GetModelDimensions()
    {
        return FastTextDetectorWrapper.FastTextGetModelDimensions(_fastText);
    }

    public void Dispose()
    {
        if (_fastText != IntPtr.Zero)
        {
            FastTextDetectorWrapper.DestroyFastText(_fastText);
            _fastText = IntPtr.Zero;
        }
    }

    private void CheckError(IntPtr errorPtr)
    {
        if (errorPtr != IntPtr.Zero)
        {
            ThrowNativeException(errorPtr);
        }
    }

    private void ThrowNativeException(IntPtr errorPtr)
    {
        var error = DecodeString(errorPtr);
        FastTextDetectorWrapper.DestroyString(errorPtr);
        throw new NativeLibraryException(error);
    }

    private string DecodeString(IntPtr ptr)
    {
        return Marshal.PtrToStringUTF8(ptr) ?? throw new NullReferenceException("Failed to decode non-nullable string");
    }
}
