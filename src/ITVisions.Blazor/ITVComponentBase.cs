using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ITVisions.Blazor;

public class TTTComponentBase : ComponentBase
{

 [Inject]
 IJSRuntime JS { get; set; }

 bool LogFailException = false;

 public enum LogType
 {
  info, warn, error
 }

 /// <summary>
 /// Ausgabe in Browser-Konsole. Funktioniert auch in Blazor Server (aber nicht bei Static SSR!)
 /// </summary>
 public async Task Log(object obj, LogType typ = LogType.info)
 {
  var s = obj.ToString();
  System.Diagnostics.Trace.WriteLine(s);
  var methodToCall = "console." + typ.ToString();
  try
  {
   await JS.InvokeVoidAsync(methodToCall, s);
  }
  catch (Exception ex)
  {
   if (LogFailException) throw new ApplicationException($"Error at TTTComponentBase.Log({s})", ex);
  }
 }

 /// <summary>
 /// Neu-Render erzwingen, funktioniert auch in Blazor WebAssembly
 /// </summary>
 public async Task UIUpdate()
 {
  await Log("UIUpdate");
  await InvokeAsync(StateHasChanged);
  await Task.Delay(1);
 }

 public bool AllowRendering = true;

 protected override bool ShouldRender()
 {
  return this.AllowRendering;
 }
}
