using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MiracleList;

namespace MLBlazorRCL;

public class LoginModel : ComponentBase {
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

 #region Properties zur Datenbindung
 public string Username { get; set; }
 public string Password { get; set; }
 public string Backend { get; set; }
 public string ErrorMsg { get; set; }
 private bool shouldRender { get; set; }
 public SortedDictionary<string, string> ServerList { get; set; } = new();
 #endregion

 protected override async System.Threading.Tasks.Task OnInitializedAsync() {
  shouldRender = false;
  Console.WriteLine(nameof(LoginModel) + "." + (nameof(OnInitializedAsync)));
  Console.WriteLine("URI: " + this.NavigationManager.Uri);
  #region Umleitung als Reaktion auf die URL /logout
  if (this.NavigationManager.Uri.ToLower().Contains("/logout")) {
   await ((IMLAuthenticationStateProvider)mLAuthenticationStateProvider).Logout();
  }
  #endregion

  // letztes verwendetes Backend aus Local Storage
  Backend = await localStorage.GetItemAsync<string>("Backend");
  if (!String.IsNullOrEmpty(Backend)) {
   await ((IMLAuthenticationStateProvider)mLAuthenticationStateProvider).SetCurrentBackend(Backend);
  }

  #region Wenn wir noch authentifiziert sind (mit Daten im Local Storage), dann gehen wir direkt zu /main
  if (authenticationStateTask != null) {
   if ((await authenticationStateTask).User.Identity.IsAuthenticated) {
    NavigationManager.NavigateTo("/main");
   }
  }
  #endregion

  #region Vorgaben im Login-Formular
  bool includeLocalHost = this.NavigationManager.Uri.ToLower().Contains("localhost");
  ServerList = AppState.GetBackendSet(includeLocalHost);

  // sonst erstes Backend
  if (ServerList.Count > 0 && String.IsNullOrEmpty(Backend)) Backend = ServerList.Values.First();
  shouldRender = true;

  if (System.Diagnostics.Debugger.IsAttached) // zum einfacheren Debugging
  {
   Username = IAppState.DebugUser;
   Password = IAppState.DebugPassword;
  }
  shouldRender = true;
  #endregion
 }

 protected override bool ShouldRender() {
  return shouldRender;
 }

 protected override async Task OnParametersSetAsync() {
  Console.WriteLine(nameof(LoginModel) + "." + (nameof(OnParametersSetAsync)));

  // gibt es Daten im QueryString? (Aufruf von einer anderen Anwendung)
  if (!String.IsNullOrEmpty(Q_User) && !(string.IsNullOrEmpty(Q_Password))) {
   // Set the Backend-URL for the Proxy
   var url = "https://miraclelistbackend.azurewebsites.net/";
   LoginInfo li = await ((IMLAuthenticationStateProvider)mLAuthenticationStateProvider).LogIn(Q_User, Q_Password, url);
   if (li != null && String.IsNullOrEmpty(li.Message)) { NavigationManager.NavigateTo("/app"); return; }
  }
 }

 /// <summary>
 /// Reaktion auf Benutzeraktion
 /// </summary>
 protected async Task Login() {
  ErrorMsg = "Logging in...";
  Util.Log($"{nameof(LoginModel)}.{nameof(Login)}: {Username}/{Password}/{Backend}");

  // Anmelden versuchen
  LoginInfo loginInfo = await ((IMLAuthenticationStateProvider)mLAuthenticationStateProvider).LogIn(Username, Password, Backend);

  if (loginInfo != null && String.IsNullOrEmpty(loginInfo.Message)) {

   /// TODO: Das würde ich gerne machen, bringt aber bUnit zum Absturz ("System.InvalidOperationException : Queue empty.")
   //var u = (await authenticationStateTask).User;
   //if (u.Identity.IsAuthenticated)

   // daher:
   var username = loginInfo.Username;

   if (!String.IsNullOrEmpty(Username)) {
    ErrorMsg = "";
    // DEMO: 32. NavigationManager
    Util.Log("Login.DoLogin: Login OK!");
    this.NavigationManager.NavigateTo("/main");
    return;
   }
   else {
    ErrorMsg = "Unknown Login Error!";
    Util.Log("Login.DoLogin: Unknown Login Error!");
   }
  }
  else {
   ErrorMsg = "Login Error: " + loginInfo?.Message;
   Util.Log("Login.DoLogin: Login Error!");
  }
 }
} // end class Login