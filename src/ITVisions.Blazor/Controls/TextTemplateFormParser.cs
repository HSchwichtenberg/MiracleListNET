using System;
using System.Collections.Generic;
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
   if (line.StartsWith("-") || line.StartsWith("+"))
   {
    var isRequired = line.StartsWith("+");
    var fieldLine = line.TrimStart('-', '+').Trim();
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

      // Wenn nur eine Option vorhanden ist, als Checkbox behandeln
      if (options.Count == 1)
      {
       formFields.Add(new FormField
       {
        Key = key,
        Label = $"{currentSection}: {label}",
        Type = FieldType.CheckBox,
        Options = options,
        Required = isRequired
       });
      }
      else
      {
       formFields.Add(new FormField
       {
        Key = key,
        Label = $"{currentSection}: {label}",
        Type = FieldType.RadioButtons,
        Options = options,
        Required = isRequired
       });
      }
     }
     // Multiselect
     else if (fieldDef.Contains("[Multiselect]"))
     {
      var optionsString = fieldDef.Replace("[Multiselect]", "").Trim();
      var options = optionsString.Split(',')
       .Select(o => o.Trim())
       .Where(o => !string.IsNullOrWhiteSpace(o))
       .ToList();

      formFields.Add(new FormField
      {
       Key = key,
       Label = $"{currentSection}: {label}",
       Type = FieldType.Multiselect,
       Options = options,
       Required = isRequired
      });
     }
     // Rating
     else if (fieldDef.Contains("[Rating]"))
     {
      var rangeString = fieldDef.Replace("[Rating]", "").Trim();
      var options = new List<string>();

      // Prüfe auf Bereich (z.B. "1-5")
      if (rangeString.Contains("-"))
      {
       var partsRating = rangeString.Split('-');
       if (partsRating.Length == 2 && int.TryParse(partsRating[0], out int start) && int.TryParse(partsRating[1], out int end))
       {
        for (int i = start; i <= end; i++)
        {
         options.Add(i.ToString());
        }
       }
      }

      formFields.Add(new FormField
      {
       Key = key,
       Label = $"{currentSection}: {label}",
       Type = FieldType.Rating,
       Options = options,
       Required = isRequired
      });
     }
     // Textarea
     else if (fieldDef.Contains("[Textarea]"))
     {
      formFields.Add(new FormField
      {
       Key = key,
       Label = $"{currentSection}: {label}",
       Type = FieldType.TextArea,
       Required = isRequired
      });
     }
     // Number
     else if (fieldDef.Contains("[Number]"))
     {
      var rangeString = fieldDef.Replace("[Number]", "").Trim();
      int? min = null;
      int? max = null;

      // Prüfe auf Bereich (z.B. "0-10")
      if (rangeString.Contains("-"))
      {
       var partsNumber = rangeString.Split('-');
       if (partsNumber.Length == 2 && int.TryParse(partsNumber[0], out int minVal) && int.TryParse(partsNumber[1], out int maxVal))
       {
        min = minVal;
        max = maxVal;
       }
      }

      formFields.Add(new FormField
      {
       Key = key,
       Label = $"{currentSection}: {label}",
       Type = FieldType.Number,
       Min = min,
       Max = max,
       Value = min,
       Required = isRequired
      });
     }
     // Date
     else if (fieldDef.Contains("[Date]"))
     {
      formFields.Add(new FormField
      {
       Key = key,
       Label = $"{currentSection}: {label}",
       Type = FieldType.Date,
       Required = isRequired
      });
     }
     // Email
     else if (fieldDef.Contains("[Email]"))
     {
      formFields.Add(new FormField
      {
       Key = key,
       Label = $"{currentSection}: {label}",
       Type = FieldType.Email,
       Required = isRequired
      });
     }
     // Phone
     else if (fieldDef.Contains("[Phone]"))
     {
      formFields.Add(new FormField
      {
       Key = key,
       Label = $"{currentSection}: {label}",
       Type = FieldType.Phone,
       Required = isRequired
      });
     }
     // Textfeld
     else if (fieldDef.Contains("___") || fieldDef.Contains("[Text]"))
     {
      formFields.Add(new FormField
      {
       Key = key,
       Label = $"{currentSection}: {label}",
       Type = FieldType.Text,
       Required = isRequired
      });
     }
     // Select (kommagetrennte Optionen ohne spezielle Marker)
     else if (fieldDef.Contains(","))
     {
      var options = fieldDef.Split(',')
       .Select(o => o.Trim())
       .Where(o => !string.IsNullOrWhiteSpace(o))
       .ToList();

      formFields.Add(new FormField
      {
       Key = key,
       Label = $"{currentSection}: {label}",
       Type = FieldType.Select,
       Options = options,
       Required = isRequired
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
   else if (line.Contains("[Number]"))
   {
    var tempLine = line.Replace("[Number]", "|||[Number]|||");
    var parts = tempLine.Split(new[] { "|||[Number]|||" }, StringSplitOptions.None);
    var label = parts[0].TrimEnd(':').Trim();
    var rangeString = parts.Length > 1 ? parts[1].Trim() : "";
    var key = $"field_{fieldCounter++}_{label.Replace(" ", "_")}";

    int? min = null;
    int? max = null;

    // Prüfe auf Bereich (z.B. "0-10")
    if (rangeString.Contains("-"))
    {
     var rangeParts = rangeString.Split('-');
     if (rangeParts.Length == 2 && int.TryParse(rangeParts[0], out int minVal) && int.TryParse(rangeParts[1], out int maxVal))
     {
      min = minVal;
      max = maxVal;
     }
    }

    formFields.Add(new FormField
    {
     Key = key,
     Label = label,
     Type = FieldType.Number,
     Min = min,
     Max = max,
     Value = min
    });
   }
   else if (line.Contains("[Date]"))
   {
    var label = line.Replace("[Date]", "").TrimEnd(':').Trim();
    var key = $"field_{fieldCounter++}_{label.Replace(" ", "_")}";

    formFields.Add(new FormField
    {
     Key = key,
     Label = label,
     Type = FieldType.Date
    });
   }
   else if (line.Contains("[Email]"))
   {
    var label = line.Replace("[Email]", "").TrimEnd(':').Trim();
    var key = $"field_{fieldCounter++}_{label.Replace(" ", "_")}";

    formFields.Add(new FormField
    {
     Key = key,
     Label = label,
     Type = FieldType.Email
    });
   }
   else if (line.Contains("[Phone]"))
   {
    var label = line.Replace("[Phone]", "").TrimEnd(':').Trim();
    var key = $"field_{fieldCounter++}_{label.Replace(" ", "_")}";

    formFields.Add(new FormField
    {
     Key = key,
     Label = label,
     Type = FieldType.Phone
    });
   }
  }

  return formFields;
 }
}
