using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DA;
using ITVisions;
using Microsoft.AspNetCore.SignalR;

namespace MiracleList_Backend.Hubs {

 /// <summary>
 /// DEMO: 60. SignalR-Hub (im Backend)
 /// </summary>
 public class MLHub : Hub<IMLHub> {
  /// <summary>
  /// Die UserID ist eigentliche eine Zahl. Zur Kompatibilität mit dem 3-Tier-Ansatz, wo ein Token (Text) verwendet wird, wird hier auch die UserID per Text übermittelt
  /// </summary>
  /// <param name="userID"></param>
  private void CheckUserID(string userID) {
   int userIDInt = 0;
   if (!Int32.TryParse(userID, out userIDInt)) throw new ApplicationException("Wrong userID format");
   var u = new BL.UserManager(Int32.Parse(userID)).CurrentUser;
   if (u == null) throw new ApplicationException("Unknown user ID");
  }

  //[HubMethodName("Register")] -> nur, wenn Name der Nachricht anders als Methodennamen sein soll
  public async Task Register(string userID) {
   CheckUserID(userID);

   // Alle Clients (== Browser) eines Benutzers bilden eine Gruppe (anhand des Token)
   await base.Groups.AddToGroupAsync(Context.ConnectionId, userID);
  }

  public async Task CategoryListUpdate(string userID) {
   CheckUserID(userID);

   // Protokollierung
   new BL.LogManager().Log(BO.Event.Call, BO.Severity.Information, "User=" + userID, nameof(CategoryListUpdate));
   // Sende Benachrichtigung an die ganze Gruppe, außer der aktuellen Verbindung!
   await Clients.OthersInGroup(userID).CategoryListUpdate(Context.ConnectionId);
  }

  public async Task TaskListUpdate(string userID, int categoryID) {
   CheckUserID(userID);

   // Protokollierung
   new BL.LogManager().Log(BO.Event.Call, BO.Severity.Information, "User=" + userID + "Category=" + categoryID, nameof(TaskListUpdate));
   // Sende Benachrichtigung an die ganze Gruppe, außer der aktuellen Verbindung!
   await Clients.OthersInGroup(userID).TaskListUpdate(Context.ConnectionId, categoryID);
  }

  /// <summary>
  /// Beim Aufbau einer Verbindung eines Clients zu diesem Hub
  /// </summary>
  public override async Task OnConnectedAsync() {
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
  public override async Task OnDisconnectedAsync(Exception exception) {
   var logMessage = "";
   logMessage = logMessage.AddLine(DateTime.Now.ToString() + " OnDisconnectedAsync: " + Context.ConnectionId.ToString() + "/" + Context.UserIdentifier);
   WriteToLog(logMessage);
   await base.OnDisconnectedAsync(exception);
  }

  private static void WriteToLog(string logMessage) {
   //using (StreamWriter w = File.AppendText(@"t:\log.txt"))
   //{
   // w.WriteLine(logMessage + "\n");
   //}
  }

 }
}
