using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DA;
using ITVisions;
using Microsoft.AspNetCore.SignalR;

namespace MiracleList_Backend.Hubs
{

 /// <summary>
 /// DEMO: 60. SignalR-Hub (im Backend)
 /// TODO: Umstellen auf Strongly Typed Hub Strongly --> https://docs.microsoft.com/en-us/aspnet/core/signalr/hubs?view=aspnetcore-5.0
 /// </summary>
 public class MLHub : Hub<IMLHub>
 {
  //[HubMethodName("Register")] -> nur, wenn Name der Nachricht anders als Methodennamen sein soll
  public async Task Register(string token)
  {
   // Prüfe die Gültigkeit des Tokens
   if (new BL.UserManager(token).IsValid() != BL.UserManager.TokenValidationResult.Ok) return;
   // Alle Clients (== Browser) eines Benutzers bilden eine Gruppe (anhand des Token)
   await base.Groups.AddToGroupAsync(Context.ConnectionId, token);
  }

  public async Task CategoryListUpdate(string token)
  {
   var u = new BL.UserManager(token).CurrentUser;
   if (u == null) return;
   // Protokollierung
   new BL.LogManager().Log(BO.Event.Call, BO.Severity.Information, "User=" + u.UserName, nameof(CategoryListUpdate));
   // Sende Benachrichtigung an die ganze Gruppe, außer der aktuellen Verbindung!
   await Clients.OthersInGroup(token).CategoryListUpdate(Context.ConnectionId);
  }

  public async Task TaskListUpdate(string token, int categoryID)
  {
   var u = new BL.UserManager(token).CurrentUser;
   if (u == null) return;
   if (new BL.UserManager(token).CurrentUser == null) return;
   // Protokollierung
   new BL.LogManager().Log(BO.Event.Call, BO.Severity.Information, "User=" + u.UserName + "Category=" + categoryID, nameof(TaskListUpdate));
   // Sende Benachrichtigung an die ganze Gruppe, außer der aktuellen Verbindung!
   await Clients.OthersInGroup(token).TaskListUpdate(Context.ConnectionId, categoryID);
  }

  /// <summary>
  /// Beim Aufbau einer Verbindung eines Clients zu diesem Hub
  /// </summary>
  public override async Task OnConnectedAsync()
  {
   var logMessage = "";
   logMessage = logMessage.AddLine(DateTime.Now.ToString() + " OnConnectedAsync: " + Context.ConnectionId.ToString() + "/" + Context.UserIdentifier);
   //foreach (var f in Context.Features)
   //{
   // logMessage = logMessage.AddLine(f.Key + ": " + f.Value);
   //}
   WriteToLog(logMessage);
   await base.OnConnectedAsync();
  }

  /// <summary>
  /// Beim Beenden einer Verbindung eines Clients zu diesem Hub
  /// </summary>
  public override async Task OnDisconnectedAsync(Exception exception)
  {
   var logMessage = "";
   logMessage = logMessage.AddLine(DateTime.Now.ToString() + " OnDisconnectedAsync: " + Context.ConnectionId.ToString() + "/" + Context.UserIdentifier);
   WriteToLog(logMessage);
   await base.OnDisconnectedAsync(exception);
  }

  private static void WriteToLog(string logMessage)
  {
   //using (StreamWriter w = File.AppendText(@"t:\log.txt"))
   //{
   // w.WriteLine(logMessage + "\n");
   //}
  }

 }
}