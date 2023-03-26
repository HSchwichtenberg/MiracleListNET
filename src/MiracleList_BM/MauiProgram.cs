using System.Reflection;
using Blazored.LocalStorage;
using Blazored.Toast;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MiracleList;
using MLBlazorRCL.MainView;
using Web;

namespace BM;

public static class MauiProgram
{
 public static MauiApp CreateMauiApp()
 {
  // Standard-Startcode einer .NET-MAUI-Anwendung
  var builder = MauiApp.CreateBuilder();
  builder
   .UseMauiApp<App>()
   .ConfigureFonts(fonts =>
   {
    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
   });

  #region Konfiguration lesen
  var a = Assembly.GetExecutingAssembly();
  using var stream = a.GetManifestResourceStream("BM.appsettings.json");
  var config = new ConfigurationBuilder()
     .AddJsonStream(stream)
     .Build();
  builder.Configuration.AddConfiguration(config);
  #endregion

  #region DI konfigurieren
  var services = builder.Services;

  #region DI Integration von Blazor
  services.AddMauiBlazorWebView();
#if DEBUG
  services.AddBlazorWebViewDeveloperTools();
#endif
  #endregion

  #region Services für Shared Objects zwischen MAUI und Web
  services.AddSingleton<HybridSharedState>();
  #endregion

  #region Services für MiracleList-Web
  services.AddSingleton<IAppState, AppState>();
  services.AddOptions(); // notwendig für AuthenticationStateProvider
  services.AddScoped<IMLAuthenticationStateProvider, MLAuthenticationStateProvider3Tier>();
  services.AddScoped<AuthenticationStateProvider, MLAuthenticationStateProvider3Tier>();
  services.AddAuthorizationCore();
  services.AddSingleton(new HttpClient()); // für MiracleListProxy
  services.AddScoped<MiracleList.IMiracleListProxy, MiracleList.MiracleListProxy>();
  services.AddBlazoredToast();
  services.AddBlazoredLocalStorage();
  services.AddBlazorContextMenu();
  services.AddScoped<BlazorUtil>();
  services.AddScoped<IHttpContextAccessor, HttpContextAccessorDummy>();   // braucht BlazorUtil
  #endregion

  #region DI für Beispiele außerhalb der MiracleList
  // Für Session state
  services.AddScoped<TypedSessionState>();
  services.AddScoped<GenericSessionState>();
  #endregion

  #region Zusätzliche Komponenten, die MLBlazorRCL rendern soll
  // Export-Schaltflächen
  AdditionalComponents.TaskExportAdditionalComponent = typeof(Web.Components.Export);
  // Datei-UploadChangeEventArgs bei TaskEdit.razor
  AdditionalComponents.TaskEditAdditionalComponent = typeof(MLBlazorRCL.Files.FilesFromWebservice);
  #endregion

  #endregion
  return builder.Build();
 }
}