#pragma warning disable 1998
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITVisions.Blazor
{


 /// <summary>
 /// Hilfsklasse für Blazor WebAssembly und Blazor Server
 /// </summary>
 public partial class BlazorUtil
 {


  public async ValueTask Alert(string text)
  {
   if (_jsRuntime == null) return;
   Log("Alert");
   await _jsRuntime.InvokeVoidAsync("ShowAlert", text.Trim());
  }

  /// <summary>
  /// Zeige einen Browser-Dialog "confirm"
  /// </summary>
  /// <param name="text1">Zeile 1</param>
  /// <param name="text2">Zeile 2</param>
  /// <returns></returns>
  public async ValueTask<bool> Confirm(string text1, string text2 = "")
  {
   // DEMO: 10. JS-Interop einfach (CS)
   if (_jsRuntime == null) return false;
   Log("Confirm");
   return await _jsRuntime.InvokeAsync<bool>("confirm", text1 + 
    (!string.IsNullOrEmpty(text2) ? "\n"  + text2 : ""));
  }

  public async ValueTask ConfirmDialog(string text, int id, object objReferenceForCallback, Func<int, bool, Task> Callback)
  {
   // DEMO: 11. JS-Interop komplex mit JS Isolation und Callback (CS)
   // SkriptDatei laden
   IJSObjectReference skript = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/_content/ITVisions.Blazor/Controls/ConfirmDialog.razor.js");
   // Bootstrap-Bestätigungsdialog starten
   await skript.InvokeVoidAsync("confirmBootstrap", objReferenceForCallback, Callback.Method.Name, id, text, true);
  }

 }
}
