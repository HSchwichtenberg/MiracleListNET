using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using MiracleList;

namespace Web;


/// <summary>
/// Eigene Datei als Wrapper für /wwwroot/appsettings.json
/// </summary>
public class AppState : IAppState
{
 public string BackendURL { get; set; }
 private string signalRHubURL = "https://miraclelistbackend.azurewebsites.net/MLHUBV2";
 public string SignalRHubURL { get => signalRHubURL; set => signalRHubURL = value; }
 public HubConnection HubConnection { get; set; }
 public string BackendDisplayName => BackendURL ?? "";
 public string ClientID => this.configuration["Backend:ClientID"];
 public string Token { get; set; }
 public string Username { get; set; }

 private readonly IConfiguration configuration;

 // Diese Daten werden aus /wwwroot/appsettings.json gelesen
 // Bitte ändern Sie die Daten dort, siehe auch readme.md
 public string StagingURL;
 public string LiveURL;
 public string DebugURL;

 public AppState(IConfiguration configuration)
 {

  this.configuration = configuration;

  // Alle Server in Console ausgeben
  IConfigurationSection section = configuration.GetSection("Backend");
  foreach (var s in section.GetChildren())
  {
   Console.WriteLine(s.Key + "=" + s.Value);
  }

  // Direkter Zugriff auf diese drei Einträge
  this.StagingURL = this.configuration["Backend:StagingURL"];
  this.LiveURL = this.configuration["Backend:LiveURL"];
  this.DebugURL = this.configuration["Backend:DebugURL"];

  Console.WriteLine($"AppState.ctor: {this.LiveURL}/{this.StagingURL}/{this.DebugURL} ");

 }

 public SortedDictionary<string, string> GetBackendSet(bool includeLocalhost = false)
 {
  var list = new SortedDictionary<string, string>() {
   { LiveURL, LiveURL }, 
   //{ "Staging", StagingURL }, 
   { DebugURL, DebugURL } };
  //if (!includeLocalhost) list = list.Values.Where(x => !x.ToLower().Contains("localhost") && !x.ToLower().Contains("staging"));
  return list;
 }
}