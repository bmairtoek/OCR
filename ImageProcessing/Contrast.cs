using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using Windows.Storage;

namespace OCR.ImageProcessing
{
    class Contrast : IImageProcessor
    {
        public float startValue { get; } = 0;
        public float lastValue { get; } = 2;
        public void Execute(float value, StorageFile inputFile, string outputFolderPath)
        {
            using (Image image = Image.Load(inputFile.OpenStreamForReadAsync().Result))
            {
                image.Mutate(img => img.Contrast(value));
                image.Save(Path.Combine(outputFolderPath, value.ToString() + ".png"));
            }
        }
    }
}
