
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BD.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class WPFApp : Application
{

 private void Application_Startup(object sender, StartupEventArgs e)
 {

  // Sprache festlegen beim Anwendungstart
  //var culture = new CultureInfo("en-US");
  //CultureInfo.DefaultThreadCurrentCulture = culture;
  //CultureInfo.DefaultThreadCurrentUICulture = culture;
  //Thread.CurrentThread.CurrentCulture = culture;
  //Thread.CurrentThread.CurrentCulture = culture;

  SetupExceptionHandling();

  if (!WebView2Helper.IsWebView2Installed(out string version))
  {
   var result = MessageBox.Show($"❌ Blazor Desktop erfordert Komponente 'Microsoft Edge WebView2'. Diese wurde auf dem System nicht gefunden!\n\nDie Installation des Edge-Browsers selbst reicht nicht!\n\nSiehe https://developer.microsoft.com/de-de/microsoft-edge/webview2\n\nMöchten Sie die Komponente jetzt herunterladen?", $"{System.Diagnostics.Process.GetCurrentProcess().ProcessName}: WebView2 not found", MessageBoxButton.YesNo);
   if (result == MessageBoxResult.Yes)
   {
    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
    {
     FileName = "https://developer.microsoft.com/de-de/microsoft-edge/webview2/",
     UseShellExecute = true
    });
   }
   Environment.Exit(1);
  }

 }

 private void SetupExceptionHandling()
 {
  AppDomain.CurrentDomain.UnhandledException += (s, e) =>
  {
   MessageBox.Show((e.ExceptionObject).ToString(), $"{System.Diagnostics.Process.GetCurrentProcess().ProcessName}: AppDomain.CurrentDomain.UnhandledException");
  };

  DispatcherUnhandledException += (s, e) =>
  {
   {
    MessageBox.Show(e.Exception.ToString(), $"{System.Diagnostics.Process.GetCurrentProcess().ProcessName}: DispatcherUnhandledException");
    e.Handled = true;
   };
  };

  TaskScheduler.UnobservedTaskException += (s, e) =>
  {
   MessageBox.Show(e.Exception.ToString(), $"{System.Diagnostics.Process.GetCurrentProcess().ProcessName}: TaskScheduler.UnobservedTaskException");
   e.SetObserved();
  };
 }
}