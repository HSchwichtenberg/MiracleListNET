#pragma warning disable 1998
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using MiracleList;

namespace Web;

// DEMO: 41. MLAuthenticationStateProvider
public class MLAuthenticationStateProvider3Tier : AuthenticationStateProvider, IMLAuthenticationStateProvider
{
 #region DI / [Inject] not possible here
 private IMiracleListProxy proxy { get; set; } = null; // Backend Proxy
 private IAppState AppState { get; set; } = null; // App State
 public IServiceProvider ServiceProvider { get; }
 private BlazorUtil blazorUtil { get; set; } = null; // Utility for Browser Console
 private ILocalStorageService localStorage { get; set; } // Local Storage in Browser
 #endregion

 // Data from Login() operation
 public LoginInfo CurrentLoginInfo { get; set; } = null;

 // Name of local Storage Keys
 const string LoginInfoStorageKey = "LoginInfo";
 const string BackendStorageKey = "Backend";

 public MLAuthenticationStateProvider3Tier(IMiracleListProxy proxy, BlazorUtil blazorUtil, ILocalStorageService localStorage, IAppState appState, IServiceProvider ServiceProvider)
 {
  // DI
  this.blazorUtil = blazorUtil;
  this.proxy = proxy;
  this.localStorage = localStorage;
  this.AppState = appState;
  this.ServiceProvider = ServiceProvider;
 }

 public async Task SetCurrentBackend(string backendKey)
 {

  var backend = AppState.GetBackendByKey(backendKey);
  if (backend == null) { return; };

  proxy.BaseUrl = backend;
  AppState.BackendURL = backend;
  AppState.SignalRHubURL = new Uri(new Uri(backend), "MLHubV2").ToString();
  await localStorage.SetItemAsync("Backend", AppState.BackendDisplayName);
 }

 private void SetCurrentUser(LoginInfo current)
 {
  this.CurrentLoginInfo = current;
  AppState.Token = current != null ? current.Token : "";
  AppState.Username = current != null ? current.Username : "";
 }

 /// <summary>
 /// Login to be called by Razor Component Login.razor
 /// </summary>
 public async Task<LoginInfo> LogIn(string username, string password, string backendKey)
 {

  var backend = AppState.GetBackendByKey(backendKey);

  var l = new LoginInfo() { Username = username, Password = password, ClientID = AppState.ClientID };
  await SetCurrentBackend(backend);

  blazorUtil.Log($"{nameof(LogIn)}: Login at {backend} for {l.Username} ...");
  try
  {
   // Aufruf von /Login via generiertem Proxy
   SetCurrentUser(await proxy.LoginAsync(l));

   if (String.IsNullOrEmpty(CurrentLoginInfo.Token)) // Kein Token --> Fehler
   {
    blazorUtil.Log($"{nameof(LogIn)}: Login Error: {CurrentLoginInfo.Message}!");
   }
   else
   {
    blazorUtil.Log($"{nameof(LogIn)}: Login OK :-) Token={CurrentLoginInfo.Token}!");
   }
  }
  catch (Exception ex)
  {

   blazorUtil.Log($"{nameof(LogIn)}: Login Error: {ex}!");
   SetCurrentUser(new LoginInfo
   {
    Message = ex.ToString()
   });
  }

  // Notify new state!
  Notify();
  // Store user token and backend URL in local Storage
  blazorUtil.Log("Write to Local storage", proxy.BaseUrl + "/" + CurrentLoginInfo.Username);
  await localStorage.SetItemAsync(LoginInfoStorageKey, CurrentLoginInfo);
  await localStorage.SetItemAsync(BackendStorageKey, backend);
  return CurrentLoginInfo;
 }

 /// <summary>
 /// Logout to be called by Razor Component Login.razor
 /// </summary>
 public async Task Logout()
 {
  blazorUtil.Log("Logout", this.CurrentLoginInfo);
  if (this.CurrentLoginInfo == null) return;
  var e = await proxy.LogoffAsync(this.CurrentLoginInfo.Token);
  blazorUtil.Log("Logout-Result", e);
  if (e)
  {
   // Store the LoginInfo without token to present username again in Login.razor
   CurrentLoginInfo.Token = null;
   await localStorage.SetItemAsync(LoginInfoStorageKey, CurrentLoginInfo);
   // Remove LoginInfo in RAM for clearing authenticaton state
   SetCurrentUser(null);
   Notify();
  }
 }

