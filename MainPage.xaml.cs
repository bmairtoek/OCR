﻿using System;
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
using System.Threading;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OCR
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<StorageFile> pickedFiles = new List<StorageFile>();
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Pick_Files(object sender, RoutedEventArgs e)
        {
            new ImageProcessing.Blur().Execute();

            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
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
                    this.files.Text += file.Path + " \n";
                }
            }
            else
            {
                this.files.Text = "Operation cancelled.";
            }
        }

        private async void Recognize_Text(object sender, RoutedEventArgs e)
        {
            this.outputBlock.Text = string.Empty;
            if (this.pickedFiles.Count > 0)
            {
                IOcrEngine ocrEngine = null;
                switch (this.GetSelectedRadioButtonTag())
                {
                    case "Microsoft":
                        ocrEngine = OcrEngineFactory.createMicrosoftOcrEngine();
                        break;
                    case "Google":
                        ocrEngine = OcrEngineFactory.createGoogleOcrEngine();
                        break;
                }
                string newText = string.Empty;
                await Task.Run(async() => { newText = await ocrEngine.RecognizeAsync(pickedFiles); });
                this.outputBlock.Text = newText;
            }
            else
            {
                this.outputBlock.Text = "Pick files first";
            }
        }

        private string GetSelectedRadioButtonTag()
        {
            var radioButtons = engines.Children.OfType<RadioButton>();
            foreach (var radioButton in radioButtons)
            {
                if (radioButton.IsChecked == true)
                {
                    return radioButton.Tag as string;
                }
            }
            throw new Exception("radio button no selected");
        }
    }
}
