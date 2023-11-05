
using Blazored.LocalStorage;
using Blazored.Toast;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MiracleList;
using MLBlazorRCL.MainView;
using Web.Components;

namespace Web;

public class Program
{
 public static void Main(string[] args)
 {
  var builder = WebApplication.CreateBuilder(args);

  // Add services to the container.
  builder.Services.AddRazorComponents()
      .AddInteractiveServerComponents()
      .AddInteractiveWebAssemblyComponents();

  AddServices(builder);

  var app = builder.Build();

  // Configure the HTTP request pipeline.
  if (app.Environment.IsDevelopment())
  {
   app.UseWebAssemblyDebugging();
  }
  else
  {
   app.UseExceptionHandler("/Error");
   // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
   app.UseHsts();
  }

  app.UseHttpsRedirection();
  app.UseAntiforgery();
  app.UseStaticFiles();

  app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
     typeof(Web.Client.Routes).Assembly,
     typeof(MLBlazorRCL.Login.Login).Assembly,
     typeof(Samples.SamplesList).Assembly

     );

  app.Run();
 }

 /// <summary>
 /// DI
 /// </summary>
 public static void AddServices(WebApplicationBuilder builder)
 {
  IServiceCollection services = builder.Services;

  //Console.WriteLine("Liste der vorregistrierten Dienste in Blazor United:");
  //foreach (var s in services)
  //{
  // Console.WriteLine(s.ServiceType.FullName + ": " + s.Lifetime);
  //}

  #region DI Konfiguration und Anwendungszustand
  services.AddSingleton<IAppState, AppState>();
  #endregion

  #region DI Authentifizierungsdienste
  services.AddOptions(); // notwendig für AuthenticationStateProvider
  services.AddCascadingAuthenticationState(); // neu seit Blazor 8.0
  services.AddScoped<IMLAuthenticationStateProvider, MLAuthenticationStateProvider3Tier>();
  services.AddScoped<AuthenticationStateProvider, MLAuthenticationStateProvider3Tier>();
  services.AddAuthentication("ML");
  services.AddAuthorizationCore(); // sonst: System.InvalidOperationException: Cannot provide a value for property 'AuthorizationPolicyProvider' on type 'Microsoft.AspNetCore.Components.Authorization.AuthorizeView'. There is no registered service of type 'Microsoft.AspNetCore.Authorization.IAuthorizationPolicyProvider'
  #endregion

  #region DI für Serverkommunikation
  services.AddSingleton(new HttpClient() { });
  services.AddScoped<MiracleList.IMiracleListProxy, MiracleList.MiracleListProxy>();
  #endregion

  #region DI für Blazor-Zusatzkomponenten
  // Für Kontextmenü mit https://github.com/stavroskasidis/BlazorContextMenu
  services.AddBlazorContextMenu();

  // Für Toasts-Benachrichtigungen mit Blazored.Toast https://github.com/Blazored/Toast 
  services.AddBlazoredToast();

  // Für Blazored.LocalStorage https://github.com/Blazored/LocalStorage 
  services.AddBlazoredLocalStorage();
  #endregion

  #region DI für Beispiele außerhalb der MiracleList
  // Für Session-State-Demo
  services.AddScoped<TypedSessionState>();
  services.AddScoped<GenericSessionState>();
  #endregion

  #region DI für sonstige Hilfsbibliotheken
  services.AddBlazorUtilForBlazorServer();
  #endregion

  #region Zusätzliche Komponenten, die MLBlazorRCL rendern soll
  // Datei-UploadChangeEventArgs bei TaskEdit.razor
  AdditionalComponents.TaskEditAdditionalComponent = typeof(MLBlazorRCL.Files.FilesFromWebservice);
  #endregion
 }
}