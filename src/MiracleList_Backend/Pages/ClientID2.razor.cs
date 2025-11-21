using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BL;
using BO;
using ITVisions;
using ITVisions.AspNetCore;
using ITVisions.Network;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace MiracleList_Backend.Pages;

public partial class ClientID2
{
 public class ClientIDModelResult
 {
  public string Name { get; set; }
  public string EMail { get; set; }
  public string ClientArt { get; set; }
  public Guid ClientID { get; set; }
  public string ErrorMessage { get; set; }
 }

 #region Formularfelder
 [SupplyParameterFromForm]
 [Required]
 public string Name { get; set; }
 [Required]
 [SupplyParameterFromForm]
 public string Firma { get; set; }
 [Required]
 [EmailAddress]
 [SupplyParameterFromForm]
 public string EMail { get; set; }
 [Range(typeof(bool), "true", "true")]
 [SupplyParameterFromForm]
 public bool Einverstanden { get; set; }
 [Required]
 [SupplyParameterFromForm]
 public string ClientArt { get; set; }
 #endregion

 #region Weitere Daten für UI
 public bool DownloadAnbieten
 {
  get
  {
   var webRoot = env.WebRootPath;
   var path = System.IO.Path.Combine(webRoot, "Download/Flyer.pdf");
   return System.IO.File.Exists(path);
  }
 }

 public List<SelectListItem> ClientArten { get; set; } = (new List<String>() { "Web-Client", "Desktop-Client", "Mobile Client", "Server-Anwendung" }).Select(s => new SelectListItem { Value = s, Text = s }).ToList();
 #endregion

 #region DI
 [Inject]
 private NavigationManager NavigationManager { get; set; }
 [Inject]
 private IWebHostEnvironment env { get; set; } // injected via DI
 [Inject]
 private IConfiguration config { get; set; } // injected via DI
 [CascadingParameter]
 public Microsoft.AspNetCore.Http.HttpContext HttpContext { get; set; }
 #endregion

 private EditContext editContext { get; set; }

 protected override void OnInitialized()
 {
  editContext = new(this);
  if (!env.IsProduction())
  {
   this.EMail = this.EMail.NotNull("test@IT-Visions.de");
   this.Name = this.Name.NotNull("Max Mustermann");
   this.Firma = this.Firma.NotNull("Musterfirma AG");
  }
  this.ClientArt = this.ClientArt.NotNull("Web-Client");
 }

 /// <summary>
 /// POST von Formular 1
 /// </summary>
 public void Download()
 {
  NavigationManager.NavigateTo("/Download/Flyer.pdf");
 }

 /// <summary>
 /// POST von Formular 2
 /// </summary>
 public void Beantragen()
 {
  if (this.editContext != null)
  {
   var isValid = editContext.Validate();
   if (!isValid) { return; }
  }

  #region Client via Geschäftslogik registrieren und E-Mail senden
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
   $"Sie erhalten nachstehend Ihre personalisierte Client-ID für die Nutzung der WebAPIs auf dem MiracleList-Backend (https://miraclelistbackend.azurewebsites.net).\n\nBitte beachten Sie die Regeln: https://miraclelistbackend.azurewebsites.net/client\n\nBitte beachten Sie, dass eine Client-ID jederzeit widerrufen werden kann, wenn den Zugang missbrauchen!\n\n" +
   $"Name: {c.Name}\n" +
   $"Firma: {c.Company}\n" +
   $"E-Mail: {c.EMail}\n" +
   (!String.IsNullOrEmpty(c.Type) ? $"Typ: {c.Type}\n" : "") +
   $"Client-ID: {c.ClientID}\n\n" +
   "Sie benötigen diese personalisierte Client-ID, wenn Sie selbst einen Beispiel-Client für das MiracleList-Backend schreiben wollen. Die Client-ID ist als Parameter bei der Login-Operation zu übergeben.\n\nDr. Holger Schwichtenberg, www.IT-Visions.de";

  var e1 = new MailUtil().SendMail(config["EMail:SMTPSender"], EMail, "Ihre Client-ID für MiracleList-Backend", text, false, config["EMail:SMTPCC"], config["EMail:SMTPBCC"]);

  new LogManager().Log(Event.ClientCreated, Severity.Information, EMail, "CreateClientID", "", null, HttpContext.Connection.RemoteIpAddress.ToString(), text + "\n\n" + s + "E-Mail: " + e1);
  #endregion

  // Übergabewerte in Session und Folgeseite aufrufen
  var result = new ClientIDModelResult() { Name = this.Name, EMail = this.EMail, ClientID = c.ClientID, ClientArt = ClientArt, ErrorMessage = e1 };
  HttpContext.Session.SetObject("ClientIDModelResult", result);
  NavigationManager.NavigateTo("/Confirmation2");
 }
}