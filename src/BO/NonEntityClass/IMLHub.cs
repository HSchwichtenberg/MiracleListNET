using System.Threading.Tasks;

namespace MiracleList_Backend.Hubs;

/// <summary>
/// Schnittstelle für MLHub. Version 2
/// Wurde hier integriert, damit Backend und Clients die Namen nutzen können
/// </summary>
public interface IMLHubV2
{
 Task Register(string Token);
 Task CategoryListUpdate(string Token);
 Task TaskListUpdate(string Token, int categoryID);
}

/// <summary>
/// Schnittstelle für MLHub. Version 3
/// Wurde hier integriert, damit Backend und Clients die Namen nutzen können
/// Gegenüber Version 2 überträgt TaskListUpdate nun nicht nur die ID der Kategorie, sondern das ganze Kategorie-Objekt, damit der Client auch den Namen auslesen kann.
/// </summary>
public interface IMLHubV3
{
 Task Register(string Token);
 Task CategoryListUpdate(string Token);
 Task TaskListUpdate(string Token, BO.Category categoryID);
}