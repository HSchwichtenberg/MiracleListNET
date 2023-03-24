using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Blazor.Extensions.Logging;
using Blazored.LocalStorage;
using Blazored.Toast;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using MiracleList;

namespace Web;

/// <summary>
/// Die Program.cs ist bewusst mit klassischen OOP-Pattern aufgebaut
/// und verzichtet auf die meisten Kurzschreibweisen, die Microsoft in den Projektvorlagen zu .NET 6 eingeführt hat
/// </summary>
public class Program
{
 // DEMO: 01a. BW-Startcode mit DI
 public static async Task Main(string[] args)
 {
  Console.WriteLine("Blazor WebAssembly Main()");
  Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
  Console.WriteLine("App-Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

  var builder = WebAssemblyHostBuilder.CreateDefault(args);
  builder.RootComponents.Add<App>("#app");
  AddServices(builder);

  Console.WriteLine("Environment: " + builder.HostEnvironment.Environment);
  Console.WriteLine("BaseAddress: " + builder.HostEnvironment.BaseAddress);
  Console.WriteLine("RootComponent: " + builder.RootComponents[0].Selector);

  builder.Services.AddLocalization();

  // neu seit 6.0 Preview 7 für <HeadContent>:
  builder.RootComponents.Add<HeadOutlet>("head::after");

  #region Mehrsprachigkeit
  Console.WriteLine("Setting Culture...");

  var host = builder.Build();
  var jsInterop = host.Services.GetRequiredService<IJSRuntime>();
  var result = await jsInterop.InvokeAsync<string>("blazorCulture.get");
  if (result != null)
  {
   var culture = new CultureInfo(result);
   CultureInfo.DefaultThreadCurrentCulture = culture;
   CultureInfo.DefaultThreadCurrentUICulture = culture;
  }
  else
  {
   var culture = new CultureInfo("de-de");
   CultureInfo.DefaultThreadCurrentCulture = culture;
   CultureInfo.DefaultThreadCurrentUICulture = culture;
  }
  #endregion

  Console.WriteLine("Starting...");
  await host.RunAsync();
 }

 /// <summary>
 /// DI
 /// </summary>
 public static void AddServices(WebAssemblyHostBuilder builder)
 {
  IServiceCollection services = builder.Services;

  //Console.WriteLine("Liste der vorregistrierten Dienste in Blazor WebAssembly:");
  //foreach (var s in services)
  //{
  // Console.WriteLine(s.ServiceType.FullName + ": " + s.Lifetime);
  //}

  #region DI Konfiguration und Anwendungszustand
  services.AddSingleton<IAppState, AppState>();
  #endregion

  #region DI Authentifizierungsdienste
  services.AddOptions(); // notwendig für AuthenticationStateProvider
  services.AddScoped<IMLAuthenticationStateProvider, MLAuthenticationStateProvider3Tier>();
  services.AddScoped<AuthenticationStateProvider, MLAuthenticationStateProvider3Tier>();
  services.AddAuthorizationCore(); // sonst: System.InvalidOperationException: Cannot provide a value for property 'AuthorizationPolicyProvider' on type 'Microsoft.AspNetCore.Components.Authorization.AuthorizeView'. There is no registered service of type 'Microsoft.AspNetCore.Authorization.IAuthorizationPolicyProvider'.
  #endregion

  #region DI Serverkommunikation
  services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }); // aus der Projektvorlage von Microsoft!
  services.AddScoped<MiracleList.IMiracleListProxy, MiracleList.MiracleListProxy>();
  #endregion

  #region DI für Blazor-Zusatzkomponenten
  // Kontextmenü mit https://github.com/stavroskasidis/BlazorContextMenu
  services.AddBlazorContextMenu();

  // Für Toasts-Benachrichtigungen mit Blazored.Toast
  services.AddBlazoredToast();

  // NUGET: Blazored.LocalStorage;
  // GITHUB:  https://github.com/Blazored/LocalStorage --> 
  // Startup: using Blazored.LocalStorage / services.AddBlazoredLocalStorage();
  services.AddBlazoredLocalStorage();
  #endregion

  #region DI für Beispiele außerhalb der MiracleList
  // Für Session-State-Demo
  services.AddScoped<TypedSessionState>();
  services.AddScoped<GenericSessionState>();

  #endregion

  #region DI für sonstige Hilfsbibliotheken
  services.AddScoped<BlazorUtil>();
  // braucht BlazorUtil
  services.AddScoped<IHttpContextAccessor, HttpContextAccessorDummy>();
  #endregion

  #region Zusätzliche Komponenten, die MLBlazorRCL rendern soll
  // Datei-UploadChangeEventArgs bei TaskEdit.razor
  AdditionalComponents.TaskEditAdditionalComponent = typeof(Web.Components.FilesFromWebservice);
  #endregion
 }
}