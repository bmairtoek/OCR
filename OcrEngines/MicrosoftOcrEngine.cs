using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Ocr;
using Windows.Storage;

namespace OCR.OcrEngines
{
    class MicrosoftOcrEngine: IOcrEngine
    {

        public MicrosoftOcrEngine()
        {

        }
        public async Task<string> RecognizeAsync(List<StorageFile> pickedFiles)
        {
            StringBuilder text = new StringBuilder();
            foreach (StorageFile file in pickedFiles)
            {
                OcrEngine engine = OcrEngine.TryCreateFromUserProfileLanguages();
                var stream = await file.OpenAsync(FileAccessMode.Read);
                var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(stream);
                var softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                OcrResult ocrResult = await engine.RecognizeAsync(softwareBitmap);
                foreach (OcrLine line in ocrResult.Lines)
                    text.Append(line.Text + "\n");
            }
            return text.ToString();
        }
    }
}
