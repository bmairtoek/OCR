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

        public Task<string> RecognizeAsync(List<StorageFile> pickedFiles)
        {
            return new Task<string>(() => { return string.Empty; });
        }
    }
}
