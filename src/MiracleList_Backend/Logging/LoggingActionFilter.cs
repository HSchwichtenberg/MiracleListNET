using BL;
using BO;
using Microsoft.AspNetCore.Mvc.Filters;
using ITVisions;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Controllers;

/// <summary>
/// Logging jeder Action in DB
/// wird im Rahmen von ASP.NET Core MVC vor und nach jeder Aktion ausgeführt
/// https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters#action-filters
/// </summary>
public class LoggingActionFilter : IActionFilter
{
 public void OnActionExecuting(ActionExecutingContext context)
 {
  var user = context.HttpContext.User.Identity;
  int userID = 0;
  int? userIDNull = null;
  if (Int32.TryParse(context.HttpContext.User.Identity.Name, out userID))
  {
   userIDNull = userID;
  }

  string token = context.HttpContext.User.Claims.Where(x => x.Type == "Token").Select(c => c.Value).SingleOrDefault();

  string action = "";
  var cad = (context.ActionDescriptor as ControllerActionDescriptor);
  if (cad!=null)
  {
   if (cad.ControllerName.Contains("V2")) action = "v2/";
   else action = "v1/";
   action += cad.ActionName;
  }
  else
  {
   action = context.ActionDescriptor.DisplayName;
  }

  var text = "";
  foreach (var a in context.ActionArguments)
  {
   text += a.Key + ": " + a.Value.ToNameValueString() +"\n";
  }

  string s = "";
  foreach (var v in context.HttpContext.Request.Headers)
  {
   s += v.Key + ":" + v.Value + "\n";
  }

  new LogManager().Log(Event.Call, Severity.Information, text, action, token, userIDNull, context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString(),s );
 }

 public void OnActionExecuted(ActionExecutedContext context)
 {
  // do something after the action executes
 }
}