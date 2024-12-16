#pragma warning disable 1998
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ITVisions.Blazor;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ITVisions;
using Microsoft.AspNetCore.Http.Features;

namespace Web.Samples_BS.Systeminformationen
{
 public partial class SystemInfoDemo
 {
  [Inject]
  public BlazorUtil Util { get; set; } = null;
  [Inject]
  public IJSRuntime JSRuntime { get; set; } = null;
  [Inject]
  public NavigationManager NavigationManager { get; set; } = null;
  //[Inject]
  IHttpContextAccessor HttpContextAccessor { get; set; } = null;

  public string Ausgabe1 { get; set; }
  public string Ausgabe2 { get; set; }

  #region Standard-Lebenszyklus-Ereignisse
  protected override void OnInitialized()
  {
   Util.Log(nameof(SystemInfoDemo) + ".OnInitialized()");

   Ausgabe1 = (new ITVisions.EnvInfo().GetString(lineseparator: "<br>"));

   var ctx = HttpContextAccessor.HttpContext;

   Ausgabe2 += "Quelle: " + ctx.GetType().FullName + "<br>";

   Ausgabe2 += "<h3>Gezielte Informationen</h3>";
   Ausgabe2 += "Browser: " + ctx.Request.Headers["User-Agent"] + "<br>";
   Ausgabe2 += "Webserver: " + ctx.GetServerVariable("SERVER_SOFTWARE") + "<br>";
   Ausgabe2 += "Server-URL: " + ctx.Request.Host + "<br>";
   Ausgabe2 += "HTTPS aktiv: " + ctx.Request.IsHttps + "<br>";
   Ausgabe2 += "Path (Startpunkt der Blazor-Anwendung): " + ctx.Request.Path + "<br>";
   Ausgabe2 += "Route (aktuelle URL des Browsers): " + NavigationManager.Uri + "<br>";

   Ausgabe2 += "<h3>Server Variables</h3>";
   var serverVars = ctx.Features.Get<IServerVariablesFeature>();
   Ausgabe2 += serverVars.ToNameValueString(attributeSeparator: "<br>");
   //foreach (var sv in serverVars) --> Kein Enumerator 
   //{

   //}

   Ausgabe2 += ctx.Connection.ToNameValueString(attributeSeparator: "<br>");


   Ausgabe2 += "<h3>Connection</h3>";
   Ausgabe2 += ctx.Connection.ToNameValueString(attributeSeparator: "<br>");

   Ausgabe2 += "<h3>WebSockets</h3>";
   Ausgabe2 += ctx.WebSockets.ToNameValueString(attributeSeparator: "<br>");

   Ausgabe2 += "<h3>Request</h3>";
   Ausgabe2 += ctx.Request.ToNameValueString(attributeSeparator: "<br>");

   Ausgabe2 += "<h3>Response</h3>";
   Ausgabe2 += ctx.Response.ToNameValueString(attributeSeparator: "<br>");

   Ausgabe2 += "<h3>Cookies</h3>";
   Ausgabe2 += ctx.Request.Cookies.ToNameValueString(attributeSeparator: "<br>");

   //Ausgabe += "<h3>Cookies</h3>";
   //Ausgabe += ctx.Get


  }



  protected async override Task OnInitializedAsync()
  {
   Util.Log(nameof(SystemInfoDemo) + ".OnInitializedAsync()");
  }

  protected override void OnParametersSet()
  {
   Util.Log(nameof(SystemInfoDemo) + ".OnParametersSet()");
  }

  protected async override Task OnParametersSetAsync()
  {
   Util.Log(nameof(SystemInfoDemo) + ".OnParametersSetAsync()");
  }

  protected override void OnAfterRender(bool firstRender)
  {
   Util.Log(nameof(SystemInfoDemo) + ".OnAfterRender(firstRender=" + firstRender + ")");
   // this.StateHasChanged(); // --> Endlosschleife !!! :-(
  }

  protected async override Task OnAfterRenderAsync(bool firstRender)
  {
   Util.Log(nameof(SystemInfoDemo) + ".OnAfterRenderAsync(firstRender=" + firstRender + ")");
  }

  public void Dispose()
  {
   Util.Log(nameof(SystemInfoDemo) + ".Dispose()");
  }
  #endregion

  #region Reaktionen auf Benutzerinteraktionen



  #endregion
 }
}