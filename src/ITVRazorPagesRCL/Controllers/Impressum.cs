using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ITVRazorClassLibrary.Controllers
{
 public class Impressum : Controller
 {
  // GET: /<controller>/
  public IActionResult Index()
  {

   ViewBag.Adresse = @"Max Mustermann (Inhaber)<BR>
 Musterfirma<BR>
 Musterstraße 40b<br>
 D-12345 Musterstadt (Deutschland)";
   return View();
  }
 }
}
