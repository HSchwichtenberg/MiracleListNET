using Microsoft.Web.WebView2.Core;

public static class WebView2Helper
{
 /// <summary>
 /// Prüft, ob die Microsoft Edge WebView2 Runtime installiert ist.
 /// </summary>
 /// <param name="version">Gibt die gefundene Versionsnummer zurück (oder null, wenn nicht installiert).</param>
 /// <returns>true, wenn installiert; andernfalls false.</returns>
 public static bool IsWebView2Installed(out string? version)
 {
  version = null;

  try
  {
   version = CoreWebView2Environment.GetAvailableBrowserVersionString();
   return !string.IsNullOrEmpty(version);
  }
  catch (WebView2RuntimeNotFoundException ex)
  {
  System.Diagnostics.Debug.WriteLine("WebView2 Runtime nicht gefunden: " + ex.Message);
   return false;
  }
  catch
  {
   // Falls z. B. eine andere Exception auftritt (z. B. Zugriffsfehler)
   return false;
  }
 }
}
