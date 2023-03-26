using System;
using System.Reflection;
using BL;
using Blazored.LocalStorage;
using Blazored.Toast;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiracleList;
using MiracleList_Backend.Hubs;
using Web.Data;

namespace Web;

// DEMO: 01b. BS-Startcode mit DI
public class Startup
{
 readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment Env;
 public IConfiguration Configuration { get; }

 public Startup(Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
 {
  this.Env = env;

  Console.WriteLine("WebRootPath: " + env.ContentRootPath);

  #region Load configuration
  // Get all configuration sources
  // NUGET: Microsoft.Extensions.Configuration.JSON
  // NUGET: Microsoft.Extensions.Configuration.EnvironmentVariables
  var builder = new ConfigurationBuilder()
      .SetBasePath(env.ContentRootPath)
      .AddInMemoryCollection()
      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
      .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
      .AddEnvironmentVariables(); // only loads process env variables https://github.com/aspnet/Configuration/issues/721

  // build configuration now
  Configuration = builder.Build();

  // get Connection String from configuration
  var CS = Configuration["ConnectionStrings:MiracleListDB"];

  // Inject connection string into DAL
  DA.Context.IsRuntime = true;
  DA.Context.ConnectionString = CS;
  #endregion
 }

 // This method gets called by the runtime. Use this method to add services to the container.
 // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
 public void ConfigureServices(IServiceCollection services)
 {
  System.Diagnostics.Debug.WriteLine("Liste der vorregistrierten Dienste in Blazor Server:");
  foreach (var s in services)
  {
   System.Diagnostics.Debug.WriteLine(s.ServiceType.FullName + ": " + s.Lifetime);
  }

  // für Mehrsprachigkeit
  services.AddLocalization();

  // für die Startseite _Host.cshtml
  services.AddRazorPages();
  // für CultureController (vgl. https://docs.microsoft.com/de-de/aspnet/core/blazor/globalization-localization)
  services.AddControllers();

  // für die Razor Components *.razor
  services.AddServerSideBlazor().AddCircuitOptions(o =>
  {
   if (this.Env.IsDevelopment()) //only add error details when debugging
   {
    o.DetailedErrors = true;
   }
  });

  #region DI Konfiguration und Anwendungszustand
  services.AddScoped<IAppState, AppState>();
  #endregion

  #region DI Authentifizierungsdienste
  services.AddScoped<IMLAuthenticationStateProvider, MLAuthenticationStateProvider2Tier>();
  services.AddScoped<AuthenticationStateProvider, MLAuthenticationStateProvider2Tier>();
  services.AddAuthorizationCore(); // sonst: System.InvalidOperationException: Cannot provide a value for property 'AuthorizationPolicyProvider' on type 'Microsoft.AspNetCore.Components.Authorization.AuthorizeView'. There is no registered service of type 'Microsoft.AspNetCore.Authorization.IAuthorizationPolicyProvider'.
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

  #region Di für SignalR Server ("Hub") für MiracleList Notifications
  services.AddSignalR().AddMessagePackProtocol();
  #endregion

  #region DI für Beispiele außerhalb der MiracleList
  // Standardbeispiel von Microsoft
  services.AddSingleton<WeatherForecastService>();

  // für Circuit-Liste
  services.AddScoped<CircuitHandler, ITVisions.Blazor.Services.CircuitListCircuitHandler>();

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
  services.AddScoped<BlazorUtil>();
  #endregion

  #region Zusätzliche Komponenten, die MLBlazorRCL rendern soll
  // Datei-UploadChangeEventArgs bei TaskEdit.razor
  AdditionalComponents.TaskEditAdditionalComponent = typeof(MLBlazorRCL.FilesFromFilesystem);
  #endregion
 }

 // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
 public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
 {
  if (env.IsDevelopment())
  {
   app.UseDeveloperExceptionPage();
  }
  else
  {
   app.UseExceptionHandler("/Home/Error");
   // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
   app.UseHsts();
  }

  app.UseHttpsRedirection();
  app.UseStaticFiles();

  #region Mehrsprachigkeit
  var supportedCultures = new[] { "en-US", "fr-FR", "de-DE" };
  var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
      .AddSupportedCultures(supportedCultures)
      .AddSupportedUICultures(supportedCultures);
  app.UseRequestLocalization(localizationOptions);
  #endregion

  app.UseRouting();

  app.UseEndpoints(endpoints =>
  {
   endpoints.MapControllers(); // für CulstureController, siehe https://docs.microsoft.com/de-de/aspnet/core/blazor/globalization-localization

   endpoints.MapBlazorHub(opts =>
   {
    opts.WebSockets.CloseTimeout = new TimeSpan(1, 0, 0); // default: 5 sek
   });

   endpoints.MapFallbackToPage("/_Host");
   // für ASP.NET SignarlR
   endpoints.MapHub<MLHub>("/MLHub");
  });
 }
}