namespace Web.Vorlagen
{
 using ITVisions.Blazor;
 using Microsoft.AspNetCore.Components;
 using Microsoft.JSInterop;
 using System.Threading.Tasks;

 [Microsoft.AspNetCore.Components.RouteAttribute("/Samples/CodeOnly")]
 public class CodeOnlyComponent : Microsoft.AspNetCore.Components.ComponentBase
 {
  [Inject]
  public BlazorUtil Util { get; set; } = null;
  [Inject]
  public IJSRuntime JSRuntime { get; set; } = null;
  [Inject]
  public NavigationManager NavigationManager { get; set; } = null;

  // Achtung: Dies nur für BS! Kein HttpContext in BW!
  //[Inject]
  //Microsoft.AspNetCore.Http.IHttpContextAccessor HttpContextAccessor { get; set; } = null;

  [Parameter]
  public decimal X { get; set; } = 1.23m;
  [Parameter]
  public decimal Y { get; set; } = 2.34m;

  public decimal Sum = 0;

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
   Util.Log($"{nameof(CodeOnlyComponent)}.Add(). x={X} y={Y} sum={Sum}");
   await Util.SetTitle(Sum.ToString());
  }
  #endregion

  #region UI-Rendering
  protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
  {
   __builder.AddMarkupContent(0, "<h2>Code-Only-Component</h2>\r\nX:\r\n");
   __builder.OpenElement(1, "input");
   __builder.AddAttribute(2, "type", "number");
   __builder.AddAttribute(3, "value", BindConverter.FormatValue(X, culture: global::System.Globalization.CultureInfo.InvariantCulture));
   __builder.AddAttribute(4, "onchange", EventCallback.Factory.CreateBinder(this, __value => X = __value, X, culture: global::System.Globalization.CultureInfo.InvariantCulture));
   __builder.SetUpdatesAttributeName("value");
   __builder.CloseElement();
   __builder.AddMarkupContent(5, "\r\nY:\r\n");
   __builder.OpenElement(6, "input");
   __builder.AddAttribute(7, "type", "number");
   __builder.AddAttribute(8, "value", BindConverter.FormatValue(Y, culture: global::System.Globalization.CultureInfo.InvariantCulture));
   __builder.AddAttribute(9, "onchange", EventCallback.Factory.CreateBinder(this, __value => Y = __value, Y, culture: global::System.Globalization.CultureInfo.InvariantCulture));
   __builder.SetUpdatesAttributeName("value");
   __builder.CloseElement();
   __builder.AddMarkupContent(10, "\r\n");
   __builder.OpenElement(11, "button");
   __builder.AddAttribute(12, "onclick", EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this,
                  AddAsync
            ));
   __builder.AddContent(13, "Add x + y");
   __builder.CloseElement();
   __builder.AddMarkupContent(14, "\r\nSum: ");
   __builder.AddContent(15, Sum);
  }
  #endregion
 }
}