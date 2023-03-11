namespace Web.Vorlagen
{
 using ITVisions.Blazor;
 using Microsoft.AspNetCore.Components;
 using Microsoft.JSInterop;
 using System.Threading.Tasks;

 [Microsoft.AspNetCore.Components.RouteAttribute("/Samples/CodeOnly2")]
 public class CodeOnlyComponentNonVisual : Microsoft.AspNetCore.Components.ComponentBase
 {
  [Inject]
  public BlazorUtil Util { get; set; } = null;

  [Parameter]
  public decimal X { get; set; } = 0m;
  [Parameter]
  public decimal Y { get; set; } = 0m;

  [Parameter]
  public EventCallback<decimal> NewValue { get; set; }

  protected async override Task OnInitializedAsync()
  {
   Util.Log("CodeOnlyComponentNonVisual.OnInitializedAsync()");
  }

  decimal lastSum;
  protected override void OnParametersSet()
  {
   decimal sum = X + Y;

   Util.Log($"{nameof(CodeOnlyComponentNonVisual)}.Add(). x={X} y={Y} sum={sum}");
   if (lastSum != sum) { lastSum = sum; NewValue.InvokeAsync(sum); }
  }

  protected override void OnAfterRender(bool firstRender)
  {
   Util.Log("OnAfterRender(firstRender=" + firstRender + ")");
   // this.StateHasChanged(); // --> Endlosschleife !!! :-(
  }

  protected async override Task OnAfterRenderAsync(bool firstRender)
  {
   Util.Log("CodeOnlyComponentNonVisual.OnAfterRenderAsync(firstRender=" + firstRender + ")");
  }

  public void Dispose()
  {
   Util.Log("CodeOnlyComponentNonVisual.Dispose()");
  }

 }
}