using ITVisions.AspNetCore;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using static MiracleList_Backend.Pages.ClientID2;

namespace MiracleList_Backend.Pages;

public partial class ClientIDConfirmation2
{
 public ClientIDModelResult ClientIDModelResult { get; set; }

 [CascadingParameter]
 public HttpContext HttpContext { get; set; }

 protected override void OnInitialized()
 {
  this.ClientIDModelResult = HttpContext.Session.GetObject<ClientIDModelResult>("ClientIDModelResult");
 }
}