using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace MiracleList.Pages
{
 public class ClientIDConfirmationModel : PageModel
 {
  [TempData]
  public string ClientIDModel_Result { get; set; }
  [BindProperty]
  public ClientIDModelResult ClientIDModelResult { get; set; }

  public void OnGet()
  {
   //(string Name, string EMail) info = (this.Name, this.EMail);
   this.ClientIDModelResult = JsonConvert.DeserializeObject<ClientIDModelResult>(this.ClientIDModel_Result);
  }
 }
}