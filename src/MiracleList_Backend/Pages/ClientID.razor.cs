using System;
using BO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ITVisions;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using BL;
using ITVisions.AspNetCore;
using ITVisions.Network;

namespace MiracleList_Backend.Pages;

public partial class ClientID
{

 public class ClientIDModelResult
 {
  public string Name { get; set; }
  public string EMail { get; set; }
  public Guid ClientID { get; set; }
  public string ErrorMessage { get; set; }
 }

 public string Name { get; set; }
 public string Firma { get; set; }
 public string EMail { get; set; }
 public bool Einverstanden { get; set; }
 public string ClientArt { get; set; }

 public bool DownloadAnbieten
 {
  get
  {
   var webRoot = env.WebRootPath;
   var path = System.IO.Path.Combine(webRoot, "Download/Flyer.pdf");
   return System.IO.File.Exists(path);
  }
 }
 public int Aufrufe { get; set; }
 public List<SelectListItem> ClientArten { get; set; } = (new List<String>() { "Web-Client", "Desktop-Client", "Mobile Client", "Server-Anwendung" }).Select(s => new SelectListItem { Value = s }).ToList();

 [Inject]
 private NavigationManager NavigationManager { get; set; }
 [Inject]
 private IWebHostEnvironment env { get; set; } // injected via DI

 [CascadingParameter]
 public HttpContext? HttpContext { get; set; }

 protected override void OnInitialized()
 {
  if (!env.IsProduction())
  {
   this.EMail = "test@IT-Visions.de";
   this.Name = "Test";
   this.Firma = "Test";
  }
 }

 public void OnPostBeantragen()
 {
  #region Logik
  // Client via Geschäftslogik registrieren und E-Mail senden

  string Type = (ClientArt + (HttpContext.Request.Form.ContainsKey("C_Quelle") && !String.IsNullOrEmpty(HttpContext.Request.Form["C_Quelle"].ToString()) ? "/" + HttpContext.Request.Form["C_Quelle"] : "")).Truncate(20);

  var c = new Client
  {
   Name = Name,
   Company = Firma,
   EMail = EMail,
   Created = DateTime.Now,
   ClientID = Guid.NewGuid(),
   Type = Type
  };
  ;

  HttpContext.Session.SetObject("Client", c);

  string s = this.HttpContext.Connection.RemoteIpAddress + "\n";
  foreach (var v in HttpContext.Request.Headers)
  {
   s += v.Key + ":" + v.Value + "\n";
  }

  c.Memo = s;
  var cm = new ClientManager();

  cm.New(c);

  var text =
   $"Sie erhalten nachstehend Ihre personalisierte Client-ID. Bitte beachten Sie, dass eine Client-ID jederzeit widerrufen werden kann, wenn Sie diese missbrauchen! Bitte beachten Sie die Regeln: https://miraclelistbackend.azurewebsites.net/client\n\n" +
   $"Name: {c.Name}\n" +
   $"Firma: {c.Company}\n" +
   $"E-Mail: {c.EMail}\n" +
   (!String.IsNullOrEmpty(c.Type) ? $"Typ: {c.Type}\n" : "") +
   $"Client-ID: {c.ClientID}\n\n" +
   "Sie benötigen eine personalisierte Client-ID, wenn Sie selbst einen Beispiel-Client für das MiracleList-Backend schreiben wollen. Die Client-ID ist als Parameter bei der Login-Operation zu übergeben.\n\nDr. Holger Schwichtenberg, www.IT-Visions.de";

  var e1 = new MailUtil().SendMail("do-not-reply@mail.miraclelist.net", EMail, "Client-ID für MiracleList-Backend", text
    );

  new LogManager().Log(Event.ClientCreated, Severity.Information, EMail, "CreateClientID", "", null, HttpContext.Connection.RemoteIpAddress.ToString(), text + "\n\n" + s + "E-Mail: " + e1);

  #endregion

  var result = new ClientIDModelResult() { Name = this.Name, EMail = this.EMail, ClientID = c.ClientID, ErrorMessage = e1 };

  HttpContext.Session.SetObject("ClientIDModelResult", result);

  NavigationManager.NavigateTo("/Confirmation2");

 }

}
