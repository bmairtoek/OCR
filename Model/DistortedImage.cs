using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR.Model
{
    class DistortedImage
    {
        public string distortion;
        public string folderPath;
        public float value;

        public DistortedImage(string distortion, string path, float value)
        {
            this.distortion = distortion;
            this.folderPath = path;
            this.value = value;
        }
    }
}
