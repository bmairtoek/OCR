using System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace OCR
{
    class Informator
    {
        private TextBlock output;
        public Informator(TextBlock textBlock)
        {
            this.output = textBlock;
        }  

        public async void Log(string message)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    output.Text += message;
                    output.Text += Environment.NewLine;
                    this.TextChanged();
                }
            );
        }
        public async void Clear()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    output.Text = string.Empty;
                }
            );
        }

        private void TextChanged()
        {
            var grid = (Grid)VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.output));
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(grid); i++)
            {
                object obj = VisualTreeHelper.GetChild(grid, i);
                if (!(obj is ScrollViewer)) continue;
                ((ScrollViewer)obj).ChangeView(0.0f, ((ScrollViewer)obj).ExtentHeight, 1.0f);
                break;
            }
        }
    }
}