 /// <summary>
 /// Called by Blazor infrastructure if AuthenticationState is unknown
 /// </summary>
 /// <returns>AuthenticationState</returns>
 public override async Task<AuthenticationState> GetAuthenticationStateAsync()
 {
  if (CurrentLoginInfo == null)
  {
   try
   {
    // no token in RAM! Is there a token in local Storage?
    blazorUtil.Log("Reading local storage..");
    await SetCurrentBackend(await localStorage.GetItemAsync<string>(BackendStorageKey));
    SetCurrentUser(await localStorage.GetItemAsync<LoginInfo>(LoginInfoStorageKey));
    blazorUtil.Log("Data from Local storage", "URL=" + proxy.BaseUrl + " USER=" + CurrentLoginInfo?.Username);
   }
   catch (Exception ex)
   {
    blazorUtil.Log(nameof(GetAuthenticationStateAsync) + ": cannot read data from local storage: " + ex.ToString());
    SetCurrentUser(null);
   }

   if (this.CurrentLoginInfo != null && !String.IsNullOrEmpty(this.CurrentLoginInfo.Token) && !String.IsNullOrEmpty(proxy.BaseUrl))
   {
    // If we have a token: re-validate token in backend (is the token still valid?)
    blazorUtil.Log($"Re-Validating token {this.CurrentLoginInfo.Token}...");
    try
    {
     // login using the token
     SetCurrentUser((await proxy.LoginAsync(CurrentLoginInfo)));
     blazorUtil.Log("Data from Re-Validation", this.CurrentLoginInfo);
     if (this.CurrentLoginInfo == null || String.IsNullOrEmpty(CurrentLoginInfo.Token))
     {
      blazorUtil.Log($"{nameof(LogIn)}: Token not valid: {CurrentLoginInfo.Message}!");
      await localStorage.RemoveItemAsync(LoginInfoStorageKey);  // Token löschen, wenn ungültig!
      CurrentLoginInfo = null;
     }
    }
    catch (Exception ex)
    {
     blazorUtil.Log($"{nameof(GetAuthenticationStateAsync)}: cannot re-validate token {this.CurrentLoginInfo.Token}: {ex}");
     await localStorage.RemoveItemAsync(LoginInfoStorageKey);  // Token löschen, wenn ungültig!
     SetCurrentUser(null);
    }
   }

   if (CurrentLoginInfo == null)
   {
    blazorUtil.Log(nameof(GetAuthenticationStateAsync) + ": not authenticated!");
    return CreateAuthenticationState(null); // null == no user!
   }
  }
  else
  {
   blazorUtil.Log(nameof(GetAuthenticationStateAsync) + ":" + CurrentLoginInfo.Username);
  }

  return CreateAuthenticationState(CurrentLoginInfo);
 }

 /// <summary>
 /// Notify Blazor infrastructure about new Authentication State
 /// </summary>
 private void Notify()
 {
  var aus = CreateAuthenticationState(CurrentLoginInfo);
  var e = Task.FromResult(aus);
  this.NotifyAuthenticationStateChanged(e);
 }

 /// <summary>
 /// Convert LoginInfo to AuthenticationState
 /// </summary>
 private AuthenticationState CreateAuthenticationState(LoginInfo l)
 {
  // If you create your identity including authenticationType parameter the user is authenticated. If you create your identity without authenticationType parameter, the user is not authenticated.
  if (l == null || String.IsNullOrEmpty(l.Token))
  { // not authenticated :-( --> returning null is not valid!!!
   return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
  }
  else   // authenticated :-)
  {
   const string authType = "MiracleList WebAPI Authentication";
   var identity = new ClaimsIdentity(new[]
   {
   new Claim("Backend", proxy.BaseUrl),
   new Claim(ClaimTypes.Sid, l.Token), // use SID claim for token
   new Claim(ClaimTypes.Name, l.Username), // user name as text
   new Claim("LogInDateTime", DateTime.Now.ToString())
   }, authType);

   var user = new ClaimsPrincipal(identity);
   return new AuthenticationState(user);
  }
 }

 public Task<bool> CheckLocalToken()
 {
  throw new NotImplementedException();
 }

 public async Task<BackendState> CheckBackend(string address)
 {

  BackendState result = new BackendState();
  // 3-Tier -> Prüfung WebAPI-Server durch Aufruf einer Operation
  // Wir brauchen hier einen einene Scope, weil die Instanzen von IMiracleListProxy scoped im DI sind
  // und wir nicht mit einer Instanz verschiedene Server abfragen können!
  using (var scope = ServiceProvider.CreateScope())
  {
   IMiracleListProxy proxy = scope.ServiceProvider.GetService(typeof(IMiracleListProxy)) as IMiracleListProxy;
   proxy.BaseUrl = address;
   try
   {

    var serverData = await proxy.AboutAsync();
    var ServerAppVersion = serverData[6].Replace("Application Version: ", "");
    var ServerFramework = serverData[8];
    result.State = BackendStateStatus.OK;
    result.StateDetails = "WebAPI-Backend-Version: " + ServerAppVersion;
   }
   catch (Exception ex)
   {
    result.State = BackendStateStatus.Error;
    result.StateDetails = ex.Message;
   }
  }

  return result;
 }
}