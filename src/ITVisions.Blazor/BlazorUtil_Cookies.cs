using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITVisions.Blazor
{
 partial class BlazorUtil
 {

  public async Task<SortedDictionary<string, string>> GetCookies()
  {
   var r = new SortedDictionary<string, string>();
   var s = await GetCookiesAsString();
   if (String.IsNullOrEmpty(s)) return r;
   var cookies = s.Split(';');
   foreach (var cookie in cookies)
   {
    var cookieNameWert = cookie.Split('=');
    r.Add(cookieNameWert[0].Trim(), cookieNameWert[1].Trim());
   }
   return r;
  }

  public async Task<string> GetCookie(string name)
  {
   var dic = await GetCookies();
   if (!dic.ContainsKey(name)) return null;
   return dic[name];
  }

  public async Task<string> GetCookiesAsString()
  {
   if (_jsRuntime == null) return null;
   var s = await _jsRuntime.InvokeAsync<string>("getCookie");
   return s;
  }

  public async Task SetCookieDetails(string name, object value, int days = 1)
  {
   await _jsRuntime.InvokeVoidAsync("setCookieDetails", name, value, days);
  }

  public async Task<bool> SetCookie(string name, object value, bool endless = false)
  {
   if (_jsRuntime == null) return false;
   var cookie = name + "=" + value + @";path=/";
   if (endless) cookie += "; expires = Fri, 31 Dec 9999 23:59:59 GMT";
   return await _jsRuntime.InvokeAsync<bool>("setCookie", cookie);
  }

  public async Task<bool> RemoveCookie(string name)
  {
   if (_jsRuntime == null) return false;
   var cookie = name + "=" + "" + @";path=/";
   cookie += "; expires = Fri, 31 Dec 1970 23:59:59 GMT";
   return await _jsRuntime.InvokeAsync<bool>("setCookie", cookie);
  }


 }
}
