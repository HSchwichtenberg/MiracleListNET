using System.Threading.Tasks;

namespace MiracleList_Backend.Hubs;

/// <summary>
/// Schnittstelle für MLHub.
/// Wurde hier integriert, damit Backend und Clients die Namen nutzen können
/// </summary>
public interface IMLHub
{
 Task Register(string Token);
 Task CategoryListUpdate(string Token);
 Task TaskListUpdate(string Token, int categoryID);
}