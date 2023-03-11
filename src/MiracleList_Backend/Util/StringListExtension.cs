using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace ITVisions.AspNetCore
{
 public static class StringListExtension
 {
  public static List<SelectListItem> ToSelectListItem(this List<string> liste)
  {
   var l = new List<SelectListItem>();
   foreach (var s in liste)
   {
    l.Add(new SelectListItem() { Text = s });
   }

   return l;
  }
 }
}
