using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace OCR.ImageProcessing
{
    class Rotate : IImageProcessor
    {
        public void Execute()
        {
            using (Image image = Image.Load(Windows.ApplicationModel.Package.Current.InstalledLocation.Path + "\\Assets\\sample.jpg"))
            {
                image.Mutate(img => img.Rotate(30));
                image.Save(Path.GetTempPath() + "test.png");
            }
        }
    }
}
