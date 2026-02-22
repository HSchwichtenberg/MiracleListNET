using System;
using System.Linq;
using ITVisions;

namespace ITVisions.Blazor.Controls;

public static class TextTemplateFormParser
{
 public static FormFieldList Parse(string template)
 {
  var formFields = new FormFieldList();

  if (string.IsNullOrEmpty(template))
  {
   return formFields;
  }

  var lines = template.Split('\n', StringSplitOptions.TrimEntries);
  string currentSection = null;
  int fieldCounter = 0;

  foreach (var line in lines)
  {
   if (string.IsNullOrWhiteSpace(line)) continue;

   // Headline mit #
   if (line.StartsWith("#"))
   {
    var headlineText = line.TrimStart('#').Trim();
    currentSection = headlineText;
    formFields.Add(new FormField
    {
     Key = $"headline_{fieldCounter++}",
     Label = headlineText,
     Type = FieldType.Headline
    });
    continue;
   }

   // Zeile mit Feld
   if (line.StartsWith("-"))
   {
    var fieldLine = line.TrimStart('-').Trim();
    var parts = fieldLine.Split(':', 2);

    if (parts.Length == 2)
    {
     var label = parts[0].Trim();
     var fieldDef = parts[1].Trim();
     var key = $"{currentSection}_{label}".Replace(" ", "_").Replace(":", "");
     fieldCounter++;

     // Radio-Buttons erkennen
     if (fieldDef.Contains("O "))
     {
      // Entferne führende/nachfolgende Whitespaces und splitte bei "O "
      var optionParts = fieldDef.Split(new[] { "O " }, StringSplitOptions.RemoveEmptyEntries);
      var options = optionParts
       .Select(o => o.Trim())
       .Where(o => !string.IsNullOrWhiteSpace(o))
       .ToList();

      formFields.Add(new FormField
      {
       Key = key,
       Label = $"{currentSection}: {label}",
       Type = FieldType.RadioButtons,
       Options = options
      });
     }
     // Textarea
     else if (fieldDef.Contains("[Textarea]"))
     {
      formFields.Add(new FormField
      {
       Key = key,
       Label = $"{currentSection}: {label}",
       Type = FieldType.TextArea
      });
     }
     // Textfeld
     else if (fieldDef.Contains("___") || fieldDef.Contains("[Text]"))
     {
      formFields.Add(new FormField
      {
       Key = key,
       Label = $"{currentSection}: {label}",
       Type = FieldType.Text
      });
     }
    }
   }
   // Standalone Felder (ohne -)
   else if (line.Contains("___") || line.Contains("[Text]"))
   {
    var parts = line.Split(new[] { "___", "[Text]" }, StringSplitOptions.None);
    var label = parts[0].TrimEnd(':').Trim();
    var key = $"field_{fieldCounter++}_{label.Replace(" ", "_")}";

    formFields.Add(new FormField
    {
     Key = key,
     Label = label,
     Type = FieldType.Text
    });
   }
   else if (line.Contains("[Textarea]"))
   {
    var label = line.Replace("[Textarea]", "").TrimEnd(':').Trim();
    var key = $"field_{fieldCounter++}_{label.Replace(" ", "_")}";

    formFields.Add(new FormField
    {
     Key = key,
     Label = label,
     Type = FieldType.TextArea
    });
   }
  }

  return formFields;
 }
}
