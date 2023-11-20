using System;
using System.Collections.Generic;
using System.Linq;
using BO;
using DA;
using ITVisions.EFCore;
using Microsoft.EntityFrameworkCore;

namespace BL
{
 public class ClientManager : EntityManagerBase<Context, Client>
 {

  public class CheckClientResult
  {
   public CheckClientResultCode CheckClientResultCode;
   public Client client;
  }
  public enum CheckClientResultCode
  {
   Ok, ClientIDWrongFormat, ClientIDUnknown, ClientLimitExceeded, ClientIDDuplicate

  }

  public void Trace(string s)
  {
   System.Diagnostics.Debug.WriteLine(s);
  }

  public CheckClientResult CheckClient(string guidStr)
  {
   //ctx.Log(Trace);
   var e = new CheckClientResult();
   if (guidStr == "TODO: Ihre erhaltene ClientID") new CheckClientResult() { CheckClientResultCode = CheckClientResultCode.Ok };
   Guid guid;
   if (!Guid.TryParse(guidStr, out guid)) new CheckClientResult() { CheckClientResultCode = CheckClientResultCode.ClientIDWrongFormat };
   //ctx.ClientSet.OrderBy(x=>x.ClientID).Last();

   //   System.InvalidCastException : Unable to cast object of type 'System.Guid' to type 'System.String'.
   var clients = ctx.ClientSet.Where(x => x.ClientID == guid).ToList();
   if (clients.Count == 0) return new CheckClientResult() { CheckClientResultCode = CheckClientResultCode.ClientIDUnknown };
   if (clients.Count > 1) return new CheckClientResult() { CheckClientResultCode = CheckClientResultCode.ClientIDDuplicate };
   return new CheckClientResult() { client = clients.ElementAt(0), CheckClientResultCode = CheckClientResultCode.Ok };
  }
 }
}