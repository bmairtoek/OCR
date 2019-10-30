using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Graphics.Imaging;

namespace OCR.OcrEngines
{
    class MicrosoftOcrEngine: IOcrEngine
    {
        public async Task<List<string>> RecognizeAsync(List<StorageFile> pickedFiles)
        {
            List<string> results = new List<string>();
            // OcrEngine engine = OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language("en-US"));
            OcrEngine engine = OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language("pl"));
            int i = 0;
            foreach (StorageFile file in pickedFiles)
            {
                Informator.Log("    OCR - "+i+" out of "+ pickedFiles.Count);
                StringBuilder text = new StringBuilder();
                var stream = await file.OpenAsync(FileAccessMode.Read);
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                OcrResult ocrResult = await engine.RecognizeAsync(softwareBitmap);
                foreach (OcrLine line in ocrResult.Lines)
                    text.Append(line.Text + "\n");
                results.Add(text.ToString());
                i++;
            }
            return results;
        }
    }
}
