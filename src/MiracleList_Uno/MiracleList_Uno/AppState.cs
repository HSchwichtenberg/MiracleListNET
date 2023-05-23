using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using MiracleList;

namespace MiracleList_WinUI;

/// <summary>
/// Eigene Datei als Wrapper für /wwwroot/appsettings.json
/// </summary>
public class AppState : IAppState
{
    public string Token { get; set; }
    public string Username { get; set; }
    public string BackendURL { get; set; }
    private string signalRHubURL = "https://miraclelistbackend.azurewebsites.net/MLHUBV2";
    public string SignalRHubURL { get => signalRHubURL; set => signalRHubURL = value; }
    public string BackendDisplayName => BackendURL ?? "";
    public string ClientID => "11111111-1111-1111-1111-111111111120";
    public HubConnection HubConnection { get; set; }
    private readonly IConfiguration configuration;

    public string CurrentUserDirectoryAbsolutePath { get; }
    public string CurrentUserDirectoryRelativePath { get; }

    // Diese Daten werden aus /wwwroot/appsettings.json gelesen
    // Bitte ändern Sie die Daten dort, siehe auch readme.md
    public string StagingURL;
    public string LiveURL;
    public string DebugURL;

    public AppState(IConfiguration configuration)

    {
        this.configuration = configuration;

        //this.StagingURL = this.configuration["Backend:StagingURL"];
        this.LiveURL = "https://miraclelistbackend.azurewebsites.net/";
        this.DebugURL = "http://localhost:8889/";

    }

    public SortedDictionary<string, string> GetBackendSet(bool includeLocalhost = false)
    {
        var list = new SortedDictionary<string, string>() {
   { LiveURL, LiveURL },
   { DebugURL, DebugURL } };
        return list;
    }
}