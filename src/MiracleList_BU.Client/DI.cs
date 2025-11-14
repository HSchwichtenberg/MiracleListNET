using Blazored.LocalStorage;
using Blazored.Toast;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components.Authorization;
using MiracleList;
using MLBlazorRCL.MainView;

namespace Web.Client;

public class DI
{
 /// <summary>
 /// Gemeinsame Dienste für Server und Client
 /// </summary>
 public static void AddServices(IServiceCollection services)
 {
  #region DI Authentifizierungsdienste
  services.AddOptions(); // notwendig für AuthenticationStateProvider
  services.AddCascadingAuthenticationState(); // neu seit Blazor 8.0
  services.AddScoped<IMLAuthenticationStateProvider, MLAuthenticationStateProvider3Tier>();
  services.AddScoped<AuthenticationStateProvider, MLAuthenticationStateProvider3Tier>();
  services.AddAuthorizationCore();
  #endregion

  #region DI für Serverkommunikation
  services.AddSingleton(new HttpClient());
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
  services.AddBlazorUtil();
  #endregion

  #region Zusätzliche Komponenten, die MLBlazorRCL rendern soll
  // Datei-UploadChangeEventArgs bei TaskEdit.razor
  AdditionalComponents.TaskEditAdditionalComponent = typeof(MLBlazorRCL.Files.FilesFromWebservice);
  #endregion
 }
}