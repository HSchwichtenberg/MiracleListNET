using System;
using System.Collections.Generic;

namespace ITVisions.Blazor
{
 public class GenericSessionState
 {
  Dictionary<string, Object> daten = new Dictionary<string, object>();
  /// <summary>
  /// Indexer
  /// </summary>
  /// <param name="name">Name of object</param>
  /// <returns>object value</returns>
  public object this[string name]
  {
   get { return daten[name]; }
   set { daten[name] = value; }
  }

  public T Get<T>(string name, T defaultValue = default(T))
  {
   return (T)GetOrCreate(name, defaultValue);
  }

  public int GetInt(string name, int defaultValue = 0)
  {
   return (int)GetOrCreate(name, defaultValue);
  }

  public long GetLong(string name, long defaultValue = 0)
  {
   return (long)GetOrCreate(name, defaultValue);
  }

  public DateTime GetDateTime(string name, DateTime defaultValue = default(DateTime))
  {
   return (DateTime)GetOrCreate(name, defaultValue);
  }

  public bool GetBool(string name, bool defaultValue = false)
  {
   return (bool)GetOrCreate(name, defaultValue);
  }

  public string GetString(string name, string defaultValue = "")
  {
   return (string)GetOrCreate(name, defaultValue);
  }

  public object GetObject(string name, object defaultValue = null)
  {
   return (object)GetOrCreate(name, defaultValue);
  }

  private object GetOrCreate(string name, object defaultValue)
  {
   if (!daten.ContainsKey(name))
   {
    daten[name] = defaultValue; return defaultValue;
   }
   return (object)daten[name];
  }

 }
}