#pragma warning disable 1998

using Blazored.LocalStorage;
using Blazored.SessionStorage;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace ITVisions.Blazor
{
 /// <summary>
 /// Hilfsklasse für Blazor
 /// </summary>
 public partial class JSUtil
 {
  IJSRuntime _jsRuntime { get; set; }
  private NavigationManager NavigationManager { get; set; } = null;
  private IHttpContextAccessor httpContextAccessor { get; set; } = null;
  private BlazorUtil Util { get; set; } = null;
  private ISessionStorageService LocalStorageService { get; set; } = null;
  // DI
  public JSUtil(IJSRuntime jsRuntime, NavigationManager NavigationManager, IHttpContextAccessor httpContextAccessor, BlazorUtil Util, ISessionStorageService LocalStorageService)
  {
   _jsRuntime = jsRuntime;
   this.NavigationManager = NavigationManager;
   this.httpContextAccessor = httpContextAccessor;
   this.Util = Util;
   this.LocalStorageService = LocalStorageService;
  }

  public async Task<bool> Notification(string head, string text)
  {
   //Util.Log("Notification.cs", head + "/" + text);
   if (_jsRuntime == null) return false;
   return await _jsRuntime.InvokeAsync<bool>("showNotification", head, text);
  }

  public Guid? ConvertToGuid(string guidString)
  {
   if (guidString != null && Guid.TryParse(guidString, out Guid guid)) return guid;
   return null;
  }

  public static Boolean TryParse<T>(String source, out T value)
  {
   TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
   try
   {
    value = (T)converter.ConvertFromString(source);
    return true;
   }
   catch
   {
    value = default(T);
    return false;
   }
  }

  /// <summary>
  /// Lädt Wert aus Cookie oder local Storage
  /// </summary>
  public async Task<T> GetValueFromBrowser<T>(string key, T defaultValue = default(T))
  {
   //try
   //{
   // var wertString = await Util.GetCookie(key);

   // if (!String.IsNullOrEmpty(wertString))
   // {
   //  Util.Log(key + " from Cookie: " + wertString);

   //  if (TryParse<T>(wertString, out T wert)) return wert;
   //  return defaultValue;
   // }
   //}
   //catch (Exception ex)
   //{
   // Util.Log("Error reading " + key + " from Cookie: " + ex.Message, LogType.warn);
   //}

   try
   {
    var guid = (await LocalStorageService.GetItemAsync<T>(key));
    if (guid != null) Util.Log(key + " from Local Storage: " + guid);
    return guid;
   }
   catch (Exception ex)
   {
    Util.Log("Error reading " + key + " from local Storage: " + ex.Message, LogType.warn);
    return defaultValue;
   }
  }

  public async Task RemoveValueInBrowser(string Key)
  {
   await SetValueInBrowser(Key, null);
  }

  public async Task<Boolean> IsCookieAllow()
  {
   const string name = "CookieCheck";
   const int testvalue = 123;
   await SetValueInBrowser(name, testvalue);
   var b = await GetValueFromBrowser<int>(name);
   return (b == testvalue);
  }


  public async Task SetValueInBrowser(string Key, object value)
  {
   try
   {
    if (value == null) await Util.RemoveCookie(Key);
    else await Util.SetCookie(Key, value);
  }
   catch (Exception ex)
   {
    Util.Log("Error writing " + Key + " to Cookie: " + ex.Message, LogType.warn);
   }

   try
   {
    if (value == null) await LocalStorageService.RemoveItemAsync(Key);
    else await LocalStorageService.SetItemAsync(Key, value);
   }
   catch (Exception ex)
   {
    Util.Log("Error writing " + Key + " to local Storage: " + ex.Message, LogType.warn);
   }
  }
 }
}