using System;
using System.Collections.Generic;
using ITVisions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using MiracleList;

namespace Web;

/// <summary>
/// Eigene Datei als Wrapper für /wwwroot/appsettings.json
/// </summary>
public class AppState : IAppState
{
 public bool ShouldOfferReloadAfterLoginForTransitionToWebAssembly => true;
 public string? Token { get; set; }
 public string? Username { get; set; }
 public string? BackendURL { get; set; }

 public string BackendDisplayName => BackendURL ?? "";
 public string ClientID => this.configuration["Backend:ClientID"] ?? "";

 public string? SignalRHubURL { get; set; }
 public HubConnection? HubConnection { get; set; }

 public string CurrentUserDirectoryAbsolutePath { get; } = "";
 public string CurrentUserDirectoryRelativePath { get; } = "";

 private readonly IConfiguration configuration;
 private readonly IWebHostEnvironment host;

 // Diese Daten werden aus /wwwroot/appsettings.json gelesen
 // Bitte ändern Sie die Daten dort, siehe auch readme.md
 public string StagingURL = "";
 public string LiveURL = "";
 public string DebugURL = "";

 public AppState(IConfiguration configuration, IWebHostEnvironment host)
 {
  this.configuration = configuration;
  this.host = host;

  // Alle Server in Console ausgeben
  IConfigurationSection section = configuration.GetSection("Backend");
  foreach (var s in section.GetChildren())
  {
   Console.WriteLine(s.Key + "=" + s.Value);
  }

  // Direkter Zugriff auf diese drei Einträge
  this.StagingURL = this.configuration["Backend:StagingURL"].NotNull();
  this.LiveURL = this.configuration["Backend:LiveURL"].NotNull();
  this.DebugURL = this.configuration["Backend:DebugURL"].NotNull();

  Console.WriteLine($"AppState.ctor: {this.LiveURL}/{this.StagingURL}/{this.DebugURL} ");
 }

 public SortedDictionary<string, string> GetBackendSet(bool includeLocalhost = false)
 {
  var list = new SortedDictionary<string, string>() {
   { LiveURL, LiveURL },
   //{ "Staging", StagingURL },
   };

  if (includeLocalhost || host.IsDevelopment()) list.Add(DebugURL, DebugURL);
  if (host.IsEnvironment("Staging")) list.Add(StagingURL, StagingURL);
  return list;
 }
}