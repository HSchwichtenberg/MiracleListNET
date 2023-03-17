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

  string AlertDanger(string text, string title)
  {
   return Alert("danger", "oi-medical-cross", text, title);
  }

  string AlertRunning(string text, string title)
  {
   return Alert("secondary", "oi-question-mark", text, title);
  }


  string AlertSuccess(string text, string title)
  {
   return Alert("success", "oi-circle-check", text, title);
  }

  string Alert(string type, string icon, string text, string title)
  {
   return "<div class='alert alert-" + type + "' title='" + title + "' data-toggle='tooltip' data-placement='bottom'><span class='oi " + icon + "'   aria-hidden='true'></span>" + text + "</div>";
  }
 }
}