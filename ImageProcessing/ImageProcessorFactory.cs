﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR.ImageProcessing
{
    static class ImageProcessorFactory
    {
        public static Blur CreateBlurProcessor()
        {
            return new Blur();
        }
        public static Rotate CreateRotateProcessor()
        {
            return new Rotate();
        }
        public static KeystoneEffect CreateKeystoneEffent()
        {
            return new KeystoneEffect();
        }
    }
}
