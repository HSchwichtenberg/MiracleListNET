#pragma warning disable 1998
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace ITVisions.Blazor
{
 /// <summary>
 /// Utility Class for Blazor WebAssembly and Blazor Server
 /// Desktop Notification API des Browsers
 /// </summary>
 public partial class BlazorUtil
 {
  public async Task RequestPermission(string head, string text)
  {
   Log("RequestPermission", head + "/" + text);
   if (_jsRuntime == null) return ;
   await _jsRuntime.InvokeVoidAsync("requestPermission", head, text);
  }

  public async Task<bool> Notification(string head, string text)
  {
   Log("Notification", head + "/" + text);
   if (_jsRuntime == null) return false;
   return await _jsRuntime.InvokeAsync<bool>("showNotification", head, text);
  }
 }
}