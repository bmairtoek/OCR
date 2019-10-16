using Windows.Storage;

namespace OCR.ImageProcessing
{
    interface IImageProcessor
    {
        float startValue { get; }
        float lastValue { get; }
        void Execute(float value, StorageFile inputFile, string outputFolderPath);
    }
}
