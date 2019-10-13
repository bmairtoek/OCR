using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using Windows.Storage;

namespace OCR.ImageProcessing
{
    class KeystoneEffect : IImageProcessor
    {
        public float startValue { get; } = 1;
        public float lastValue { get; } = 0.1f;
        public void Execute(float value, StorageFile inputFile, string outputFolderPath)
        {
            using (Image image = Image.Load(inputFile.OpenStreamForReadAsync().Result))
            {
                image.Mutate(img => img.Transform(new ProjectiveTransformBuilder().AppendTaper(TaperSide.Top, TaperCorner.Both, value)));
                image.Save(Path.Combine(outputFolderPath, value.ToString() + ".png"));
            }
        }
    }
}
