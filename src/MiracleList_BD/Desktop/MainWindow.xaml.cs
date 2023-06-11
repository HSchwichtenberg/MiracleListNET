using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using BL;
using Blazored.LocalStorage; // NuGet Blazor.Extensions.Storage
using Blazored.Toast;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.Wpf;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiracleList;
using MLBlazorRCL.MainView;
using Web;

namespace BD.Desktop;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
 private IConfiguration Configuration { get; set; }
 private ServiceProvider ServiceProvider { get; set; }
 private HybridSharedState HybridSharedState { get; set; }
 private IAppState AppState { get; set; }
 private ServiceCollection services { get; set; }

 System.Windows.Threading.DispatcherTimer timer;

 public MainWindow()
 {
  services = new ServiceCollection();

  // Einrichten der Blazor-Anwendung
  services.AddWpfBlazorWebView();

#if DEBUG
  // Nutzung der Browser Developer Tools, nur beim Debugging
  services.AddBlazorWebViewDeveloperTools();
#endif

  #region Konfigurationsdatei einlesen
  var builder = new ConfigurationBuilder()
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), true);
  Configuration = builder.Build();
  services.AddSingleton<IConfiguration>(Configuration);
  #endregion

  #region Services für Shared Objects zwischen Desktop und Web
  services.AddSingleton<HybridSharedState>();
  #endregion

  #region Services für MiracleList-Web
  services.AddBlazorUtil();
  services.AddScoped<IMLAuthenticationStateProvider, MLAuthenticationStateProvider2Tier>();
  services.AddScoped<AuthenticationStateProvider, MLAuthenticationStateProvider2Tier>();
  services.AddAuthorizationCore();
  services.AddBlazoredLocalStorage();
  services.AddBlazorContextMenu();
  services.AddBlazoredToast();
  services.AddSingleton<IAppState, AppState>();
  services.AddScoped<IMiracleListProxy, MiracleListNoProxy>();
  #endregion

  #region DI für Beispiele außerhalb der MiracleList
  // Für Session-State-Demo
  services.AddScoped<TypedSessionState>();
  services.AddScoped<GenericSessionState>();

  // für HttpClient
  services.AddScoped<System.Net.Http.HttpClient>();

  // für Mehrspachigkeit
  services.AddLocalization();
  #endregion

  #region Zusätzliche Komponenten, die MLBlazorRCL rendern soll
  // Datei-UploadChangeEventArgs bei TaskEdit.razor
  AdditionalComponents.TaskEditAdditionalComponent = typeof(MLBlazorRCL.Files.FilesFromFilesystem);
  // Export-Schaltflächen
  AdditionalComponents.TaskExportAdditionalComponent = typeof(Web.Components.Export);
  #endregion

  ServiceProvider = services.BuildServiceProvider();
  Resources.Add("services", ServiceProvider);

  InitializeComponent();

  // Aktualisierung der Statusbar bei Zustandsänderungen in der Blazor-Anwendung
  HybridSharedState = ServiceProvider.GetService<HybridSharedState>();
  AppState = ServiceProvider.GetService<IAppState>();
  HybridSharedState.HybridSharedStateChanged += (o, e) => this.StatusBarUpdate();

  // Notwendig für die Möglichkeit, den Kopfbereich zu beeinflussen
  var rh = new Microsoft.AspNetCore.Components.WebView.Wpf.RootComponent();
  rh.ComponentType = typeof(HeadOutlet);
  rh.Selector = "head::after";
  C_WebView.RootComponents.Add(rh);

  HybridSharedState.HostControl = C_WebView; // Erlaubt der Blazor-App, sein Host-Control zu sehen :-)

  // DEMO: BD-05 Events des BlazorWebView Controls
  C_WebView.BlazorWebViewInitializing += (object o, BlazorWebViewInitializingEventArgs e) =>
  {
   e.EnvironmentOptions = new Microsoft.Web.WebView2.Core.CoreWebView2EnvironmentOptions();
   //e.EnvironmentOptions.AllowSingleSignOnUsingOSPrimaryAccount = true; // https://github.com/dotnet/maui/issues/5512
  };

  C_WebView.BlazorWebViewInitialized += (object o, BlazorWebViewInitializedEventArgs e) =>
  {

   e.WebView.ZoomFactor = 1;
   // Zoomen deaktivieren
   //AppState.WebView.CoreWebView2.Settings.IsZoomControlEnabled = false;
   // oder wenn man dem Benutzer sagen will, dass er zoomen nicht darf:
   e.WebView.ZoomFactorChanged += (o, e2) =>
   {
    e.WebView.ZoomFactor = 1;
    MessageBox.Show("Zoomen ist hier nicht erlaubt!");
   };
  };

  // ggf. Notwendig für MSIX
  //C_WebView.HostPage = System.AppContext.BaseDirectory + @"\web\wwwroot\index.html"; // für MSIX, sonst einfach: @"web\wwwroot\index.html";

  // Zeitgesteuerte Aktualisierung der Statusbar
  timer = new System.Windows.Threading.DispatcherTimer();
  timer.Tick += new EventHandler((s, e) => StatusBarUpdate());
  timer.Interval = new TimeSpan(0, 0, 1);
  timer.Start();

  // Statusbar erstmalig befüllen
  StatusBarUpdate();
 }

 private void WebView_ZoomFactorChanged1(object sender, EventArgs e)
 {
  throw new NotImplementedException();
 }

 private void WebView_ZoomFactorChanged(object sender, EventArgs e)
 {
  throw new NotImplementedException();
 }

 private void About_Click(object sender, RoutedEventArgs e)
 {
  MessageBox.Show(
      owner: this,
      messageBoxText: new ITVisions.EnvInfo().GetString(),
      caption: "WPF-Rahmenanwendung für MiracleList Blazor Desktop");
 }

 /// <summary>
 /// Anzeige in der WPF-Statuszeile aktualisieren
 /// </summary>
 public void StatusBarUpdate()
 {
  this.C_Status.Content = $"{System.Runtime.InteropServices.RuntimeInformation.OSDescription} | {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription} | Blazor Desktop {FileVersionInfo.GetVersionInfo(typeof(BlazorWebView).Assembly.Location).FileVersion} | Process #{System.Environment.ProcessId} | {System.IO.Path.GetFileName(System.Environment.ProcessPath)} | {System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024} MB | {DateTime.Now.ToLongTimeString()} | {(AppState.IsLoggedIn ? AppState.Username : "Kein Benutzer")} | {HybridSharedState.Location ?? "Starting..."}";
  this.C_Status.ToolTip = "Letzte Aktualisierung in Managed Thread #" + System.Threading.Thread.CurrentThread.ManagedThreadId;
 }
}