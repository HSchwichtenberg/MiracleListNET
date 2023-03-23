using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MiracleList;

namespace Web;

public class AppState : IAppState {

 /// <summary>
 /// In ML_BS ist das Token die UserID = Zahl
 /// </summary>
 public string Token { get; set; }
 public string Username { get; set; }
 public string BackendURL { get; set; }
 public HubConnection HubConnection { get; set; }
 private string signalRHubURL;
 public string SignalRHubURL { get => signalRHubURL; set => signalRHubURL = value; }

 public string BackendDisplayName {
  get {
   if (String.IsNullOrEmpty(BackendURL)) return "";
   var csb = new SqlConnectionStringBuilder(BackendURL);
   var server = csb.DataSource;
   if (server.ToLower().Contains("windows.net")) server = "AZURE SQL DB";
   return server;
  }
 }

 public string ClientID => throw new NotImplementedException();

 public NavigationManager NavigationManager { get; }
 public IHostEnvironment Host { get; }

 private readonly IConfiguration configuration;
 public SortedDictionary<string, string> ConnectionStrings = new();

 public AppState(IConfiguration configuration, NavigationManager NavigationManager, IHostEnvironment host) {
  this.NavigationManager = NavigationManager;
  Host = host;
  this.configuration = configuration;

  signalRHubURL = this.NavigationManager.ToAbsoluteUri("/MLHub").ToString();

  try {
   Console.WriteLine($"AppSettings.ctor");
   IConfigurationSection section = configuration.GetSection("ConnectionStrings");
   foreach (var s in section.GetChildren()) {
    Console.WriteLine("Lade connectionString: " + s.Key);
    var server = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(s.Value).DataSource;
    if (server.ToLower().Contains("windows.net")) server = "AZURE SQL DB";
    ConnectionStrings.Add(server, s.Value);
   }
  }
  catch (Exception) {

  }
 }

 public SortedDictionary<string, string> GetBackendSet(bool includeLocalhost = false) {
  return this.ConnectionStrings;
 }
}