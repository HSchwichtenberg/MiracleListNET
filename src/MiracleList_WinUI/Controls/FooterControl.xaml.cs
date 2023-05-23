using System;
using System.IO;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.UI.Windowing;
using Microsoft.Extensions.DependencyInjection;
using MiracleList;

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
                $"{RuntimeInformation.OSDescription} | {RuntimeInformation.FrameworkDescription} | WinUI 3 | Windows App SDK {FileVersionInfo.GetVersionInfo(typeof(Microsoft.WindowsAppSDK.Release).Assembly.Location).FileVersion} | Process #{Environment.ProcessId} {Path.GetFileName(System.Environment.ProcessPath)} | Thread #{System.Threading.Thread.CurrentThread.ManagedThreadId} | {System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024} MB | {DateTime.Now.ToLongTimeString()} | {(appState?.Username ?? "Kein Benutzer")}";
        }
    }
}
