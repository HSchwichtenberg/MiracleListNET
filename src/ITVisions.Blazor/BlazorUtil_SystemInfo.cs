using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace ITVisions.Blazor
{
 partial class BlazorUtil
 {
  public Version GetAppVersion()
  {
   return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
  }

  public string GetAppVersionShort()
  {
   return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major + "." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor;
  }

  public async ValueTask<bool> IsHttps()
  {
   if (_jsRuntime == null) return false;
   return await _jsRuntime.InvokeAsync<bool>("IsHttps", null);
  }

  public async ValueTask<bool> InitUpdateScreenSize(string id = "screenInfo")
  {
   if (_jsRuntime == null) return false;
   await _jsRuntime.InvokeAsync<string>("window.initUpdateScreenSize", id);
   return true;
  }

  public async ValueTask<string> GetScreenSize()
  {
   if (_jsRuntime == null) return "n/a";
   return await _jsRuntime.InvokeAsync<string>("window.getScreenSize", null);
  }

  public async Task<string> GetBrowserShortInfo()
  {
   var data = await _jsRuntime.InvokeAsync<string[]>("Util.getBrowserInfo", null);
   var uaParser = UAParser.Parser.GetDefault();
   if (data == null) return "?";
   var ua = uaParser.Parse(data[1]);

   return ua.UA + " @ " + ua.OS + " (Device: " + ua.Device + ")";
  }

  //public async Task<string> getBrowserShortInfo()
  //{
  // if (_jsRuntime == null) return "";
  // var b = await _jsRuntime.InvokeAsync<string>("Util.getBrowserShortInfo", null);
  // Log("getBrowserShortInfo: " + b);
  // return b;
  //}

  public async Task<string> GetBrowserInfo()
  {
   var data = await _jsRuntime.InvokeAsync<string[]>("Util.getBrowserInfo", null);
   if (data == null) return "?";
   return data[0] + " / " + data[1];
   //try
   //{
   // return httpContextAccessor.HttpContext.Request.Headers["User-Agent"];
   //}
   //catch (Exception)
   //{

   //}
  }

  public bool IsInternetExplorer()
  {
   if (httpContextAccessor == null) return false;
   return GetBrowserInfo().Result.ToUpper().Contains("MSIE");
  }

  /// <summary>
  /// IP des Aufrufers
  /// </summary>
  public string GetClientIP()
  {
   if (httpContextAccessor == null || httpContextAccessor.HttpContext == null) return "n/a";
   try
   {
    var s = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

    // Falls hinter einem Proxy (X-Forwarded-For Header prüfen)
    if (httpContextAccessor.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For") == true)
    {
     s = httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim();
    }

    if (String.IsNullOrEmpty(s)) return "n/a";
    return s;
   }
   catch (Exception)
   {
    return "n/a";
   }
  }

  public string GetServerIP()
  {
   if (httpContextAccessor == null) return null;
   try
   {
    return httpContextAccessor.HttpContext.Features.Get<IHttpConnectionFeature>()?.LocalIpAddress.ToString();
   }
   catch (Exception)
   {
    return "n/a";
   }
  }

  public async Task<TimeSpan> GetLatency()
  {
   var startTime = DateTime.UtcNow;
   var _ = await _jsRuntime.InvokeAsync<string>("toString");
   return DateTime.UtcNow - startTime;
  }


 }
}
