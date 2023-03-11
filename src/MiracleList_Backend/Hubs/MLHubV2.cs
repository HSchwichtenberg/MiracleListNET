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
 /// Version 2 des Hub: Für Gruppenzugehörigkeit wird der Username statt Token verwendet
 /// </summary>
 public class MLHubV2 : Hub<IMLHub>
 {
  //[HubMethodName("Register")] -> nur, wenn Name der Nachricht anders als Methodennamen sein soll
  public async Task Register(string token)
  {
   var u = new BL.UserManager(token);
   // Prüfe die Gültigkeit des usernames
   if (u.IsValid() != BL.UserManager.TokenValidationResult.Ok) return;
   // Alle Clients (== Browser) eines Benutzers bilden eine Gruppe (anhand des username)
   await base.Groups.AddToGroupAsync(Context.ConnectionId, u.CurrentUser.UserName);
  }

  public async Task CategoryListUpdate(string username)
  {
   // Protokollierung
   new BL.LogManager().Log(BO.Event.Call, BO.Severity.Information, "User=" + username, nameof(CategoryListUpdate));
   // Sende Benachrichtigung an die ganze Gruppe, außer der aktuellen Verbindung!
   await Clients.OthersInGroup(username).CategoryListUpdate(Context.ConnectionId);
  }

  public async Task TaskListUpdate(string username, int categoryID)
  {
   // Protokollierung
   new BL.LogManager().Log(BO.Event.Call, BO.Severity.Information, "User=" + username + "Category=" + categoryID, nameof(TaskListUpdate));
   // Sende Benachrichtigung an die ganze Gruppe, außer der aktuellen Verbindung!
   await Clients.OthersInGroup(username).TaskListUpdate(Context.ConnectionId, categoryID);
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