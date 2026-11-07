using System;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ITVisions.Blazor;
using System.Threading.Tasks;

namespace Samples.Komponentendateien
{
 public partial class CodeBehindPartielleKlasse
 {
  // Schon injiziert in Razor-Seite
  //[Inject]
  //public BlazorUtil Util { get; set; } = null;
  [Inject]
  public IJSRuntime JSRuntime { get; set; } = null;
  [Inject]
  public NavigationManager NavigationManager { get; set; } = null;

  [Parameter]
  public decimal X { get; set; } = 1.23m;
  [Parameter]
  public decimal Y { get; set; } = 2.34m;
  [Parameter]
  public EventCallback<decimal> ValueHasChanged { get; set; }

  public decimal Sum = 0;

  #region Standard-Lebenszyklus-Ereignisse
  protected override void OnInitialized()
  {
   Util.Log(nameof(CodeBehindPartielleKlasse) + ".OnInitialized()");
  }

  protected async override Task OnInitializedAsync()
  {
   Util.Log(nameof(CodeBehindPartielleKlasse) + ".OnInitializedAsync()");
  }

  protected override void OnParametersSet()
  {
   Util.Log(nameof(CodeBehindPartielleKlasse) + ".OnParametersSet()");
  }

  protected async override Task OnParametersSetAsync()
  {
   Util.Log(nameof(CodeBehindPartielleKlasse) + ".OnParametersSetAsync()");
  }

  protected override void OnAfterRender(bool firstRender)
  {
   Util.Log(nameof(CodeBehindPartielleKlasse) + ".OnAfterRender(firstRender=" + firstRender + ")");
   // this.StateHasChanged(); // --> Endlosschleife !!! :-(
  }

  protected async override Task OnAfterRenderAsync(bool firstRender)
  {
   Util.Log(nameof(CodeBehindPartielleKlasse) + ".OnAfterRenderAsync(firstRender=" + firstRender + ")");
  }

  public void Dispose()
  {
   Util.Log(nameof(CodeBehindPartielleKlasse) + ".Dispose()");
  }
  #endregion

  #region Reaktionen auf Benutzerinteraktionen
  public async Task AddAsync()
  {
   this.variable2 = 2;
   //this.variable1 = 1;



  Sum = X + Y;
   Util.Log($"{nameof(CodeBehindPartielleKlasse)}.Add(). x={X} y={Y} sum={Sum}");
   await ValueHasChanged.InvokeAsync(Sum);
   X = Sum;
   await Util.SetTitle(Sum.ToString());
   LogURL(); // Aufruf von Code in der .razor-Datei -> nur exemplarisch!
  }
  #endregion
 }
}