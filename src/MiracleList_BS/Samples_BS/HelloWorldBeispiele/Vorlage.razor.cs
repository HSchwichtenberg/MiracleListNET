using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ITVisions.Blazor;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Web.Samples_BS.HelloWorldBeispiele {
 public partial class Vorlage
 {
  [Inject]
  public BlazorUtil Util { get; set; } = null;
  [Inject]
  public IJSRuntime JSRuntime { get; set; } = null;
  [Inject]
  public NavigationManager NavigationManager { get; set; } = null;

  // Achtung: Dies nur für BS! Kein HttpContext in BW!
  [Inject]
  IHttpContextAccessor HttpContextAccessor { get; set; } = null;

  [Parameter]
  public decimal X { get; set; } = 1.23m;
  [Parameter]
  public decimal Y { get; set; } = 2.34m;

  public decimal Sum = 0;

  public Vorlage()
  {

  }

  #region Standard-Lebenszyklus-Ereignisse
  protected override void OnInitialized()
  {
   Util.Log("OnInitialized()");
  }

  protected async override Task OnInitializedAsync()
  {
   Util.Log("OnInitializedAsync()");
  }

  protected override void OnParametersSet()
  {
   Util.Log("OnParametersSet()");
  }

  protected async override Task OnParametersSetAsync()
  {
   Util.Log("OnParametersSetAsync()");
  }

  protected override void OnAfterRender(bool firstRender)
  {
   Util.Log("OnAfterRender(firstRender=" + firstRender + ")");
   // this.StateHasChanged(); // --> Endlosschleife !!! :-(
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

  public async Task AddAsync()
  {
   Sum = X + Y;
   X = Sum;
   Util.Log($"{nameof(Vorlage)}.Add(). x={X} y={Y} sum={Sum}");
   await Util.SetTitle(Sum.ToString());
  }

  #endregion
 }
}