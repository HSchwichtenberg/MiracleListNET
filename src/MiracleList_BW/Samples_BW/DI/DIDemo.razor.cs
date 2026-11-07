#pragma warning disable 1998
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ITVisions.Blazor;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
using ITVisions;

namespace Web.Samples_BW.DI
{
 public partial class DIDemo
 {
  [Inject]
  public BlazorUtil Util { get; set; } = null;
  [Inject]
  public IJSRuntime JSRuntime { get; set; } = null;

  // Nicht möglich in BW:
  //[Inject]
  //IHttpContextAccessor HttpContextAccessor { get; set; } = null;


  [Parameter]
  public decimal X { get; set; } = 1.23m;
  [Parameter]
  public decimal Y { get; set; } = 2.34m;

  public decimal Sum = 0;

  #region Standard-Lebenszyklus-Ereignisse
  protected override void OnInitialized()
  {
   Util.Log(nameof(DI) + ".OnInitialized()");
  }

  protected override async Task OnInitializedAsync()
  {
   Util.Log(nameof(DI) + ".OnInitializedAsync()");

   // Nicht möglich in BW:
   //var request = HttpContextAccessor.HttpContext.Request;
   //Util.Log(request.ToNameValueString());

   //IJSRuntime js = (IJSRuntime)HttpContextAccessor.HttpContext.RequestServices.GetService(typeof(IJSRuntime));
   //Util.Log(js.GetType().FullName);

   // JavaScript interop calls cannot be issued at this time. This is because the component is being statically rendererd. When prerendering is enabled, JavaScript interop calls can only be performed during the OnAfterRenderAsync lifecycle method.
   //var e = await js.InvokeAsync<bool>("alert", "Hier meldet sich JavaScript");
   //if (!e) return;

  }

  protected override void OnParametersSet()
  {
   Util.Log(nameof(DI) + ".OnParametersSet()");
  }

  protected override async Task OnParametersSetAsync()
  {
   Util.Log(nameof(DI) + ".OnParametersSetAsync()");
  }

  protected override void OnAfterRender(bool firstRender)
  {
   Util.Log(nameof(DI) + ".OnAfterRender(firstRender=" + firstRender + ")");
   // this.StateHasChanged(); // --> Endlosschleife !!! :-(
  }

  protected override async Task OnAfterRenderAsync(bool firstRender)
  {
   Util.Log(nameof(DI) + ".OnAfterRenderAsync(firstRender=" + firstRender + ")");
  }

  public void Dispose()
  {
   Util.Log(nameof(DI) + ".Dispose()");
  }
  #endregion

  #region Reaktionen auf Benutzerinteraktionen

  public async Task AddAsync()
  {
   Sum = X + Y;
   X = Sum;
   Util.Log($"{nameof(DI)}.Add(). x={X} y={Y} sum={Sum}");
   await Util.SetTitle(Sum.ToString());
  }

  #endregion
 }
}