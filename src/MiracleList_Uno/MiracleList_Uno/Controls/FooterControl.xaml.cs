using System.Diagnostics;
using System.Runtime.InteropServices;
using MiracleList;
using MiracleList_Uno;

namespace MiracleList_WinUI.Controls
{
    public sealed partial class FooterControl : UserControl
    {
        public FooterControl()
        {
            this.InitializeComponent();

            UpdateStatusText();
        }

        public void UpdateStatusText()
        {
            var appState = ((App)App.Current)
              .ServiceProvider
              .GetService<IAppState>();

            statusTextBlock.Text =
                $"{RuntimeInformation.OSDescription} | {RuntimeInformation.FrameworkDescription} | WinUI 3 and Uno | {DateTime.Now.ToLongTimeString()} | {(appState?.Username ?? "Kein Benutzer")}";
        }
    }
}
