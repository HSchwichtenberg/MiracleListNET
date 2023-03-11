using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using MiracleList;

namespace MiracleList;

public enum BackendStateStatus {
 Checking, OK, Error
}

public class BackendState {
 public string Display { get; set; } = "";
 public string Address { get; set; } = "";
 public BackendStateStatus State { get; set; }
 public string StateDetails { get; set; } = "";
}

/// <summary>
/// 2-Tier/3-Tier-Abstraktion für einer erweitertet AuthenticationStateProvider für Blazor
/// </summary>
public interface IMLAuthenticationStateProvider
{
 /// <summary>
 /// Ermittelt den aktuellen Anmeldezustand (Vorgabe aus AuthenticationStateProvider von Blazor)
 /// </summary>
 Task<AuthenticationState> GetAuthenticationStateAsync();

 /// <summary>
 /// Legt das aktuelle Backend fest
 /// </summary>
 /// <param name="backend">URL oder Connection String</param>
 Task SetCurrentBackend(string backend);

 /// <summary>
 /// Prüft, ob das Backend verfügbar ist
 /// </summary>
 /// <param name="backend">URL oder Connection String</param>
 Task<BackendState> CheckBackend(string backend);

 /// <summary>
 /// Benutzer anmelden
 /// </summary>
 Task<LoginInfo> LogIn(string username, string password, string backend);

 /// <summary>
 /// Benutzer abmelden
 /// </summary>
 Task Logout();
}