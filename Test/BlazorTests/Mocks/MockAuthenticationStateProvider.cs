//using BD;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BL;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MiracleList;
using Web;

namespace BlazorTests.Mocks;


/// <summary>
/// Mock for AuthenticationStateProvider for MiracleList Blazor Server
/// does not use the MiracleList Business Logic for Authentication :-)
/// </summary>
public class MockAuthenticationStateProvider : Web.MLAuthenticationStateProvider2Tier, IMLAuthenticationStateProvider {
 // DI
 private BlazorUtil blazorUtil { get; set; }
 private Blazored.LocalStorage.ILocalStorageService localStorage { get; set; }

 private BO.User currentUser { get; set; }

 // Name of local Storage Key
 const string LocalStorageKey = "MLToken";

 public MockAuthenticationStateProvider(BlazorUtil blazorUtil, Blazored.LocalStorage.ILocalStorageService localStorage, AppState settings, NavigationManager navigationManager, IJSRuntime js) : base(blazorUtil, localStorage, settings, navigationManager, js) {
  // DI
  this.blazorUtil = blazorUtil;
  this.localStorage = localStorage;
 }

 /// <summary>
 /// Login to be called by Razor Component Login.razor in case of login!
 /// </summary> 
 public override async Task<LoginInfo> LogIn(string username, string password, string backend) {
  if (String.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) {
   this.currentUser = null;
  }
  else {
   this.currentUser = new BO.User();
   this.currentUser.UserName = username;
  }
  Notify();
  if (this.currentUser != null) {
   // Store user token in local Storage
   await localStorage.SetItemAsync(LocalStorageKey, currentUser.Token);
   return new LoginInfo() { Username = this.currentUser?.UserName };
  }
  return new LoginInfo() { Message = "not ok" };
 }

 /// <summary>
 /// Logout to be called by Razor Component Login.razor in case of logout!
 /// </summary>
 public override Task Logout() {
  if (currentUser == null) return Task.FromResult(0); ;
  blazorUtil.Log("Logout");
  this.currentUser = null;
  // Remove user token from local Storage
  localStorage.RemoveItemAsync(LocalStorageKey);
  Notify();
  return Task.FromResult(0);
 }

 /// <summary>
 /// Notify Blazor infrastructure about new Authentication State
 /// </summary>
 private void Notify() {
  var aus = CreateAuthenticationState(currentUser);
  var e = Task.FromResult(aus);
  this.NotifyAuthenticationStateChanged(e);
 }

 /// <summary>
 /// Convert LoginInfo to AuthenticationState
 /// </summary>
 private AuthenticationState CreateAuthenticationState(BO.User u) {
  // If you create your identity including authenticationType parameter the user is authenticated. If you create your identity without authenticationType parameter, the user is not authenticated.
  if (u == null) { // not authenticated :-(
   return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
  }

  // authenticated :-)
  var identity = new ClaimsIdentity(new[]
 {
   new Claim("Backend",  DA.Context.ConnectionString),
   new Claim("Server", new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(DA.Context.ConnectionString).DataSource),
   new Claim("LogInDateTime", DateTime.Now.ToString()),
   new Claim(ClaimTypes.Sid, u.UserID.ToString()),
   new Claim(ClaimTypes.Name, u.UserName),
 }, "MiracleList Authentication Type");

  var user = new ClaimsPrincipal(identity);
  return new AuthenticationState(user);
 }

 public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
  if (currentUser == null) {
   blazorUtil.Log(nameof(GetAuthenticationStateAsync) + ": User not logged in!");

   try {
    string token = await localStorage.GetItemAsync<string>(LocalStorageKey);
    if (token != null) currentUser = new UserManager(token).CurrentUser;
   }
   catch (Exception) {

    blazorUtil.Log(nameof(GetAuthenticationStateAsync) + ": cannot access local storage!");
    currentUser = null;
   }

   if (currentUser == null) {
    blazorUtil.Log(nameof(GetAuthenticationStateAsync) + ": User not found in local Storage!");
    return CreateAuthenticationState(null); // null = kein User!
   }
  }

  blazorUtil.Log(nameof(GetAuthenticationStateAsync) + ": User=" + currentUser?.UserName);

  return CreateAuthenticationState(currentUser);
 }
}