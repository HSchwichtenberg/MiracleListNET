namespace ITVisions.Blazor;

using System.Text.Encodings.Web;
using Ganss.Xss;
using Microsoft.AspNetCore.Components;

public static class MarkupStringExtensions
{
 // HtmlSanitizer ist laut Dokumentation threadsicher und kann daher einmalig wiederverwendet werden.
 // class und data-*-Attribute sind in der Standardkonfiguration gesperrt, werden aber z.B. für
 // Bootstrap-Badges/-Tooltips (class, data-toggle, data-placement) benötigt und daher explizit erlaubt.
 private static readonly HtmlSanitizer Sanitizer = CreateSanitizer();

 private static HtmlSanitizer CreateSanitizer()
 {
  var sanitizer = new HtmlSanitizer();
  sanitizer.AllowedAttributes.Add("class");
  sanitizer.AllowDataAttributes = true;
  return sanitizer;
 }

 /// <summary>
 /// Entfernt per HtmlSanitizer nur gefährliches HTML (z.B. &lt;script&gt;, on*-Event-Attribute, javascript:-URLs),
 /// erlaubtes HTML (z.B. &lt;b&gt;, &lt;i&gt;, &lt;a&gt;) bleibt erhalten.
 /// Zusätzlich werden Zeilenumbrüche durch &lt;br&gt; ersetzt.
 /// </summary>
 /// <param name="text"></param>
 /// <returns></returns>
 public static MarkupString ToMarkupWithBreaks(this string? text)
 {
  if (text.IsEmpty) return new MarkupString("");

  var sanitized = Sanitizer.Sanitize(text ?? "");

  var html = sanitized
      .Replace("\r\n", "\n")
      .Replace("\r", "\n")
      .Replace("\n", "<br>");

  return new MarkupString(html);
 }
}
