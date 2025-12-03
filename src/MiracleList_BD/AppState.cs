using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MiracleList;

namespace Web;

public class AppState : IAppState
{
 public bool ShouldOfferReloadAfterLoginForTransitionToWebAssembly => false;
 public string Token { get; set; }
 public string Username { get; set; }
 public string BackendURL { get; set; }
 private string signalRHubURL = "https://miraclelistbackend.azurewebsites.net/MLHUB";
 public string SignalRHubURL { get => signalRHubURL; set => signalRHubURL = value; }
 public HubConnection HubConnection { get; set; }
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

 public string CurrentUserDirectoryAbsolutePath
 {
  get
  {
   return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), CurrentUserDirectoryRelativePath);
  }
 }

 public string CurrentUserDirectoryRelativePath
 {
  get
  {
   return Path.Combine("Files", new BL.UserManager(Int32.Parse(Token)).CurrentUser.UserGUID.ToString());
  }
 }
 public string ClientID => throw new NotImplementedException();

 private readonly IConfiguration configuration;

 public SortedDictionary<string, string> ConnectionStrings = new();

 public AppState(IConfiguration configuration)
 {
  this.configuration = configuration;

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

 public SortedDictionary<string, string> GetBackendSet(bool includeLocalhost = false)
 {
  return this.ConnectionStrings;
 }

}