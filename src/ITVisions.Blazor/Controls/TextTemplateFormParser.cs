using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using ITVisions;

namespace ITVisions.Blazor.Controls;

public static class TextTemplateFormParser
{

 /// <summary>
 /// Parsen eines Texttemplates in eine strukturierte Liste von FormField-Objekten, die verschiedene Feldtypen und Optionen unterstützen.
 /// </summary>
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
    currentSection = ParseHeadline(line, ref fieldCounter, formFields);
    continue;
   }

   // Zeile mit Feld (- oder +)
   if (line.StartsWith("-") || line.StartsWith("+"))
   {
    ParseFieldWithPrefix(line, currentSection, ref fieldCounter, formFields);
   }
   else
   {
    // Zeilen ohne - oder + werden wie optionale Felder behandelt
    ParseFieldWithPrefix("-" + line, currentSection, ref fieldCounter, formFields);
   }
  }

  return formFields;
 }

 /// <summary>
 /// Überschrift auswerten
 /// </summary>
 private static string ParseHeadline(string line, ref int fieldCounter, FormFieldList formFields)
 {
  var headlineText = line.TrimStart('#').Trim();
  formFields.Add(new FormField
  {
   Key = $"headline_{fieldCounter++}",
   Label = headlineText,
   Type = FieldType.Headline
  });
  return headlineText;
 }

 /// <summary>
 /// Feld auswerten
 /// </summary>
 private static void ParseFieldWithPrefix(string line, string currentSection, ref int fieldCounter, FormFieldList formFields)
 {
  var isRequired = line.StartsWith("+");
  var fieldLine = line.TrimStart('-', '+').Trim();
  var parts = fieldLine.Split(':', 2);

  string label;
  string fieldDef;
  string tooltipNote = null;

  // Wenn nur Feldname ohne ":", dann als [Text] behandeln
  if (parts.Length == 1)
  {
   label = parts[0].Trim();
   fieldDef = "[Text]";
  }
  else if (parts.Length == 2)
  {
   label = parts[0].Trim();
   fieldDef = parts[1].Trim();
  }
  else
  {
   return;
  }

  // Prüfe ZUERST auf Standardwert im Label (für Felder ohne Doppelpunkt OHNE Tooltip)
  string defaultValue = null;
  if (parts.Length == 1 && label.Contains(" = "))
  {
   var defaultParts = label.Split(new[] { " = " }, StringSplitOptions.None);
   if (defaultParts.Length >= 2)
   {
    label = defaultParts[0].Trim();
    defaultValue = string.Join(" = ", defaultParts.Skip(1)).Trim().Trim('"');
   }
  }

  // Prüfe auf Tooltip-Note in geschweiften Klammern
  if (label.Contains("{") && label.Contains("}"))
  {
   var startIndex = label.IndexOf('{');
   var endIndex = label.IndexOf('}');
   if (endIndex > startIndex)
   {
    tooltipNote = label.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();
    var labelRemainder = label.Substring(endIndex + 1).Trim();
    label = label.Substring(0, startIndex).Trim();

    // Überschreibe defaultValue wenn nach Tooltip noch ein = kommt
    if (labelRemainder.Contains(" = "))
    {
     var defaultParts = labelRemainder.Split(new[] { " = " }, StringSplitOptions.None);
     if (defaultParts.Length >= 2)
     {
      defaultValue = string.Join(" = ", defaultParts.Skip(1)).Trim().Trim('"');
     }
    }
   }
  }

  // Prüfe auf Standardwert (= wert am Ende) im fieldDef
  if (fieldDef.Contains(" = "))
  {
   var defaultParts = fieldDef.Split(new[] { " = " }, StringSplitOptions.None);
   if (defaultParts.Length == 2)
   {
    fieldDef = defaultParts[0].Trim();
    defaultValue = defaultParts[1].Trim().Trim('"');
   }
  }

  // Falls Key bereits vergeben ist, hänge GUID an
  if (formFields.Any(f => f.Label == label))
  {
   label = $"{label}_!DOPPELT!_{Guid.NewGuid():N}";
  }

  var baseKey = $"{currentSection}_{label}".Replace(" ", "_").Replace(":", "");
  var key = baseKey;

  // Falls Key bereits vergeben ist, hänge GUID an
  if (formFields.Any(f => f.Key == key))
  {
   key = $"{baseKey}_{Guid.NewGuid():N}";
  }

  fieldCounter++;

  // Prüfe auf verschiedene Feldtypen
  if (fieldDef.Contains("O "))
  {
   ParseRadioButtonsOrCheckbox(key, currentSection, label, fieldDef, isRequired, defaultValue, tooltipNote, formFields);
  }
  else if (fieldDef.Contains("[Multiselect]", StringComparison.OrdinalIgnoreCase))
  {
   ParseMultiselect(key, currentSection, label, fieldDef, isRequired, defaultValue, tooltipNote, formFields);
  }
  else if (fieldDef.Contains("[Rating]", StringComparison.OrdinalIgnoreCase))
  {
   ParseRating(key, currentSection, label, fieldDef, isRequired, defaultValue, tooltipNote, formFields);
  }
  else if (fieldDef.Contains("[Range]", StringComparison.OrdinalIgnoreCase))
  {
   ParseRange(key, currentSection, label, fieldDef, isRequired, defaultValue, tooltipNote, formFields);
  }
  else if (fieldDef.Contains("[Textarea]", StringComparison.OrdinalIgnoreCase) || fieldDef.Contains("[Textarea:", StringComparison.OrdinalIgnoreCase))
  {
   ParseTextArea(key, currentSection, label, fieldDef, isRequired, defaultValue, tooltipNote, formFields);
  }
  else if (fieldDef.Contains("[Number]", StringComparison.OrdinalIgnoreCase))
  {
   ParseNumber(key, currentSection, label, fieldDef, isRequired, defaultValue, tooltipNote, formFields);
  }
  else if (fieldDef.Contains("[Date]", StringComparison.OrdinalIgnoreCase))
  {
   ParseDate(key, currentSection, label, isRequired, defaultValue, tooltipNote, formFields);
  }
  else if (fieldDef.Contains("[Time]", StringComparison.OrdinalIgnoreCase))
  {
   ParseTime(key, currentSection, label, isRequired, defaultValue, tooltipNote, formFields);
  }
  else if (fieldDef.Contains("[Email]", StringComparison.OrdinalIgnoreCase))
  {
   ParseEmail(key, currentSection, label, isRequired, defaultValue, tooltipNote, formFields);
  }
  else if (fieldDef.Contains("[Password]", StringComparison.OrdinalIgnoreCase))
  {
   ParsePassword(key, currentSection, label, isRequired, defaultValue, tooltipNote, formFields);
  }
  else if (fieldDef.Contains("[Url]", StringComparison.OrdinalIgnoreCase))
  {
   ParseUrl(key, currentSection, label, isRequired, defaultValue, tooltipNote, formFields);
  }
  else if (fieldDef.Contains("[Phone]", StringComparison.OrdinalIgnoreCase))
  {
   ParsePhone(key, currentSection, label, isRequired, defaultValue, tooltipNote, formFields);
  }
  else if (fieldDef.Contains("___") || fieldDef.Contains("[Text]", StringComparison.OrdinalIgnoreCase))
  {
   ParseText(key, currentSection, label, isRequired, defaultValue, tooltipNote, formFields);
  }
  else if (fieldDef.Contains(","))
  {
   ParseSelect(key, currentSection, label, fieldDef, isRequired, defaultValue, tooltipNote, formFields);
  }
 }

 private static void ParseRadioButtonsOrCheckbox(string key, string currentSection, string label, string fieldDef, bool isRequired, string defaultValue, string tooltipNote, FormFieldList formFields)
 {
  var optionParts = fieldDef.Split(new[] { "O " }, StringSplitOptions.RemoveEmptyEntries);
  var options = optionParts
   .Select(o => o.Trim())
   .Where(o => !string.IsNullOrWhiteSpace(o))
   .ToList();

  // Wenn nur eine Option vorhanden ist, als Checkbox behandeln
  var fieldType = options.Count == 1 ? FieldType.CheckBox : FieldType.RadioButtons;

  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = fieldType,
   Options = options,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote
  });
 }

 private static void ParseMultiselect(string key, string currentSection, string label, string fieldDef, bool isRequired, string defaultValue, string tooltipNote, FormFieldList formFields)
 {
  var optionsString = System.Text.RegularExpressions.Regex.Replace(fieldDef, "\\[Multiselect\\]", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
  var options = optionsString.Split(',')
   .Select(o => o.Trim())
   .Where(o => !string.IsNullOrWhiteSpace(o))
   .ToList();

  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Multiselect,
   Options = options,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote
  });
 }

 private static void ParseRating(string key, string currentSection, string label, string fieldDef, bool isRequired, string defaultValue, string tooltipNote, FormFieldList formFields)
 {
  var rangeString = System.Text.RegularExpressions.Regex.Replace(fieldDef, "\\[Rating\\]", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
  var options = ParseRangeOptions(rangeString);

  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Rating,
   Options = options,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote
  });
 }

 private static void ParseRange(string key, string currentSection, string label, string fieldDef, bool isRequired, string defaultValue, string tooltipNote, FormFieldList formFields)
 {
  var rangeString = System.Text.RegularExpressions.Regex.Replace(fieldDef, "\\[Range\\]", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
  var (min, max) = ParseMinMax(rangeString);

  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Range,
   Min = min,
   Max = max,
   Value = !string.IsNullOrEmpty(defaultValue) ? defaultValue : min?.ToString(),
   Required = isRequired,
   Note = tooltipNote
  });
 }

 private static void ParseTextArea(string key, string currentSection, string label, string fieldDef, bool isRequired, string defaultValue, string tooltipNote, FormFieldList formFields)
 {
  // Extrahiere Zeilenzahl aus [Textarea:8]
  var rows = 4; // Standard: 4 Zeilen
  var match = System.Text.RegularExpressions.Regex.Match(fieldDef, @"\[Textarea:(\d+)\]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
  if (match.Success && int.TryParse(match.Groups[1].Value, out int rowCount))
  {
   rows = rowCount;
  }

  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.TextArea,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote,
   Options = new List<string> { rows.ToString() }
  });
 }

 private static void ParseNumber(string key, string currentSection, string label, string fieldDef, bool isRequired, string defaultValue, string tooltipNote, FormFieldList formFields)
 {
  var rangeString = System.Text.RegularExpressions.Regex.Replace(fieldDef, "\\[Number\\]", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
  var (min, max) = ParseMinMax(rangeString);

  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Number,
   Min = min,
   Max = max,
   Value = !string.IsNullOrEmpty(defaultValue) ? defaultValue : min?.ToString(),
   Required = isRequired,
   Note = tooltipNote
  });
 }

 private static void ParseDate(string key, string currentSection, string label, bool isRequired, string defaultValue, string tooltipNote, FormFieldList formFields)
 {
  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Date,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote
  });
 }

 private static void ParseTime(string key, string currentSection, string label, bool isRequired, string defaultValue, string tooltipNote, FormFieldList formFields)
 {
  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Time,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote
  });
 }

 private static void ParseEmail(string key, string currentSection, string label, bool isRequired, string defaultValue, string tooltipNote, FormFieldList formFields)
 {
  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Email,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote
  });
 }

 private static void ParsePassword(string key, string currentSection, string label, bool isRequired, string defaultValue, string tooltipNote, FormFieldList formFields)
 {
  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Password,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote
  });
 }

 private static void ParseUrl(string key, string currentSection, string label, bool isRequired, string defaultValue, string tooltipNote, FormFieldList formFields)
 {
  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Url,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote
  });
 }

 private static void ParsePhone(string key, string currentSection, string label, bool isRequired, string defaultValue, string tooltipNote, FormFieldList formFields)
 {
  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Phone,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote
  });
 }

 private static void ParseText(string key, string currentSection, string label, bool isRequired, string defaultValue, string tooltipNote, FormFieldList formFields)
 {
  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Text,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote
  });
 }

 private static void ParseSelect(string key, string currentSection, string label, string fieldDef, bool isRequired, string defaultValue, string tooltipNote, FormFieldList formFields)
 {
  var options = fieldDef.Split(',')
   .Select(o => o.Trim())
   .Where(o => !string.IsNullOrWhiteSpace(o))
   .ToList();

  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Select,
   Options = options,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote
  });
 }
 
 private static List<string> ParseRangeOptions(string rangeString)
 {
  var options = new List<string>();

  if (rangeString.Contains("-"))
  {
   var parts = rangeString.Split('-');
   if (parts.Length == 2 && int.TryParse(parts[0], out int start) && int.TryParse(parts[1], out int end))
   {
    for (int i = start; i <= end; i++)
    {
     options.Add(i.ToString());
    }
   }
  }

  return options;
 }

 private static (int? min, int? max) ParseMinMax(string rangeString)
 {
  int? min = null;
  int? max = null;

  if (rangeString.Contains("-"))
  {
   var parts = rangeString.Split('-');
   if (parts.Length == 2 && int.TryParse(parts[0], out int minVal) && int.TryParse(parts[1], out int maxVal))
   {
    min = minVal;
    max = maxVal;
   }
  }

  return (min, max);
 }
}
