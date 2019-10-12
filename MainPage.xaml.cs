using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Ocr;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using OCR.OcrEngines;
using OCR.Enums;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using OCR.ImageProcessing;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OCR
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<StorageFile> pickedFiles = new List<StorageFile>();
        private Informator informator;
        public MainPage()
        {
            this.InitializeComponent();
            this.informator = new Informator(outputBlock);
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
            this.informator.Clear();
            this.ClearTempFolder();

            List<IImageProcessor> selectedDistorsions = this.GetSelectedDistorsions();
            int samples = Convert.ToInt32(SamplesSlider.Value);
            await Task.Run(()=>CreateDistortedImages(selectedDistorsions, samples));

            // run recognition for each selected engine
            List<IOcrEngine> selectedEngines = this.GetSelectedEngines();
            foreach (IOcrEngine engine in selectedEngines)
            {
                string newText = string.Empty;
                await Task.Run(async () => { newText = await engine.RecognizeAsync(pickedFiles); });
                this.outputBlock.Text = newText;
            }
        }
        private void ClearTempFolder()
        {
            string tmpPath = Path.GetTempPath();
            string[] files = Directory.GetFiles(tmpPath, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }

        private void CreateDistortedImages(List<IImageProcessor> selectedDistorsions, int samples)
        {
            string tempPath = Path.GetTempPath();
            Debug.WriteLine(tempPath);
            foreach (StorageFile file in this.pickedFiles)
            {
                string fileFolderPath = Directory.CreateDirectory(Path.Combine(tempPath, Path.GetFileName(file.Path))).FullName;
                foreach (IImageProcessor distorsion in selectedDistorsions)
                {
                    string distortionFolderPath = Directory.CreateDirectory(Path.Combine(fileFolderPath, distorsion.GetType().ToString())).FullName;
                    if (samples == 1)
                    {
                        distorsion.Execute(distorsion.MaximalValue, file, distortionFolderPath);
                    }
                    else
                    {
                        for (int i = 1; i <= samples; i++)
                        {
                            this.informator.Log("Executing "+distorsion.GetType().ToString()+" - "+i+" out of "+samples);
                            float value = distorsion.MinimalValue + (distorsion.MaximalValue - distorsion.MinimalValue) / samples * i;
                            distorsion.Execute(value, file, distortionFolderPath);
                        }
                    }
                }
                this.informator.Log("Finished distorting image: " + Path.GetFileName(file.Path));
            }
            this.informator.Log("Finished creation of distorted images");
        }
        private List<IImageProcessor> GetSelectedDistorsions()
        {
            List<IImageProcessor> selectedProcessors = new List<IImageProcessor>();
            var toggleSwitches = distortions1.Children.OfType<ToggleSwitch>().Concat(distortions2.Children.OfType<ToggleSwitch>());
            foreach (var toggleSwitch in toggleSwitches)
            {
                if (toggleSwitch.IsOn == false)
                    continue;

                if (toggleSwitch.Tag.ToString().Equals(Distortions.Blur))
                    selectedProcessors.Add(ImageProcessorFactory.CreateBlurProcessor());
                else if (toggleSwitch.Tag.ToString().Equals(Distortions.Rotation))
                    selectedProcessors.Add(ImageProcessorFactory.CreateRotateProcessor());
                else if (toggleSwitch.Tag.ToString().Equals(Distortions.KeystoneEffect))
                    selectedProcessors.Add(ImageProcessorFactory.CreateKeystoneEffent());
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
    }
}
