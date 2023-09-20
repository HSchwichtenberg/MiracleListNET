using Microsoft.AspNetCore.Components;
using System;

namespace Samples.TippsUndTricks
{
 /// <summary>
 /// Mögliche Bootstrap-Alert-CSS-Klassen
 /// </summary>
 public enum AlertType
 {
  success, info, warning, danger
 }

 public partial class Alert
 {
  [Parameter]
  public AlertType AlertType { get; set; } = AlertType.info;

  private bool visible;
  [Parameter]
#pragma warning disable BL0007 // Component parameters should be auto properties
  public bool Visible
#pragma warning restore BL0007 // Component parameters should be auto properties
  {
   get => this.Visible;
   set
   {
    if (value != this.visible)
    {
     this.visible = value;
     VisibleChanged.InvokeAsync(this.visible);
    }
   }
  }

  [Parameter]
  public EventCallback<bool> VisibleChanged { get; set; }

  [Parameter]
  public RenderFragment ChildContent { get; set; }

  public void Close()
  {
   Visible = false;
  }
 }
}