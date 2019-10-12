using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace OCR.ImageProcessing
{
    interface IImageProcessor
    {
        float MinimalValue { get; }
        float MaximalValue { get; }
        // TODO file path
        void Execute(float value, StorageFile inputFile, string outputFolderPath);
    }
}
