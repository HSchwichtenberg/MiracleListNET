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
   
   }
  }

  return formFields;
 }

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

 private static void ParseFieldWithPrefix(string line, string currentSection, ref int fieldCounter, FormFieldList formFields)
 {
  var isRequired = line.StartsWith("+");
  var fieldLine = line.TrimStart('-', '+').Trim();
  var parts = fieldLine.Split(':', 2);

  if (parts.Length != 2) return;

  var label = parts[0].Trim();
  var fieldDef = parts[1].Trim();

  // Prüfe auf Standardwert (= wert am Ende)
  string defaultValue = null;
  if (fieldDef.Contains(" = "))
  {
   var defaultParts = fieldDef.Split(new[] { " = " }, StringSplitOptions.None);
   if (defaultParts.Length == 2)
   {
    fieldDef = defaultParts[0].Trim();
    defaultValue = defaultParts[1].Trim();
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
   ParseRadioButtonsOrCheckbox(key, currentSection, label, fieldDef, isRequired, defaultValue, formFields);
  }
  else if (fieldDef.Contains("[Multiselect]"))
  {
   ParseMultiselect(key, currentSection, label, fieldDef, isRequired, defaultValue, formFields);
  }
  else if (fieldDef.Contains("[Rating]"))
  {
   ParseRating(key, currentSection, label, fieldDef, isRequired, defaultValue, formFields);
  }
  else if (fieldDef.Contains("[Range]"))
  {
   ParseRange(key, currentSection, label, fieldDef, isRequired, defaultValue, formFields);
  }
  else if (fieldDef.Contains("[Textarea]"))
  {
   ParseTextArea(key, currentSection, label, isRequired, defaultValue, formFields);
  }
  else if (fieldDef.Contains("[Number]"))
  {
   ParseNumber(key, currentSection, label, fieldDef, isRequired, defaultValue, formFields);
  }
  else if (fieldDef.Contains("[Date]"))
  {
   ParseDate(key, currentSection, label, isRequired, defaultValue, formFields);
  }
  else if (fieldDef.Contains("[Time]"))
  {
   ParseTime(key, currentSection, label, isRequired, defaultValue, formFields);
  }
  else if (fieldDef.Contains("[Email]"))
  {
   ParseEmail(key, currentSection, label, isRequired, defaultValue, formFields);
  }
  else if (fieldDef.Contains("[Password]"))
  {
   ParsePassword(key, currentSection, label, isRequired, defaultValue, formFields);
  }
  else if (fieldDef.Contains("[Url]"))
  {
   ParseUrl(key, currentSection, label, isRequired, defaultValue, formFields);
  }
  else if (fieldDef.Contains("[Phone]"))
  {
   ParsePhone(key, currentSection, label, isRequired, defaultValue, formFields);
  }
  else if (fieldDef.Contains("___") || fieldDef.Contains("[Text]"))
  {
   ParseText(key, currentSection, label, isRequired, defaultValue, formFields);
  }
  else if (fieldDef.Contains(","))
  {
   ParseSelect(key, currentSection, label, fieldDef, isRequired, defaultValue, formFields);
  }
 }

 private static void ParseRadioButtonsOrCheckbox(string key, string currentSection, string label, string fieldDef, bool isRequired, string defaultValue, FormFieldList formFields)
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
   Value = defaultValue
  });
 }

 private static void ParseMultiselect(string key, string currentSection, string label, string fieldDef, bool isRequired, string defaultValue, FormFieldList formFields)
 {
  var optionsString = fieldDef.Replace("[Multiselect]", "").Trim();
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
   Value = defaultValue
  });
 }

 private static void ParseRating(string key, string currentSection, string label, string fieldDef, bool isRequired, string defaultValue, FormFieldList formFields)
 {
  var rangeString = fieldDef.Replace("[Rating]", "").Trim();
  var options = ParseRangeOptions(rangeString);

  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Rating,
   Options = options,
   Required = isRequired,
   Value = defaultValue
  });
 }

 private static void ParseRange(string key, string currentSection, string label, string fieldDef, bool isRequired, string defaultValue, FormFieldList formFields)
 {
  var rangeString = fieldDef.Replace("[Range]", "").Trim();
  var (min, max) = ParseMinMax(rangeString);

  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Range,
   Min = min,
   Max = max,
   Value = !string.IsNullOrEmpty(defaultValue) ? defaultValue : min?.ToString(),
   Required = isRequired
  });
 }

 private static void ParseTextArea(string key, string currentSection, string label, bool isRequired, string defaultValue, FormFieldList formFields)
 {
  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.TextArea,
   Required = isRequired,
   Value = defaultValue
  });
 }

 private static void ParseNumber(string key, string currentSection, string label, string fieldDef, bool isRequired, string defaultValue, FormFieldList formFields)
 {
  var rangeString = fieldDef.Replace("[Number]", "").Trim();
  var (min, max) = ParseMinMax(rangeString);

  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Number,
   Min = min,
   Max = max,
   Value = !string.IsNullOrEmpty(defaultValue) ? defaultValue : min?.ToString(),
   Required = isRequired
  });
 }

 private static void ParseDate(string key, string currentSection, string label, bool isRequired, string defaultValue, FormFieldList formFields)
 {
  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Date,
   Required = isRequired,
   Value = defaultValue
  });
 }

 private static void ParseTime(string key, string currentSection, string label, bool isRequired, string defaultValue, FormFieldList formFields)
 {
  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Time,
   Required = isRequired,
   Value = defaultValue
  });
 }

 private static void ParseEmail(string key, string currentSection, string label, bool isRequired, string defaultValue, FormFieldList formFields)
 {
  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Email,
   Required = isRequired,
   Value = defaultValue
  });
 }

 private static void ParsePassword(string key, string currentSection, string label, bool isRequired, string defaultValue, FormFieldList formFields)
 {
  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Password,
   Required = isRequired,
   Value = defaultValue
  });
 }

 private static void ParseUrl(string key, string currentSection, string label, bool isRequired, string defaultValue, FormFieldList formFields)
 {
  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Url,
   Required = isRequired,
   Value = defaultValue
  });
 }

 private static void ParsePhone(string key, string currentSection, string label, bool isRequired, string defaultValue, FormFieldList formFields)
 {
  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Phone,
   Required = isRequired,
   Value = defaultValue
  });
 }

 private static void ParseText(string key, string currentSection, string label, bool isRequired, string defaultValue, FormFieldList formFields)
 {
  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Text,
   Required = isRequired,
   Value = defaultValue
  });
 }

 private static void ParseSelect(string key, string currentSection, string label, string fieldDef, bool isRequired, string defaultValue, FormFieldList formFields)
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
   Value = defaultValue
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
