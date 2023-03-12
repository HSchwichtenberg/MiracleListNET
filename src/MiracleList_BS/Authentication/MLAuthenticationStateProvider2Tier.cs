﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BL;
using DA;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Data.SqlClient;
using MiracleList;

namespace Web;

/// <summary>
/// AuthenticationStateProvider for MiracleList Blazor Server
/// uses the MiracleList Business Logic (UserManager) for Authentication
/// </summary>
public class MLAuthenticationStateProvider2Tier : AuthenticationStateProvider, IMLAuthenticationStateProvider
{
 // DI
 private BlazorUtil blazorUtil { get; set; }
 private Blazored.LocalStorage.ILocalStorageService localStorage { get; set; }
 private IAppState AppState { get; set; } = null; // App State
 private NavigationManager NavigationManager { get; set; } = null; // App State
 private BO.User currentUser { get; set; }

 // Name of local Storage Key
 const string LocalStorageKey = "MLToken";
 const string BackendStorageKey = "Backend";

 public MLAuthenticationStateProvider2Tier(BlazorUtil blazorUtil, Blazored.LocalStorage.ILocalStorageService localStorage, IAppState appState, NavigationManager navigationManager)
 {
  // DI
  this.blazorUtil = blazorUtil;
  this.localStorage = localStorage;
  this.AppState = appState;
  this.NavigationManager = navigationManager;
 }

 public async Task SetCurrentBackend(string backendkey)
 {
  var backend = AppState.GetBackendByKey(backendkey);
  if (backend == null) { return; };
  Context.ConnectionString = backend;
  AppState.BackendURL = backend;

  // wichtig: Im Local Storage nur den Display Name ablegen!!!
  await localStorage.SetItemAsync("Backend", backendkey);
 }

 private void SetCurrentUser(BO.User current)
 {
  this.currentUser = current;
  AppState.Token = current != null ? current.UserID.ToString() : "";
  AppState.Username = current != null ? current.UserName : "";
 }

 /// <summary>
 /// Login to be called by Razor Component Login.razor in case of login!
 /// </summary> 
 public virtual async Task<LoginInfo> LogIn(string username, string password, string backendkey)
 {
  try
  {
   await SetCurrentBackend(backendkey);
   UserManager um = new UserManager();
   um = new BL.UserManager(username, password);
   SetCurrentUser(um.CurrentUser);
   Notify();
   if (this.currentUser != null)
   {
    um.InitDefaultTasks();
    // Store user token in local Storage
    await localStorage.SetItemAsync(LocalStorageKey, currentUser.Token);
    return new LoginInfo() { Username = this.currentUser.UserName };
   }
   return new LoginInfo() { Message = "not ok!" };
  }
  catch (Exception ex)
  {
   return new LoginInfo() { Message = ex.Message };
  }
 }

 /// <summary>
 /// Logout to be called by Razor Component Login.razor in case of logout!
 /// </summary>
 public virtual Task Logout()
 {
  if (currentUser == null) return Task.FromResult(0);
  blazorUtil.Log("Logout");
  UserManager.Logout(currentUser);
  SetCurrentUser(null);
  // Remove user token from local Storage
  localStorage.RemoveItemAsync(LocalStorageKey);
  Notify();
  return Task.FromResult(0);
 }

 /// <summary>
 /// Notify Blazor infrastructure about new Authentication State
 /// </summary>
 private void Notify()
 {
  var aus = CreateAuthenticationState(currentUser);
  var e = Task.FromResult(aus);
  this.NotifyAuthenticationStateChanged(e);
 }

 /// <summary>
 /// Convert LoginInfo to AuthenticationState
 /// </summary>
 private AuthenticationState CreateAuthenticationState(BO.User u)
 {
  // If you create your identity including authenticationType parameter the user is authenticated. If you create your identity without authenticationType parameter, the user is not authenticated.
  if (u == null)
  { // not authenticated :-(
   return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
  }

  // authenticated :-)
  var identity = new ClaimsIdentity(new[]
 {
   new Claim("Backend",  DA.Context.ConnectionString),
   new Claim("Server", new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(DA.Context.ConnectionString).DataSource),
   new Claim("LogInDateTime", DateTime.Now.ToString()),
   new Claim(ClaimTypes.Sid, u.UserID.ToString()),
   new Claim("Token", u.Token),
   new Claim(ClaimTypes.Name, u.UserName),
 }, "MiracleList Authentication Type");

  var user = new ClaimsPrincipal(identity);
  return new AuthenticationState(user);
 }

 public override async Task<AuthenticationState> GetAuthenticationStateAsync()
 {
  if (currentUser == null)
  {
   blazorUtil.Log(nameof(GetAuthenticationStateAsync) + ": User not logged in!");
   await CheckLocalToken();
  }
  return CreateAuthenticationState(currentUser);
 }

 public async Task<bool> CheckLocalToken()
 {
  blazorUtil.Log(nameof(CheckLocalToken));
  try
  {
   // no token in RAM! Is there a token in local Storage?
   blazorUtil.Log("Reading local storage..");
   await SetCurrentBackend(await localStorage.GetItemAsync<string>(BackendStorageKey));
   string token = await localStorage.GetItemAsync<string>(LocalStorageKey);
   if (!String.IsNullOrEmpty(token)) { SetCurrentUser(new UserManager(token).CurrentUser); Notify(); return true; }
   else { await localStorage.RemoveItemAsync(LocalStorageKey); } // Token löschen, wenn ungültig!
  }
  catch (Exception)
  {
   blazorUtil.Log(nameof(GetAuthenticationStateAsync) + ": cannot access local storage!");
   return false;
  }
  if (currentUser == null)
  {
   blazorUtil.Log(nameof(GetAuthenticationStateAsync) + ": User not found in local Storage!");
  }
  return false;
 }

 public async Task<BackendState> CheckBackend(string address)
 {

  BackendState result = new BackendState();

  try
  {
   var connection = new SqlConnection(address);
   connection.Open();
   result.State = BackendStateStatus.OK;
   result.StateDetails = "SQL Server-Version: " + connection.ServerVersion;
   connection.Close();
  }
  catch (Exception ex)
  {
   result.State = BackendStateStatus.Error;
   result.StateDetails = ex.Message;
  }
  return result;
 }
}