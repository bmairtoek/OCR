using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using Windows.Storage;

namespace OCR.ImageProcessing
{
    class Rotate : IImageProcessor
    {
        public float MinimalValue { get; } = 0;
        public float MaximalValue { get; } = 60;
        public void Execute(float value, StorageFile inputFile, string outputFolderPath)
        {
            using (Image image = Image.Load(inputFile.OpenStreamForReadAsync().Result))
            {
                image.Mutate(img => img.Rotate(value));
                image.Save(Path.Combine(outputFolderPath, value.ToString() + ".png"));
            }
        }
    }
}
