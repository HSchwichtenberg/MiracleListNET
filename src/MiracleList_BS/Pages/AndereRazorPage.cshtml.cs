using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages
{
 public class AndereRazorPageModel : PageModel
 {
  public string Title { get; set; }
  public int Count { get; set; }
  public void OnGet(string title, int count)
  {
   this.Count = count;
   this.Title = title;
  }
 }
}
