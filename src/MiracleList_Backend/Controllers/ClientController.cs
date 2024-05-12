using System;
using System.Threading.Tasks;
using BL;
using BO;
using ITVisions.Network;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MiracleList.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class ClientController : Controller
{
 // GET: /<controller>/

 public IActionResult Index()
 {
  if (HttpContext.Request.Path.ToString().Contains("ix", StringComparison.OrdinalIgnoreCase)) ViewBag.Quelle = "iX";
  return View();
 }

 // GET: /<controller>/

 public IActionResult Test()
 {

  return View("Index");
 }

 [HttpPost]
 public async Task<IActionResult> Create()
 {

  string name = HttpContext.Request.Form["C_Name"];
  string firma = HttpContext.Request.Form["C_Firma"];
  string email = HttpContext.Request.Form["C_EMail"];
  string einverstanden = HttpContext.Request.Form["C_Einverstanden"];

  if (string.IsNullOrEmpty(name)) this.ModelState.AddModelError("C_Name", "Name darf nicht leer sein!");
  if (string.IsNullOrEmpty(firma)) this.ModelState.AddModelError("C_Firma", "Firma darf nicht leer sein!");
  if (string.IsNullOrEmpty(email)) this.ModelState.AddModelError("C_EMail", "EMail darf nicht leer sein!");
  if (einverstanden != "Ja") this.ModelState.AddModelError("C_Einverstanden", "Sie müssen einverstanden sein!");
  if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(email)) this.ModelState.AddModelError("C_EMail", "EMail ungültig!");
  if (await MailUtil.IsWegwerfadresse(email)) this.ModelState.AddModelError("C_EMail", "E-Mail-Domain nicht erlaubt!");

  if (!this.ModelState.IsValid)
  {
   ViewBag.name = name;
   ViewBag.email = email;
   ViewBag.firma = firma;
   return View("Index");
  }
  var c = new Client();
  c.Name = name;
  c.Company = firma;
  c.EMail = email;
  c.Created = DateTime.Now;
  c.ClientID = Guid.NewGuid();
  c.Type = HttpContext.Request.Form["C_Quelle"]; ;

  string s = this.Request.HttpContext.Connection.RemoteIpAddress + "\n";
  foreach (var v in this.Request.Headers)
  {
   s += v.Key + ":" + v.Value + "\n";
  }

  c.Memo = s;
  var cm = new ClientManager();

  cm.New(c);

  ViewBag.Email = email;

  var text =
   $"Sie erhalten nachstehend Ihre personalisierte Client-ID. Bitte beachten Sie, dass eine Client-ID jederzeit widerrufen werden kann, wenn Sie diese missbrauchen! Bitte beachten Sie die Regeln: https://miraclelistbackend.azurewebsites.net/client\n\n" +
   $"Name: {c.Name}\n" +
   $"Firma: {c.Company}\n" +
   $"E-Mail: {c.EMail}\n" +
   (!String.IsNullOrEmpty(c.Type) ? $"Typ: {c.Type}\n" : "") +
   $"Client-ID: {c.ClientID}\n\n" +
   "Sie benötigen eine personalisierte Client-ID, wenn Sie selbst einen Beispiel-Client für das MiracleList-Backend schreiben wollen. Die Client-ID ist bei als Parameter der Login-Operation zu übergeben.\n\nDr. Holger Schwichtenberg, www.IT-Visions.de";

  var e1 = new MailUtil().SendMail("do-not-reply@mail.miraclelist.net", email, "Client-ID für MiracleList-Backend", text
    );

  new LogManager().Log(Event.ClientCreated, Severity.Information, email, "CreateClientID", "", null, this.Request.HttpContext.Connection.RemoteIpAddress.ToString(), text + "\n\n" + s);


  ViewBag.Status = e1.ToString();

  return View();
 }
}
