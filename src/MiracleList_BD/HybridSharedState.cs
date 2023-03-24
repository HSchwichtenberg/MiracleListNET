using System;
using System.Globalization;
using System.Threading;
using Microsoft.AspNetCore.Components.WebView.Wpf;

namespace BD;

/// <summary>
/// Shared State zwischen WPF und Blazor
/// feuert Event AppStateChanged; INotifyPropertyChanged möglich!
/// </summary>
public class HybridSharedState
{
 // DEMO: BD-06: AppState
 public BlazorWebView HostControl { get; set; }

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
 public event EventHandler<string> HybridSharedStateChanged;

 public void SetCulture(string c = "en-US")
 {
  var culture = new CultureInfo(c);
  CultureInfo.DefaultThreadCurrentCulture = culture;
  CultureInfo.DefaultThreadCurrentUICulture = culture;
  CultureInfo.CurrentCulture = culture;
  CultureInfo.CurrentUICulture = culture;
  Thread.CurrentThread.CurrentCulture = culture;
  Thread.CurrentThread.CurrentUICulture = culture;
 }
}