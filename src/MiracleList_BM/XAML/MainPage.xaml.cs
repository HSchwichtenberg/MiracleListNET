using System.Diagnostics;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.Maui;
using MiracleList;

namespace BM;

public partial class MainPage : ContentPage {
 public HybridSharedState HybridSharedState { get; set; }
 public IAppState AppState { get; set; }
 IDispatcherTimer timer;

 public MainPage(IAppState appstate, HybridSharedState hybridSharedState) {
  InitializeComponent();

  AppState = appstate;
  HybridSharedState = hybridSharedState;
  // Aktualisierung der Statusbar bei Zustandsänderungen in der Blazor-Anwendung
  HybridSharedState.HybridSharedStateChanged += (o, e) => this.StatusBarUpdate();

  // Zeitgesteuerte Aktualisierung der Statusbar
  timer = Dispatcher.CreateTimer();
  timer.Tick += new EventHandler((s, e) => StatusBarUpdate());
  timer.Interval = new TimeSpan(0, 0, 1);
  timer.Start();

  // Events des BlazorWebView Controls
  C_WebView.BlazorWebViewInitializing += (object o, BlazorWebViewInitializingEventArgs e) => {
   //e.EnvironmentOptions = new Microsoft.Web.WebView2.Core.CoreWebView2EnvironmentOptions();
   ////e.EnvironmentOptions.AllowSingleSignOnUsingOSPrimaryAccount = true; // https://github.com/dotnet/maui/issues/5512
  };

  C_WebView.BlazorWebViewInitialized += (object o, BlazorWebViewInitializedEventArgs e) => {
   HybridSharedState.HostControl = e.WebView;

   #region ZOOMEN
#if Android
      //e.WebView.CanZoomIn = false;
      //e.WebView.CanZoomOut = false;
#endif

#if WINDOWS

#endif

   // Zoomen deaktivieren
   //  this.C_WebView.WebView.CoreWebView2.Settings.IsZoomControlEnabled = false;
   // oder wenn man dem Benutzer sagen will, dass er zoomen nicht darf:
   //e.WebView.ZoomFactorChanged += (o, e2) =>
   //{
   // e.WebView.ZoomFactor = 1;
   // MessageBox.Show("Zoomen ist hier nicht erlaubt!");
   //};
  };
  #endregion

  // Statusbar erstmalig befüllen
  StatusBarUpdate();
 }

 private async void About_Clicked(object sender, EventArgs e) {
  await DisplayAlert(".NET MAUI-Rahmenanwendung für MiracleList Blazor MAUI", new ITVisions.EnvInfo().GetString(), "OK");
 }

 public void StatusBarUpdate() {


  if (this.Width>500) { this.C_Status.FontSize = 13; }
  else { this.C_Status.FontSize = 9; }
  this.C_Status.Text = $"{System.Runtime.InteropServices.RuntimeInformation.OSDescription} | {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription} | Blazor MAUI {FileVersionInfo.GetVersionInfo(typeof(BlazorWebView).Assembly.Location).FileVersion} | Process #{System.Environment.ProcessId} {System.IO.Path.GetFileName(System.Environment.ProcessPath)} | Thread #{System.Threading.Thread.CurrentThread.ManagedThreadId} | {System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024} MB | {DateTime.Now.ToLongTimeString()} | {(AppState.IsLoggedIn ? AppState.Username : "Kein Benutzer")} | {HybridSharedState.Location ?? "Starting..."}";

  // seit .NET 7.0 gibt es auch Tooltips in .NET MAUI
  ToolTipProperties.SetText(this.C_Status, "Letzte Aktualisierung am " + DateTime.Now + " in Thread #" + System.Threading.Thread.CurrentThread.ManagedThreadId);
 }
}