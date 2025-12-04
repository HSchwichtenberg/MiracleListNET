
using BO;
using DA;
using ITVisions.EFCore;

namespace BL
{
 public class LogManager : EntityManagerBase<Context, Log>
 {

  public int Log(Event _event, Severity severity, string text = "", string operation = "", string token = "", int? userID = null, string client = "", string clientDetails = "")
  {
   text = text ?? "";
   operation = operation ?? "";
   token = token ?? "";
   client = client ?? "";
   clientDetails = clientDetails ?? "";

   var l = new Log() { Event = _event, Severity = severity, Text = text, UserID = userID, Token = token, Operation = operation, Client = client, ClientDetails = clientDetails };
   this.New(l);
   return l.LogID;
  }

 }
}
