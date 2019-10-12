namespace OCR.OcrEngines
{
    static class OcrEngineFactory
    {
        public static MicrosoftOcrEngine CreateMicrosoftOcrEngine()
        {
            return new MicrosoftOcrEngine();
        }

        public static GoogleOcrEngine CreateGoogleOcrEngine()
        {
            return new GoogleOcrEngine();
        }
    }
}
