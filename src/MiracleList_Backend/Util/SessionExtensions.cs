using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ITVisions.AspNetCore
{
 public static class SessionExtensions
 {
  /// <summary>
  /// Speichert ein Objekt in einer Session-Variable (JSON-Format)
  /// </summary>
  public static void SetObject(this ISession session, string key, object value)
  {
   var config = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, PreserveReferencesHandling = PreserveReferencesHandling.None };
   session.SetString(key, JsonConvert.SerializeObject(value, Formatting.None, config));
  }

  /// <summary>
  /// Lädt ein Objekt aus einer Session-Variable (JSON-Format)
  /// </summary>
  public static T GetObject<T>(this ISession session, string key)
  {
   var value = session.GetString(key);
   return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
  }
 }
}
