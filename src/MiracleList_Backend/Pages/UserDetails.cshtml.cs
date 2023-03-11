using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BL;
using BO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MiracleList.Pages
{
 public class UserDetailsModel : PageModel
 {
  public string Message { get; set; }
  public User MLUser { get; set; }


 public UserDetailsModel()
  {

  }
  //public void OnGet()
  //{
  // this.Message = "Kein Parameter!";
  //}

  public void OnGet(int? id)
  {
   if (id == null)
   {
    this.Message = "Parameter id= fehlt!";
    return;
   }
   var um = new UserManager(id.Value);
   this.MLUser = um.CurrentUser;
   if (User!= null)    this.Message = "User #" + id + " geladen!";
   else this.Message = "User #" + id + " nicht gefunden!";
  }
 }
}