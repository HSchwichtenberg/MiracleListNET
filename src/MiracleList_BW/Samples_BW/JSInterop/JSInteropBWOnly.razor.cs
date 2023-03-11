using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Web.Samples_BW.JSInterop;

[SupportedOSPlatform("browser")]
partial class JSInteropBWOnly
{

 IJSObjectReference script;
 public string NETOutput { get; set; }

 async Task InteropForAllBlazorVariants()
 {
  //if (script == null) script = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./Pages/JSInteropBWOnly.razor.js");
  if (script == null) script = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./JSInteropBWOnly.js");

  NETOutput = await script.InvokeAsync<string>("getMessage", "Holger");
  this.StateHasChanged();

  await script.InvokeAsync<string>("setMessageNew");
 }

 [JSImport("getMessage", "./JSInteropBWOnly.js")]
 internal static partial string GetMessageFromJS(string name);

 [JSImport("setMessageNew", "./JSInteropBWOnly.js")]
 internal static partial string SetMessageNew();

 [JSInvokable()]
 public static string GetMessageFromDotnetClassic(string name)
 {
  return "Hallo " + name + ", .NET grüßt Dich!";
 }

 [JSExport] // nur mit static!
 internal static string GetMessageFromDotnetNew(string name)
 {
  return "Hallo " + name + ", .NET grüßt Dich!";
 }


 [SupportedOSPlatform("browser")]
 async Task InteropBWOnly()
 {
  // ohne diese Zeile: module was not imported yet, please call JSHost.Import() first.
  await JSHost.ImportAsync("JSInteropBWOnly", "./JSInteropBWOnly.js");
  NETOutput = GetMessageFromJS("Holger");

  SetMessageNew();
 }
}