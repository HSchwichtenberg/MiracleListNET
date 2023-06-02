using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using System;

namespace ITVisions.Blazor.Controls;

public class BlazorTimer : ComponentBase, IDisposable
{
 [Inject]
 public BlazorUtil Util { get; set; } = null;

  [Parameter]
  public double Seconds { get; set; }

  [Parameter]
  public EventCallback Callback { get; set; }

  System.Threading.Timer timer; // Außerhalb wegen GC!
  protected override void OnInitialized()
  {
   Util.Log("Starte Timer...(" + Seconds + " seconds)");
   timer = new System.Threading.Timer(
     callback: (_) => InvokeAsync(() => Callback.InvokeAsync(null)),
     state: null,
     dueTime: TimeSpan.FromSeconds(Seconds),
     period: TimeSpan.FromSeconds(Seconds));
  }

  public void Dispose()
  {
   // wichtig, damit Timer nicht weiterläuft, wenn die Komponente schon nicht mehr lebt
   if (timer != null) timer.Dispose();
  }
 }
}