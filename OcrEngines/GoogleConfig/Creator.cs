using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Vision.v1;
using Google.Apis.Vision.v1.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace OCR.OcrEngines.GoogleConfig
{
    class Creator
    {
        public BatchAnnotateImagesResponse SendGoogleOcrRequest(List<StorageFile> files)
        {
            VisionService service = this.CreateService();
            var annotate = service.Images.Annotate(this.CreateRequest(files));
            BatchAnnotateImagesResponse batchAnnotateImagesResponse = annotate.Execute();
            return batchAnnotateImagesResponse;
        }
        private GoogleCredential CreateCredential()
        {
            string keyPath = Windows.ApplicationModel.Package.Current.InstalledLocation.Path + Constants.JsonKeyPath;
            using (var stream = new FileStream(keyPath, FileMode.Open, FileAccess.Read))
            {
                string[] scopes = { VisionService.Scope.CloudPlatform };
                var credential = GoogleCredential.FromStream(stream);
                credential = credential.CreateScoped(scopes);
                return credential;
            }
        }
        private VisionService CreateService()
        {
            GoogleCredential credential = this.CreateCredential();
            var service = new VisionService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "OCR",
                GZipEnabled = true,
            });
            return service;
        }
        private BatchAnnotateImagesRequest CreateRequest(List<StorageFile> files)
        {
            BatchAnnotateImagesRequest batchRequest = new BatchAnnotateImagesRequest
            {
                Requests = new List<AnnotateImageRequest>()
            };
            foreach (StorageFile file in files)
            {
                batchRequest.Requests.Add(new AnnotateImageRequest()
                {
                    Features = new List<Feature>() { new Feature() { Type = "TEXT_DETECTION", MaxResults = 1 }, },
                    ImageContext = new ImageContext() { LanguageHints = Constants.languages },
                    Image = new Image() { Content = Convert.ToBase64String(this.ConvertFileToByteArray(file).Result) }
                });
            }
            return batchRequest;
        }

        private async Task<byte[]> ConvertFileToByteArray(StorageFile file)
        {
            byte[] fileBytes = null;
            if (file == null) return null;
            using (var stream = await file.OpenReadAsync())
            {
                fileBytes = new byte[stream.Size];
                using (var reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }
            return fileBytes;
        }
    }
}
