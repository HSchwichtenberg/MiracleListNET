using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reflection;
using System.Linq.Expressions;
using ITVisions.Blazor;

namespace Web
{
 public class ListenModel : ComponentBase
 {
  [Inject]
  public BlazorUtil Util { get; set; } = null;

  #region Standard-Events
  protected override void OnInitialized()
  {
   Util.Log("Index.OnInitialized");

  }

  protected override async System.Threading.Tasks.Task OnAfterRenderAsync(bool firstRender)
  {

  }
  #endregion

  #region Reaktionen auf Ereignisse
  
  #endregion
 }
}