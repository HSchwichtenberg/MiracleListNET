#pragma warning disable 1998
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace ITVisions.Blazor
{

 public enum LogType
 {
  info, warn, error
 }

 /// <summary>
 /// Utility Class for Blazor WebAssembly and Blazor Server
 /// </summary>
 public partial class BlazorUtil
 {
  #region Browser Console
  /// <summary>
  /// Optional prefix for each console output
  /// </summary>
  public string LogPrefix { get; set; } = "BLAZOR: ";
  public bool LogFailOnError { get; set; } = false;

  public async void Log(string s, params object[] objects)
  {
   if (objects.Length > 0)
   {
    s = s + "\n";
    foreach (var o in objects)
    {
     s = s + "\n" + ObjectToString(o);
    }
   }

   Log(s);
  }



  public async void Log(object o)
  {
   Log(ObjectToString(o));
  }

  public async void Warn(string s)
  {
   Log(s, LogType.warn);
  }

  public async void Warn(string s, object o = null)
  {
   if (o != null) s = s + ": " + ObjectToString(o);
   Log(s, LogType.warn);
  }

  public async void Error(string s, object o = null)
  {
   if (o != null) s = s + ": " + ObjectToString(o);
   Log(s, LogType.error);
  }

  private static string ObjectToString(object o)
  {
   if (o is null) return "(null)";
   // uses https://github.com/HSchwichtenberg/ObjectDumper
   try
   {
    return ObjectDumper.Dump(o, new DumpOptions() { MaxLevel = 1 });
   }
   catch (Exception)
   { // Workaround
    try
    {
     var j = JsonSerializer.Serialize(o);
     return j;
    }
    catch (Exception ex)
    {
     return "(ObjectToString failed: " + ex.Message + ")";
    }
   }
  }

  public async void Clear()
  {
   await _jsRuntime.InvokeVoidAsync("console.clear");
  }

  public async void Log(string s, LogType typ = LogType.info)
  {
   s = LogPrefix + s;
   var methodToCall = "console." + typ.ToString();
   try
   {
    await _jsRuntime.InvokeVoidAsync(methodToCall, s);
   }
   catch (Exception ex)
   {
    if (LogFailOnError) throw new ApplicationException("Error while logging: " + s, ex);
   }
  }
 }
 #endregion

}