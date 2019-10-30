using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using Windows.Storage;

namespace OCR.ImageProcessing
{
    class Blur: IImageProcessor
    {
        public float startValue { get; } = 0;
        public float lastValue { get; } = 5;
        public void Execute(float value, StorageFile inputFile, string outputFolderPath)
        {
            // Windows.ApplicationModel.Package.Current.InstalledLocation.Path+"\\Assets\\sample.jpg"
            using (Image image = Image.Load(inputFile.OpenStreamForReadAsync().Result))
            {
                image.Mutate(img => img.GaussianBlur(value));
                image.Save(Path.Combine(outputFolderPath, value.ToString()+".png"));
            }
        }
    }
}
