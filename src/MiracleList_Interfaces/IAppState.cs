using System.Net.NetworkInformation;
using Microsoft.AspNetCore.SignalR.Client;

namespace MiracleList;

/// <summary>
/// Die Schnittstelle IAppState aus dem Projekt MiracleList_Interfaces.csproj legt die Struktur für die Klasse AppState fest, die in jedem Kopfprojekt für den komponentenübergreifenden (globalen) Anwendungszustand (einschließlich Konfigurationsdaten) realisiert wird. In der Startdatei jedes Kopfprojekts erfolgt dann die Dependency Injection der entsprechenden Implementierung dieser Schnittstelle.
/// </summary>
public interface IAppState
{
 string BackendURL { get; set; }
 string BackendDisplayName { get; }
 string SignalRHubURL { get; set; }

 string ClientID { get; }
 string Username { get; set; }
 string Token { get; set; }

 bool IsLoggedIn { get => !String.IsNullOrEmpty(Username); }

 const string DebugUser = "Max Mustermann";
 const string DebugPassword = "Sehr+Geheim"; // :-)
 const string DebugToken = "";

 SortedDictionary<string, string> GetBackendSet(bool includeLocalhost = false);

 string GetBackendByKey(string key)
 {
  var backend = GetBackendSet().Where(x => x.Key == key);
  if (backend == null || backend.Count() == 0) { return null; };
  return backend.ElementAt(0).Value;
 }

 public HubConnection? HubConnection { get; set; }


}