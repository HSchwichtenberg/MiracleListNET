using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using MiracleList;

namespace MLBlazorRCL;

public class LoginModel : ComponentBase
{
 [Inject] public BlazorUtil Util { get; set; }
 [Inject] public NavigationManager NavigationManager { get; set; }
 [Inject] AuthenticationStateProvider mLAuthenticationStateProvider { get; set; } = null;
 [Inject] protected ILocalStorageService localStorage { get; set; } = null;
 [Inject] public MiracleList.IAppState AppState { get; set; } = null;

 [CascadingParameter]
 public bool IsDarkTheme { get; set; }
 [CascadingParameter] public Task<AuthenticationState> authenticationStateTask { get; set; }

 // Erlaubt den Aufruf durch externe Anwendungen (z.B. Desktop-Apps unter Umgehung des Anmeldedialogs:
 // https://localhost:44340/?user=NameDesBentuzers&Pwd=geheim
 // SICHERHEITSHINWEIS für den Praxiseinsatz: Das Kennwort sollte verschlüsselt sein!
 [Parameter]
 [SupplyParameterFromQuery(Name = "user")]
 public string Q_User { get; set; }

 [Parameter]
 [SupplyParameterFromQuery(Name = "pwd")]
 public string Q_Password { get; set; }

 public bool ForceReload { get; set; } = false;

 [PersistentState] // Zwingend notwendig, da RenderMode.InteractiveAuto nur im Prerendering ermittelbar! 
 public bool IsAutoRenderMode { get; set; } = false;

 #region Properties zur Datenbindung
 public string Username { get; set; }
 public string Password { get; set; }
 public string Backend { get; set; }
 public string LoginStatus { get; set; }
 private bool shouldRender { get; set; }
 public SortedDictionary<string, string> BackendList { get; set; } = new();
 #endregion

 protected override async System.Threading.Tasks.Task OnInitializedAsync()
 {
  shouldRender = false;   // Rendern unterdrücken, falls wir die Anmeldeseite gar nicht darstellen müssen

  // InteractiveAuto liefert Blazor nur beim Prerendering!
  if (this.AssignedRenderMode == RenderMode.InteractiveAuto) IsAutoRenderMode = true;
  ForceReload = IsAutoRenderMode;

  Util.Log($"{nameof(LoginModel)}.{nameof(OnInitializedAsync)}: {this.NavigationManager.Uri} | RenderMode: {this.RendererInfo.Name} -> {this.AssignedRenderMode}");
  #region Umleitung als Reaktion auf die URL /logout
  if (this.NavigationManager.Uri.Contains("/logout", StringComparison.OrdinalIgnoreCase))
  {
   await ((IMLAuthenticationStateProvider)mLAuthenticationStateProvider).Logout();
   return;
  }
  #endregion

  #region Wenn wir noch authentifiziert sind (mit Daten im Local Storage), dann gehen wir direkt zu /main
  if (authenticationStateTask != null)
  {
   if ((await authenticationStateTask).User.Identity.IsAuthenticated)
   {
    NavigationManager.NavigateTo("/main");
   }
  }
  #endregion

  #region Vorgaben im Login-Formular
  bool includeLocalHost = this.NavigationManager.Uri.Contains("localhost", StringComparison.OrdinalIgnoreCase);
  BackendList = AppState.GetBackendSet(includeLocalHost);

  // Wenn es eine Backendliste gibt und noch kein Backend im Local Storage war oder das Backend aus dem Local Storage nicht existiert in der Liste: nimm das erste in der Liste
  if (BackendList.Count > 0 && (String.IsNullOrEmpty(Backend) || !BackendList.Keys.Any(x => x == Backend))) Backend = BackendList.Keys.First();
  shouldRender = true;

  if (System.Diagnostics.Debugger.IsAttached) // zum einfacheren Debugging
  {
   Username = IAppState.DebugUser;
   Password = IAppState.DebugPassword;
  }

  #endregion

  shouldRender = true;
 }

 protected override bool ShouldRender()
 {
  return shouldRender;
 }

 protected override async Task OnParametersSetAsync()
 {
  Console.WriteLine(nameof(LoginModel) + "." + (nameof(OnParametersSetAsync)));

  // gibt es Daten im QueryString? (Aufruf von einer anderen Anwendung)
  if (!String.IsNullOrEmpty(Q_User) && !(string.IsNullOrEmpty(Q_Password)))
  {
   // Set the Backend-URL for the Proxy
   var url = "https://miraclelistbackend.azurewebsites.net/";
   LoginInfo li = await ((IMLAuthenticationStateProvider)mLAuthenticationStateProvider).LogIn(Q_User, Q_Password, url);
   if (li != null && String.IsNullOrEmpty(li.Message)) { NavigationManager.NavigateTo("/app",ForceReload); return; }
  }
 }

 /// <summary>
 /// Reaktion auf Benutzeraktion
 /// </summary>
 protected async Task Login()
 {
  if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
  {
   LoginStatus = "Bitte E-Mail-Adresse und Kennwort eingeben.";
   return;
  }
  LoginStatus = "Anmeldung wird geprüft...";
  Util.Log($"{nameof(LoginModel)}.{nameof(Login)}: {Username}/{Password}/{Backend}");

  // Anmelden versuchen
  LoginInfo loginInfo = await ((IMLAuthenticationStateProvider)mLAuthenticationStateProvider).LogIn(Username, Password, Backend);

  if (loginInfo != null && String.IsNullOrEmpty(loginInfo.Message))
  {

   /// TODO: Das würde ich gerne machen, bringt aber bUnit zum Absturz ("System.InvalidOperationException : Queue empty.")
   //var u = (await authenticationStateTask).User;
   //if (u.Identity.IsAuthenticated)

   // daher:
   var username = loginInfo.Username;

   if (!String.IsNullOrEmpty(Username))
   {
    LoginStatus = "";
    // DEMO: 32. NavigationManager
    Util.Log("Login.DoLogin: Login OK!");
    NavigationManager.NavigateTo("/main", ForceReload); 
    return;
   }
   else
   {
    LoginStatus = "Anmeldefehler: unbekannter Fehler im Backend.";
    Util.Log("Login.DoLogin: Unknown Login Error!");
   }
  }
  else
  {
   LoginStatus = "Anmeldefehler: " + loginInfo?.Message;
   Util.Log("Login.DoLogin: Login Error!");
  }
 }
} // end class Login