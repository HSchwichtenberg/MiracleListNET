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

  /// <summary>
  /// Setzt den Titel des Browserfensters via JavaScript
  /// </summary>
  /// <param name="text"></param>
  /// <returns></returns>
  public async Task SetTitle(string text)
  {
   if (_jsRuntime == null) return;
   //Log("SetTitle " + text);
   await _jsRuntime.InvokeVoidAsync("SetTitle", text);
  }

 }
}