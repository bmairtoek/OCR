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
        public static KeystoneEffect CreateKeystoneEffect()
        {
            return new KeystoneEffect();
        }
        public static LightBrightness CreateLightBrightness()
        {
            return new LightBrightness();
        }
        public static DarkBrightness CreateDarkBrightness()
        {
            return new DarkBrightness();
        }
        public static Contrast CreateContrast()
        {
            return new Contrast();
        }
    }
}
