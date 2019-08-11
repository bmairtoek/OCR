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
        public async Task<string> RecognizeAsync(List<StorageFile> pickedFiles)
        {
            StringBuilder output = new StringBuilder();
            var responses = new GoogleConfig.Creator().SendGoogleOcrRequest(pickedFiles);
            foreach(var response in responses.Responses)
            {
                output.Append(response.FullTextAnnotation.Text + "\n");
            }
            return output.ToString();
        }
    }
}
