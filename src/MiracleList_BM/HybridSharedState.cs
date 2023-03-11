using System;
using System.Globalization;
using System.Threading;
using Microsoft.AspNetCore.Components.WebView.Maui;

namespace BM;

/// <summary>
/// Shared State zwischen WPF und Blazor
/// </summary>
public class HybridSharedState
{
 // DEMO: BD-03 AppState
 public Object HostControl { get; set; }

 private string location = null;
 /// <summary>
 /// Aktueller Benutzer in der Blazor-Anwendung
 /// </summary>
 public string Location
 {
  get { return this.location; }
  set
  {
   location = value;
   HybridSharedStateChanged?.Invoke(this, value);
  }
 }

 /// <summary>
 /// Ereignis zur Benachrichtigung bei allen Änderungen
 /// </summary>
 public event EventHandler<object> HybridSharedStateChanged;

 public void SetCulture()
 {
  var culture = new CultureInfo("en-US");
  CultureInfo.DefaultThreadCurrentCulture = culture;
  CultureInfo.DefaultThreadCurrentUICulture = culture;
  CultureInfo.CurrentCulture = culture;
  CultureInfo.CurrentUICulture = culture;
  Thread.CurrentThread.CurrentCulture = culture;
  Thread.CurrentThread.CurrentUICulture = culture;
 }
}