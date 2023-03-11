
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;

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

		AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
											{
												MessageBox.Show(error.ExceptionObject.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
											};
	}
}