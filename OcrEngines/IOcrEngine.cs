using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace OCR.OcrEngines
{
    interface IOcrEngine
    {
        Task<string> RecognizeAsync(List<StorageFile> pickedFiles);
    }
}
