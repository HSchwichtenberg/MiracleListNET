using System;
using System.IO;
using System.Threading.Tasks;
using ITVisions;
using Microsoft.AspNetCore.SignalR;

namespace MiracleList_Backend.Hubs;

/// <summary>
/// Version 2 des Hub: Für Gruppenzugehörigkeit wird User-GUID statt Token verwendet
/// </summary>
public class MLHubV2 : Hub<IMLHub>
{
 //[HubMethodName("Register")] -> nur, wenn Name der Nachricht anders als Methodennamen sein soll
 public async Task Register(string token)
 {
  var u = new BL.UserManager(token);
  // Prüfe die Gültigkeit des Tokens
  if (u.IsValid() != BL.UserManager.TokenValidationResult.Ok) return;
  // Alle Clients (== Browser) eines Benutzers bilden eine Gruppe (anhand der User-GUID)
  await base.Groups.AddToGroupAsync(Context.ConnectionId, u.CurrentUser.UserGUID.ToString());
  CUI.PrintStep("SignalR.Register: Connection " + Context.ConnectionId + " for User" + u.CurrentUser.UserName);
 }

 public async Task CategoryListUpdate(string token)
 {
  var u = new BL.UserManager(token);
  // Prüfe die Gültigkeit des Tokens
  if (u.IsValid() != BL.UserManager.TokenValidationResult.Ok) return;
  // Protokollierung
  new BL.LogManager().Log(BO.Event.Call, BO.Severity.Information, "User=" + u.CurrentUser.UserName, nameof(CategoryListUpdate));
  // Sende Benachrichtigung an die ganze Gruppe, außer der aktuellen Verbindung!
  await Clients.OthersInGroup(u.CurrentUser.UserGUID.ToString()).CategoryListUpdate(Context.ConnectionId);
  CUI.PrintStep("SignalR.CategoryListUpdate: Connection " + Context.ConnectionId + " for User" + u.CurrentUser.UserName);
 }

 public async Task TaskListUpdate(string token, int categoryID)
 {
  var u = new BL.UserManager(token);
  // Prüfe die Gültigkeit des Tokens
  if (u.IsValid() != BL.UserManager.TokenValidationResult.Ok) return;
  // Protokollierung
  new BL.LogManager().Log(BO.Event.Call, BO.Severity.Information, "User=" + u.CurrentUser.UserName + "Category=" + categoryID, nameof(TaskListUpdate));
  // Sende Benachrichtigung an die ganze Gruppe, außer der aktuellen Verbindung!
  await Clients.OthersInGroup(u.CurrentUser.UserGUID.ToString()).TaskListUpdate(Context.ConnectionId, categoryID);
  CUI.PrintStep("SignalR.TaskListUpdate: Connection " + Context.ConnectionId + " for User" + u.CurrentUser.UserName);
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

 static bool LogActive = false;
 private static void WriteToLog(string logMessage)
 {
  if (!LogActive) return;
  using (StreamWriter w = File.AppendText(@"signallog.txt"))
  {
   w.WriteLine(logMessage + "\n");
  }
 }
}