using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MiracleList.Pages
{
 public class UserListModel : PageModel
 {

  [BindProperty]
  public DataTable UserDT { get; set; }

  public void OnGet()
  {


   UserDT = new DataTable();
   UserDT.Columns.Add("UserName");
   UserDT.Columns.Add("CategoryCount");
   if (DA.Context.AdditionalColumnSet != null)
   {
    foreach (var col in DA.Context.AdditionalColumnSet.Where(x => x.StartsWith("BO.User")))
    {
     string columnname = col.Split(';')[1];
     UserDT.Columns.Add(columnname);
    }
   }

   using (var ctx = new DA.Context())
   {
    var UserList = ctx.UserSet.Include(x => x.CategorySet).ToList();

    foreach (var u in UserList)
    {
     var row = UserDT.NewRow();
     row["UserName"] = u.UserName;
     row["CategoryCount"] = u.CategorySet.Count;

     if (DA.Context.AdditionalColumnSet != null)
     {
      foreach (var col in DA.Context.AdditionalColumnSet.Where(x => x.StartsWith("BO.User")))
      {
       string columnname = col.Split(';')[1];
       // access shadow properties using ctx.Entry()
       row[columnname] = ctx.Entry(u).Property(columnname).CurrentValue;
      }
     }
     this.UserDT.Rows.Add(row);
    }

   }

  }
 }
}