using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BL;
using BO;
using ITVisions;
using ITVisions.AspNetCore; // Erweiterungsmethoden einbinden
using ITVisions.Network;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SelectListItem = Microsoft.AspNetCore.Mvc.Rendering.SelectListItem;

namespace MiracleList.Pages
{

 public class ClientIDModelResult
 {
  public string Name { get; set; }
  public string EMail { get; set; }
  public Guid ClientID { get; set; }
  public string ErrorMessage { get; set; }
 }

 // sollte in 2.1 gehen, geht aber nicht [BindPropertyAttribute], siehe https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.bindpropertyattribute?view=aspnetcore-2.1
 public class ClientIDModel : PageModel
 {

  // das geht nicht in Razor Pages, sondern nur in Blazor
  //[Inject]
  //public IWebHostEnvironment env2 { get; set; }

  public void Dispose()
  {

  }

  #region Einfache Properties für Ein-Wege-Bindung
  // KEIN [BindProperty] für statische/read-only Daten, die der Browser nicht zurücksendet
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
  public List<SelectListItem> ClientArten { get; set; } = (new List<String>() { "Web-Client", "Desktop-Client", "Mobile Client", "Server-Anwendung" }).Select(s => new SelectListItem { Value = s, Text = s }).ToList();
  #endregion

  #region Properties für Zwei-Wege-Bindung
  [BindProperty]
  [Required]

  public string Name { get; set; }
  [BindProperty]
  public string Firma { get; set; }
  [BindProperty]
  public string EMail { get; set; }
  [BindProperty]
  public bool Einverstanden { get; set; }
  [BindProperty]
  public string ClientArt { get; set; }
  #endregion

  #region Properties für Datenübergabe an Folgeseite mit [TempData]
  //[TempData]// kann nicht gleichzeitig [BindProperty] sein
  // System.InvalidOperationException: The 'MiracleList.Pages.ClientIDModel.ClientIDModelResult' property with TempDataAttribute is invalid. A property using TempDataAttribute must be of primitive or string type.
  //public ClientIDModelResult ClientIDModelResult { get; set; }

  [TempData]
  public string ClientIDModel_Result { get; set; } // Name and EMail can be serialized here in one JSON string
                                                   // Alternative: One string per Input Box
  [TempData]
  public string ClientIDModel_EMail { get; set; }
  [TempData]
  public string ClientIDModel_Name { get; set; }
  #endregion

  //public ClientIDModel()
  //{
  //// alternativ: ClientArten = (new List<String>() { "Web-Client", "Desktop-Client", "Mobile Client", "Server-Anwendung" }).ToSelectListItem();
  //}
  //public async void OnGetAsync()
  //{
  //}

  private IWebHostEnvironment env; // injected via DI
  private readonly IConfiguration config; // injected via DI

  public ClientIDModel(IWebHostEnvironment env, IConfiguration config)
  {
   this.env = env;
   this.config = config;
  }

  public void OnGet()
  {
   // ViewBag not available in Razor Pages! ViewBag.ClientArten = ClientArten;

   // optionale Daten aus Route berücksichtigen
   if (RouteData.Values["source"] != null)
   {
    ;
    this.ClientArten = (new List<String>() { RouteData.Values["source"].ToString() }).Select(s => new SelectListItem { Value = s }).ToList();
   }

   // Counter via Session
   int aufrufe = 0;
   aufrufe = HttpContext.Session.GetInt32("aufrufe") ?? 0;
   HttpContext.Session.SetInt32("aufrufe", ++aufrufe);
   this.Aufrufe = aufrufe;

   // if the user was here before, show his data (he might register multiple clients)
   var client = HttpContext.Session.GetObject<Client>("Client");
   if (client != null)
   {
    this.EMail = client.EMail;
    this.Name = client.Name;
    this.Firma = client.Company;
   }
   else
   {
    if (env.EnvironmentName == "Development")
    {
     this.EMail = "test@IT-Visions.de";
     this.Name = "Test";
     this.Firma = "Test";
    }
   }
  }

  /// <summary>
  /// Handler für Download-Schaltfläche
  /// </summary>
  public IActionResult OnPostDownload()
  {
   var webRoot = env.WebRootPath;
   var path = System.IO.Path.Combine(webRoot, "Download/Flyer.pdf");
   return PhysicalFile(path, "application/pdf");
  }

  /// <summary>
  /// Handler für Beantragen-Schaltfläche
  /// </summary>
  public IActionResult OnPostBeantragen()
  {
   #region Validierung

   // [Required] wirkt nicht (vgl. https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.1&tabs=visual-studio#mark-page-properties-required), auch nicht mit TryValidateModel(this);
   //daher hilft das nicht: if (!ModelState.IsValid) return Page();

   if (string.IsNullOrEmpty(Name)) this.ModelState.AddModelError(nameof(Name), "Name darf nicht leer sein!");
   if (string.IsNullOrEmpty(Firma)) this.ModelState.AddModelError(nameof(Firma), "Firma darf nicht leer sein!");
   if (string.IsNullOrEmpty(EMail)) this.ModelState.AddModelError(nameof(EMail), "EMail darf nicht leer sein!");
   if (string.IsNullOrEmpty(ClientArt)) this.ModelState.AddModelError(nameof(EMail), "ClientArt darf nicht leer sein!");
   if (this.Einverstanden != true) this.ModelState.AddModelError(nameof(Einverstanden), "Sie müssen einverstanden sein!");

   if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(EMail)) this.ModelState.AddModelError(nameof(EMail), "EMail ungültig!");
   if (MailUtil.IsWegwerfadresse(EMail).Result) this.ModelState.AddModelError(nameof(EMail), "E-Mail-Domain nicht erlaubt!");

   if (!this.ModelState.IsValid)
   {
    return Page();
   }
   #endregion

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

   string s = this.Request.HttpContext.Connection.RemoteIpAddress + "\n";
   foreach (var v in this.Request.Headers)
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

   var e1 = new MailUtil().SendMail(config["EMail:SMTPSender"], EMail, "Client-ID für MiracleList-Backend", text, false, config["EMail:SMTPCC"], config["EMail:SMTPBCC"]);

   new LogManager().Log(Event.ClientCreated, Severity.Information, EMail, "CreateClientID", "", null, this.Request.HttpContext.Connection.RemoteIpAddress.ToString(), text + "\n\n" + s + "E-Mail: " + e1);

   #endregion

   // Übergabewerte einzeln setzen
   // this.ClientIDModel_EMail = this.EMail;
   // this.ClientIDModel_Name = this.Name;

   // oder serialisieren:
   var result = new ClientIDModelResult() { Name = this.Name, EMail = this.EMail, ClientID = c.ClientID, ErrorMessage = e1 };

   this.ClientIDModel_Result = JsonConvert.SerializeObject(result);

   if (!String.IsNullOrEmpty(e1))
   {
    new LogManager().Log(Event.ClientCreated, Severity.Information, "Error Sending mail to: " + EMail, "CreateClientID", "", null, this.Request.HttpContext.Connection.RemoteIpAddress.ToString(), e1);

    // Folgeseite aufrufen
    return RedirectToPage("./" + nameof(ClientIDErrorModel).Replace("Model", ""));
   }
   else
   {
    // Folgeseite aufrufen
    return RedirectToPage("./" + nameof(ClientIDConfirmationModel).Replace("Model", ""));
   }
  }


 }
}