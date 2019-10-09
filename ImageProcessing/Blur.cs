using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace OCR.ImageProcessing
{
    class Blur: IImageProcessor
    {
        public void Execute()
        {
            using (Image image = Image.Load(Windows.ApplicationModel.Package.Current.InstalledLocation.Path+"\\Assets\\sample.jpg"))
            {
                image.Mutate(img => img.GaussianBlur(0.3f));
                image.Save(Path.GetTempPath()+"test.png");
            }
        }
    }
}
