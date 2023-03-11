#pragma warning disable 1998
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ITVisions.Blazor;
using System.Threading.Tasks;

namespace Samples.Routing
{

 [Route("/DemoRouting")]
 public partial class RoutingDemo
 {
  #region Standard-Lebenszyklus-Ereignisse

  protected async override Task OnInitializedAsync()
  {
   Util.Log("OnInitializedAsync()");
  }

  //protected override void OnParametersSet()
  //{
  // Util.Log("OnParametersSet()");
  //}

  protected async override Task OnParametersSetAsync()
  {
   Util.Log("OnParametersSetAsync()");
  }

  protected async override Task OnAfterRenderAsync(bool firstRender)
  {
   Util.Log("OnAfterRenderAsync(firstRender=" + firstRender + ")");
  }

  public void Dispose()
  {
   Util.Log("Dispose()");
  }
  #endregion

  #region Reaktionen auf Benutzerinteraktionen



  #endregion
 }
}