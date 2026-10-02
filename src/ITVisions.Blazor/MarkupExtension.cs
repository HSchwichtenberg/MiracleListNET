namespace ITVisions.Blazor;

using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;

public static class MarkupStringExtensions
{

 /// <summary>
 /// HtmlEncoder.Default.Encode + Ersetzen von Umbrüchen durch <br>
 /// </summary>
 /// <param name="text"></param>
 /// <returns></returns>
 public static MarkupString ToMarkupWithBreaks(this string? text)
 {
  if (text.IsEmpty) return new MarkupString("");
  var encoded = HtmlEncoder.Default.Encode(text ?? "");

  var html = encoded
      .Replace("\r\n", "\n")
      .Replace("\r", "\n")
      .Replace("\n", "<br>");

  return new MarkupString(html);
 }
}