using System;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;

namespace Web.Samples_BW.JSInterop;

[SupportedOSPlatform("browser")]
partial class JSInteropBWOnly
{

 IJSObjectReference script;
 public string NETOutput { get; set; }

 async Task InteropForAllBlazorVariants()
 {
  // ohne diese Zeile: module was not imported yet, please call JSHost.Import() first.
  // JSHost.ImportAsync lädt Dateien immer relativ zum wwwroot/_framework-Verzeichnis,
  // daher "../" und nicht "./", da JSInteropBWOnly.js direkt in wwwroot liegt.
  if (script == null) script = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "../JSInteropBWOnly.js");
  Console.WriteLine(script);
  NETOutput = await script.InvokeAsync<string>("getMessage", "Holger");
  await InvokeAsync(StateHasChanged);

  await script.InvokeAsync<string>("setMessage");
 }

 // Der Modulname hier muss exakt dem bei JSHost.ImportAsync(...) übergebenen Namen entsprechen.
 [JSImport("getMessage", "JSInteropBWOnly")]
 internal static partial string GetMessageFromJS(string name);

 [JSImport("setMessage", "JSInteropBWOnly")]
 internal static partial Task SetMessage();

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
  // JSHost.ImportAsync lädt Dateien immer relativ zum wwwroot/_framework-Verzeichnis,
  // daher "../" und nicht "./", da JSInteropBWOnly.js direkt in wwwroot liegt.
  await JSHost.ImportAsync("JSInteropBWOnly", "../JSInteropBWOnly.js");
  NETOutput = GetMessageFromJS("Holger");

  await SetMessage();
 }
}