using System;
using System.Reflection;
using BL;
using Blazored.LocalStorage;
using Blazored.Toast;
using ITVisions;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiracleList;
using MiracleList_Backend.Hubs;
using MLBlazorRCL.MainView;
using Web.Authentication;
using Web.Data;
using Web.Pages.CircuitList;

namespace Web;

public class Program
{

 public static void Main(string[] args)
 {
  IConfiguration Configuration;

  var builder = WebApplication.CreateBuilder(args);

  // für CultureController (vgl. https://docs.microsoft.com/de-de/aspnet/core/blazor/globalization-localization)
  builder.Services.AddControllers();

  // Add services to the container.
  builder.Services.AddRazorComponents()
      .AddInteractiveServerComponents();

  ConfigureServices(builder.Services);

  var app = builder.Build();

  // Configure the HTTP request pipeline.
  if (!app.Environment.IsDevelopment())
  {
   app.UseExceptionHandler("/Error");
   // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
   app.UseHsts();
  }

  Console.WriteLine("WebRootPath: " + app.Environment.WebRootPath);

  #region Load configuration
  // Get all configuration sources
  // NUGET: Microsoft.Extensions.Configuration.JSON
  // NUGET: Microsoft.Extensions.Configuration.EnvironmentVariables
  var configBuilder = new ConfigurationBuilder()
      .SetBasePath(app.Environment.ContentRootPath)
      .AddInMemoryCollection()
      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
      .AddJsonFile($"appsettings.{app.Environment.EnvironmentName}.json", optional: true)
      .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
      .AddEnvironmentVariables(); // only loads process env variables https://github.com/aspnet/Configuration/issues/721

  // build configuration now
  Configuration = configBuilder.Build();

  // get Connection String from configuration
  var CS = Configuration["ConnectionStrings:MiracleListDB"];

  // Inject connection string into DAL
  DA.Context.IsRuntime = true;
  DA.Context.ConnectionString = CS;
  #endregion

  app.UseHttpsRedirection();

  app.UseAntiforgery();

  #region Mehrsprachigkeit
  var supportedCultures = new[] { "en-US", "fr-FR", "de-DE" };
  var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
      .AddSupportedCultures(supportedCultures)
      .AddSupportedUICultures(supportedCultures);
  app.UseRequestLocalization(localizationOptions);
  #endregion

  #region Authentifizierung
  app.UseAuthentication();
  app.UseAuthorization();
  #endregion

  // für ASP.NET SignalR
  app.MapHub<MLHub>("/MLHub",
    signalRConnectionOptions =>
    {
     signalRConnectionOptions.AllowStatefulReconnects = true; // seit .NET 8.0
    });


  app.MapStaticAssets();
  app.MapRazorComponents<Pages.App>()
      .AddInteractiveServerRenderMode()
      .AddAdditionalAssemblies(
     typeof(MLBlazorRCL.Login.Login).Assembly,
     typeof(Samples.SamplesList).Assembly
     ); 

  // TODO: Wie geht das in .NET 8.0/9.0?
  //app.MapBlazorHub(opts =>
  //{
  // opts.WebSockets.CloseTimeout = new TimeSpan(1, 0, 0); // default: 5 sek
  //});

  app.MapControllers(); // für CultureController, siehe https://docs.microsoft.com/de-de/aspnet/core/blazor/globalization-localization


  app.Run();
 }

 public static void ConfigureServices(IServiceCollection services)
 {
  //System.Diagnostics.Debug.WriteLine("Liste der vorregistrierten Dienste in Blazor Server:");
  //foreach (var s in services)
  //{
  // System.Diagnostics.Debug.WriteLine(s.ServiceType.FullName + ": " + s.Lifetime);
  //}

  #region DI Mehrsprachigkeit
  services.AddLocalization();
  #endregion

  #region DI Konfiguration und Anwendungszustand
  services.AddScoped<IAppState, AppState>();
  #endregion

  // TODO Problem: https://github.com/dotnet/aspnetcore/issues/56820
  #region DI Authentifizierungsdienste
  services.AddScoped<IMLAuthenticationStateProvider, MLAuthenticationStateProvider2Tier>();
  services.AddScoped<AuthenticationStateProvider, MLAuthenticationStateProvider2Tier>();
  services.AddAuthentication()
      .AddScheme<MLAuthSchemeOptions, MLAuthSchemeHandler>(
          "ML",
          opts => { }
      );
  services.AddAuthorizationCore();
  services.AddCascadingAuthenticationState();
  #endregion

  #region DI Serverkommunikation
  services.AddScoped<IMiracleListProxy, MiracleListNoProxy>();
  #endregion

  #region DI für Blazor-Zusatzkomponenten
  services.AddBlazorContextMenu();
  services.AddBlazoredToast();

  // NUGET: Blazored.LocalStorage;
  // GITHUB:  https://github.com/Blazored/LocalStorage --> 
  services.AddBlazoredLocalStorage();

  // Alternativ möglich (einige API-Änderungen!)
  // NUGET: Blazor.Extensions.Storage
  // GITHUB: https://github.com/BlazorExtensions/Storage
  #endregion
  #region DI für SignalR Server ("Hub") für MiracleList Notifications
  services.AddSignalR().AddMessagePackProtocol().AddHubOptions<MLHub>(o => o.StatefulReconnectBufferSize = 120000); //100.000 ist der Defaultwert!
  #endregion

  #region DI für Beispiele außerhalb der MiracleList
  // Standardbeispiel von Microsoft
  services.AddSingleton<WeatherForecastService>();

  // für Circuit-Liste
  services.AddScoped<CircuitHandler, ITVisions.Blazor.Services.CircuitListCircuitHandler>();
  // für Circuit-Überwachung
  services.AddIdleCircuitHandler();

  // Für Session-State-Demo
  services.AddScoped<TypedSessionState>();
  services.AddScoped<GenericSessionState>();

  // für Nutzung HTTPContext
  services.AddHttpContextAccessor();

  // für HttpClient
  services.AddScoped<System.Net.Http.HttpClient>();

  // für Mehrspachigkeit
  services.AddLocalization();
  #endregion

  #region DI für sonstige Hilfsbibliotheken
  services.AddBlazorUtilForBlazorServer();
  #endregion

  #region Zusätzliche Komponenten, die MLBlazorRCL rendern soll
  // Datei-UploadChangeEventArgs bei TaskEdit.razor
  AdditionalComponents.TaskEditAdditionalComponent = typeof(MLBlazorRCL.Files.FilesFromFilesystem);
  #endregion
 }
}
