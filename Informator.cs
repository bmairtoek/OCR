using System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace OCR
{
    class Informator
    {
        private static TextBlock output;
        private static ScrollViewer scroll;
        public Informator(TextBlock textBlock, ScrollViewer scroll)
        {
            Informator.output = textBlock;
            Informator.scroll = scroll;
        }  

        public static async void Log(string message)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Informator.output.Text += message;
                    Informator.output.Text += Environment.NewLine;
                    Informator.scroll.ChangeView(0.0f, Informator.scroll.ExtentHeight, 1.0f);
                }
            );
        }
        public static async void Clear()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Informator.output.Text = string.Empty;
                }
            );
        }
    }
}
