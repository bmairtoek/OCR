namespace OCR.OcrEngines
{
    static class OcrEngineFactory
    {
        public static MicrosoftOcrEngine createMicrosoftOcrEngine()
        {
            return new MicrosoftOcrEngine();
        }

        public static GoogleOcrEngine createGoogleOcrEngine()
        {
            return new GoogleOcrEngine();
        }
    }
}
