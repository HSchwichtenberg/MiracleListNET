#pragma warning disable 1998
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ITVisions.Blazor
{

 /// <summary>
 /// Utility Class for Blazor 
 /// </summary>
 public partial class BlazorUtil
 {
  IJSRuntime _jsRuntime { get; set; }
  private NavigationManager NavigationManager { get; set; } = null;
  private IHttpContextAccessor httpContextAccessor { get; set; } = null;

  // DI
  public BlazorUtil(IJSRuntime jsRuntime, NavigationManager NavigationManager)
  {
   _jsRuntime = jsRuntime;
   this.NavigationManager = NavigationManager;
   //TODO: this.httpContextAccessor = httpContextAccessor;
  }

  #region Blazor Type
  public bool IsWebAssembly => BlazorType is "WebAssembly";
  public bool IsBlazorServer => BlazorType is "Server";
  public bool IsHybrid => BlazorType is "Hybrid";

  public string BlazorType =>
       _jsRuntime.GetType().FullName switch
       {
        "Microsoft.AspNetCore.Components.Server.Circuits.RemoteJSRuntime" => "Server",
        "Microsoft.AspNetCore.Components.WebAssembly.Services.DefaultWebAssemblyJSRuntime" => "WebAssembly",
        "Microsoft.AspNetCore.Components.WebView.Services.WebViewJSRuntime" => "Hybrid",
        _ => "unbekannt"
       };

  public string GetBlazorVersionInfo()
  {
   string blazorVersion = "";
   if (_jsRuntime != null)
   {
    if (!IsWebAssembly)
    {
     try
     {
      blazorVersion = FileVersionInfo.GetVersionInfo(_jsRuntime.GetType().Assembly.Location).ProductVersion;
     }
     catch (Exception)
     {
      blazorVersion = "unknown";
     }
    }
    else
    {
     blazorVersion = _jsRuntime.GetType().Assembly.GetName().Version.ToString();
    }
   }

   // Version of Runtime (.NET Core, Mono, .NET >=5)
   string runtime = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;

   return "Blazor " + BlazorType + (!String.IsNullOrEmpty(blazorVersion) ? " v" + blazorVersion : "") + " @ " + runtime;
  }

  public string GetASPNETVersion()
  {
   var ass = System.Reflection.Assembly.GetAssembly(typeof(Microsoft.AspNetCore.Components.IComponent));
   var vers = ass.GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false);
   if (vers.Length == 0) return "n/a";
   return ((System.Reflection.AssemblyInformationalVersionAttribute)vers[0]).InformationalVersion;
  }
  #endregion

 }
}