using System;
using System.Collections.Generic;
using System.IO;
using ITVisions;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MiracleList;

namespace Web;

public class AppState : IAppState
{

 /// <summary>
 /// In ML_BS ist das Token die UserID = Zahl
 /// </summary>
 public string Token { get; set; }
 public string Username { get; set; }
 public string BackendURL { get; set; }
 public HubConnection HubConnection { get; set; }
 private string signalRHubURL;
 public string SignalRHubURL { get => signalRHubURL; set => signalRHubURL = value; }

 public string BackendDisplayName
 {
  get
  {
   if (String.IsNullOrEmpty(BackendURL)) return "";
   var csb = new SqlConnectionStringBuilder(BackendURL);
   var server = csb.DataSource;
   if (server.Contains("windows.net", StringComparison.OrdinalIgnoreCase)) server = "AZURE SQL DB";
   return server;
  }
 }

 public string ClientID => throw new NotImplementedException();

 public NavigationManager NavigationManager { get; }

 IWebHostEnvironment host;
 private readonly BlazorUtil util;

 public string CurrentUserDirectoryAbsolutePath
 {
  get
  {
   return Path.Combine(host.WebRootPath, CurrentUserDirectoryRelativePath);
  }
 }

 public string CurrentUserDirectoryRelativePath
 {
  get
  {
   return Path.Combine("Files", new BL.UserManager(Int32.Parse(Token)).CurrentUser.UserGUID.ToString());
  }
 }

 private readonly IConfiguration configuration;
 public SortedDictionary<string, string> ConnectionStrings = new();

 public AppState(IConfiguration configuration, NavigationManager NavigationManager, IWebHostEnvironment host, BlazorUtil util)
 {
  this.NavigationManager = NavigationManager;
  this.host = host;
  this.util = util;
  this.configuration = configuration;

  try
  {
   signalRHubURL = this.NavigationManager?.ToAbsoluteUri("/MLHub")?.ToString();

  }
  catch (Exception)
  {

  }

  try
  {
   Console.WriteLine($"AppSettings.ctor");
   IConfigurationSection section = configuration.GetSection("ConnectionStrings");
   foreach (var s in section.GetChildren())
   {
    Console.WriteLine("Lade connectionString: " + s.Key);
    var server = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(s.Value).DataSource;
    if (server.Contains("windows.net", StringComparison.OrdinalIgnoreCase)) server = "AZURE SQL DB";
    ConnectionStrings.Add(server, s.Value);
   }
  }
  catch (Exception)
  {
  }

  var filesPath = Path.Combine(host.WebRootPath, "Files");
  try
  {
   var d = new DirectoryInfo(filesPath).GetOrCreateDir();

   // Cleanup Dateien älter als 10 Tage, damit der DEMO-Server nicht zugemüllt wird -> Dies ggf. ändern für eigene Zwecke!
   var removecount = d.DeleteOldFiles(10);

   util.Log("File-Cleanup-OK: " + removecount + " Dateien älter als 10 Tage entfernt aus " + filesPath);
  }
  catch (Exception ex)
  {
   util.Log("File-Cleanup-Error in " + filesPath + ": " + ex.ToString());
  }

 }

 public SortedDictionary<string, string> GetBackendSet(bool includeLocalhost = false)
 {
  return this.ConnectionStrings;
 }
}