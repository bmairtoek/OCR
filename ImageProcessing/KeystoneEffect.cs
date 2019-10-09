using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace OCR.ImageProcessing
{
    class KeystoneEffect : IImageProcessor
    {
        public void Execute()
        {
            using (Image image = Image.Load(Windows.ApplicationModel.Package.Current.InstalledLocation.Path + "\\Assets\\sample.jpg"))
            {
                image.Mutate(img => img.Transform(new ProjectiveTransformBuilder().AppendTaper(TaperSide.Top, TaperCorner.Both, 0.1f)));
                image.Save(Path.GetTempPath() + "test.png");
            }
        }
    }
}
