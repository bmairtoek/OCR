using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace OCR.OcrEngines
{
    class GoogleOcrEngine: IOcrEngine
    {
        public async Task<List<string>> RecognizeAsync(List<StorageFile> pickedFiles)
        {
            List<string> output = new List<string>();
            var responses = new GoogleConfig.Creator().SendGoogleOcrRequest(pickedFiles);
            foreach(var response in responses.Responses)
            {
                output.Add(response.FullTextAnnotation.Text);
            }
            return output;
        }
    }
}
