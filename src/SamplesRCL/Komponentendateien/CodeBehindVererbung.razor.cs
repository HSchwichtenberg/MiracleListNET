using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ITVisions.Blazor;
using System.Threading.Tasks;

namespace Samples.Komponentendateien
{
 public partial class CodeBehindVererbungModel : ComponentBase, IDisposable
 {
  [Inject]
  public BlazorUtil Util { get; set; } = null;
  [Inject]
  public IJSRuntime JSRuntime { get; set; } = null;
  [Inject]
  public NavigationManager NavigationManager { get; set; } = null;

  [Parameter]
  public decimal X { get; set; } = 1.23m;
  [Parameter]
  public decimal Y { get; set; } = 2.34m;

  public decimal Sum = 0;

  #region Standard-Lebenszyklus-Ereignisse
  protected override void OnInitialized()
  {
   Util.Log(nameof(CodeBehindVererbungModel) + ".OnInitialized()");
  }

  protected async override Task OnInitializedAsync()
  {
   Util.Log(nameof(CodeBehindVererbungModel) + ".OnInitializedAsync()");
  }

  protected override void OnParametersSet()
  {
   Util.Log(nameof(CodeBehindVererbungModel) + ".OnParametersSet()");
  }

  protected async override Task OnParametersSetAsync()
  {
   Util.Log(nameof(CodeBehindVererbungModel) + ".OnParametersSetAsync()");
  }

  protected override void OnAfterRender(bool firstRender)
  {
   Util.Log(nameof(CodeBehindVererbungModel) + ".OnAfterRender(firstRender=" + firstRender + ")");
   // this.StateHasChanged(); // --> Endlosschleife !!! :-(
  }

  protected async override Task OnAfterRenderAsync(bool firstRender)
  {
   Util.Log(nameof(CodeBehindVererbungModel) + ".OnAfterRenderAsync(firstRender=" + firstRender + ")");
  }

  public void Dispose()
  {
   Util.Log(nameof(CodeBehindVererbungModel) + ".Dispose()");
  }
  #endregion

  #region Reaktionen auf Benutzerinteraktionen

  public async Task AddAsync()
  {
   Sum = X + Y;
   X = Sum;
   Util.Log($"{nameof(CodeBehindVererbungModel)}.Add(). x={X} y={Y} sum={Sum}");
   await Util.SetTitle(Sum.ToString());
  }

  #endregion
 }
}