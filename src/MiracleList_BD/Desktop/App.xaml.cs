
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

 }

 private void SetupExceptionHandling()
 {
  AppDomain.CurrentDomain.UnhandledException += (s, e) =>
  {
   MessageBox.Show((e.ExceptionObject).ToString(), " AppDomain.CurrentDomain.UnhandledException");
  };

  DispatcherUnhandledException += (s, e) =>
  {
   {
    MessageBox.Show(e.Exception.ToString(), "DispatcherUnhandledException");
    e.Handled = true;
   };
  };

  TaskScheduler.UnobservedTaskException += (s, e) =>
  {
   MessageBox.Show(e.Exception.ToString(), "TaskScheduler.UnobservedTaskException");
   e.SetObserved();
  };
 }
}