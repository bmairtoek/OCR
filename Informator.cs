using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

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
                    output.Text += "\n";
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
    }
}
