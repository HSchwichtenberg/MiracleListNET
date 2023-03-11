using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MiracleList.Controllers
{
 public class Impressum : Controller
 {
  // GET: /<controller>/
  public IActionResult Index()
  {

   ViewBag.Adresse = @"Dr. Holger Schwichtenberg (Owner)<BR>
 www.IT-Visions.de<BR>
 Fahrenberg 40b<br>
 D-45257 Essen / Ruhrgebiet (Germany)";
   return View();
  }
 }
}
