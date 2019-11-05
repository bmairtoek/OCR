using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;
using OCR.OcrEngines;
using OCR.Enums;
using OCR.ImageProcessing;
using OCR.TextComparator;
using OCR.Model;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OCR
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<StorageFile> pickedFiles = new List<StorageFile>();
        private Dictionary<string, List<DistortedImage>> distortedImages = new Dictionary<string, List<DistortedImage>>();
        private Dictionary<string, StorageFile> originalImages = new Dictionary<string, StorageFile>();
        public MainPage()
        {
            this.InitializeComponent();
            new Informator(outputBlock, outputScroller);
        }

        private async void Pick_Files(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var files = await picker.PickMultipleFilesAsync();
            this.pickedFiles.Clear();
            this.files.Text = string.Empty;
            if (files.Count > 0)
            {
                foreach (StorageFile file in files)
                {
                    this.pickedFiles.Add(file);
                }
            }
            this.files.Text = pickedFiles.Count.ToString();
        }

        private async void Recognize_Text(object sender, RoutedEventArgs e)
        {
            if (this.pickedFiles.Count <= 0)
                return;
            Informator.Clear();
            this.ClearTempFolder();
            this.distortedImages.Clear();
            this.originalImages.Clear();

            List<IImageProcessor> selectedDistorsions = this.GetSelectedDistorsions();
            int samples = Convert.ToInt32(SamplesSlider.Value);
            await Task.Run(()=>CreateDistortedImages(selectedDistorsions, samples));

            // run recognition for each selected engine
            List<IOcrEngine> selectedEngines = this.GetSelectedEngines();
            foreach (IOcrEngine engine in selectedEngines)
            {
                var keys = distortedImages.Keys;
                for (int i = 0; i < keys.Count; i++)
                {
                    string resultsPath = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), keys.ElementAt(i), "OCR-output")).FullName;
                    List<StorageFile> filesToOcr = await this.GetFiles(keys.ElementAt(i));
                    List<string> ocrOutput = null;
                    await Task.Run(async () => {
                        ocrOutput = await engine.RecognizeAsync(filesToOcr);
                    });
                    File.WriteAllText(Path.Combine(resultsPath, "originalImage" + ".txt"), ocrOutput[0]);
                    for (int j=1; j<ocrOutput.Count; j++)
                    {
                        File.WriteAllText(Path.Combine(resultsPath, this.distortedImages[keys.ElementAt(i)][j-1].distortion + "_" + filesToOcr[j].Name + ".txt"), ocrOutput[j] );
                    }
                    this.AnalizeOcrResults(engine.GetType().ToString(), keys.ElementAt(i), ocrOutput);
                    Informator.Log("Finished analizing image: " + keys.ElementAt(i));
                    Informator.Log("");
                }
                Informator.Log(engine.GetType().ToString()+" has finished");
                Informator.Log("");
            }
            Informator.Log("");
            Informator.Log("FINISHED");
            Informator.Log("Results can be found at: "+Path.GetTempPath());

        }
        private async Task<List<StorageFile>> GetFiles(string key)
        {
            List<StorageFile> files = new List<StorageFile> { originalImages[key] };
            foreach(DistortedImage image in distortedImages[key])
            {
                files.Add(await StorageFile.GetFileFromPathAsync(Path.Combine(image.folderPath, image.value.ToString() + ".png")));
            }
            return files;
        }
        private void ClearTempFolder()
        {
            DirectoryInfo tempDir = new DirectoryInfo(Path.GetTempPath());

            foreach (FileInfo file in tempDir.GetFiles())
                file.Delete();

            foreach (DirectoryInfo dir in tempDir.GetDirectories())
                dir.Delete(true);
        }

        private void CreateDistortedImages(List<IImageProcessor> selectedDistorsions, int samples)
        {
            string tempPath = Path.GetTempPath();
            Debug.WriteLine(tempPath);
            foreach (StorageFile file in this.pickedFiles)
            {
                string fileFolderPath = Directory.CreateDirectory(Path.Combine(tempPath, Path.GetFileName(file.Path))).FullName;
                distortedImages[Path.GetFileName(file.Path)] = new List<DistortedImage>();
                originalImages[Path.GetFileName(file.Path)] = file;
                foreach (IImageProcessor distorsion in selectedDistorsions)
                {
                    string distortionFolderPath = Directory.CreateDirectory(Path.Combine(fileFolderPath, distorsion.GetType().ToString())).FullName;
                    if (samples == 1)
                    {
                        distorsion.Execute(distorsion.lastValue, file, distortionFolderPath);
                    }
                    else
                    {
                        for (int i = 1; i <= samples; i++)
                        {
                            Informator.Log("   Executing "+distorsion.GetType().ToString()+" - "+i+" out of "+samples);
                            float value = distorsion.startValue + (distorsion.lastValue - distorsion.startValue) / samples * i;
                            distorsion.Execute(value, file, distortionFolderPath);
                            distortedImages[Path.GetFileName(file.Path)].Add(new DistortedImage(distorsion.GetType().ToString(), distortionFolderPath, value));
                        }
                    }
                }
                Informator.Log("Finished distorting image: " + Path.GetFileName(file.Path));
                Informator.Log("");
            }
            Informator.Log("Finished creation of distorted images");
            Informator.Log("");
        }
        private List<IImageProcessor> GetSelectedDistorsions()
        {
            List<IImageProcessor> selectedProcessors = new List<IImageProcessor>();
            var toggleSwitches = distortions1.Children.OfType<ToggleSwitch>().Concat(distortions2.Children.OfType<ToggleSwitch>());
            foreach (var toggleSwitch in toggleSwitches)
            {
                if (toggleSwitch.IsOn == false)
                    continue;

                switch(toggleSwitch.Tag.ToString()) {
                    case Distortions.Blur:
                        selectedProcessors.Add(ImageProcessorFactory.CreateBlurProcessor());
                        break;
                    case Distortions.Rotation:
                        selectedProcessors.Add(ImageProcessorFactory.CreateRotateProcessor());
                        break;
                    case Distortions.KeystoneEffect:
                        selectedProcessors.Add(ImageProcessorFactory.CreateKeystoneEffect());
                        break;
                    case Distortions.LightBrightness:
                        selectedProcessors.Add(ImageProcessorFactory.CreateLightBrightness());
                        break;
                    case Distortions.DarkBrightness:
                        selectedProcessors.Add(ImageProcessorFactory.CreateDarkBrightness());
                        break;
                    case Distortions.Contrast:
                        selectedProcessors.Add(ImageProcessorFactory.CreateContrast());
                        break;
                    default:
                        break;
                }
            }
            return selectedProcessors;
        }

        private List<IOcrEngine> GetSelectedEngines()
        {
            List<IOcrEngine> selectedEngines = new List<IOcrEngine>();
            var toggleSwitches = engines.Children.OfType<ToggleSwitch>();
            foreach (var toggleSwitch in toggleSwitches)
            {
                if (toggleSwitch.IsOn == true)
                {
                    if (toggleSwitch.Tag.ToString().Equals(Engines.Microsoft))
                        selectedEngines.Add(OcrEngineFactory.CreateMicrosoftOcrEngine());
                    else if (toggleSwitch.Tag.ToString().Equals(Engines.Google))
                        selectedEngines.Add(OcrEngineFactory.CreateGoogleOcrEngine());
                }
            }
            return selectedEngines;
        }

        private void AnalizeOcrResults(string engineName, string imageName, List<string> ocrResults)
        {
            StringBuilder results = new StringBuilder();
            string originalImageResult = ocrResults[0];
            for (int i=1; i<ocrResults.Count; i++)
            {
                DistortedImage currentImage = distortedImages[imageName][i - 1];
                results.Append(imageName);
                results.Append(" | ");
                results.Append(currentImage.distortion);
                results.Append(" | ");
                results.Append(currentImage.value);
                results.Append(" | ");
                results.Append(LevenshteinDistance.Calculate(originalImageResult, ocrResults[i]).ToString());
                results.Append("/");
                results.Append((originalImageResult.Length).ToString()) ;
                ;
                results.Append(Environment.NewLine);
            }
            File.WriteAllText(Path.Combine(Path.GetTempPath(), imageName+"_"+engineName+".txt"), results.ToString());
        }
    }
}
