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
    currentSection = ParseHeadline(line, ref fieldCounter, formFields);
    continue;
   }

   // Zeile mit Feld (- oder +)
   if (line.StartsWith("-") || line.StartsWith("+"))
   {
    ParseFieldWithPrefix(line, currentSection, ref fieldCounter, formFields);
   }
   // Standalone Felder (ohne - oder +)
   else
   {
    ParseStandaloneField(line, ref fieldCounter, formFields);
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
  var key = $"{currentSection}_{label}".Replace(" ", "_").Replace(":", "");
  fieldCounter++;

  // Prüfe auf verschiedene Feldtypen
  if (fieldDef.Contains("O "))
  {
   ParseRadioButtonsOrCheckbox(key, currentSection, label, fieldDef, isRequired, formFields);
  }
  else if (fieldDef.Contains("[Multiselect]"))
  {
   ParseMultiselect(key, currentSection, label, fieldDef, isRequired, formFields);
  }
  else if (fieldDef.Contains("[Rating]"))
  {
   ParseRating(key, currentSection, label, fieldDef, isRequired, formFields);
  }
  else if (fieldDef.Contains("[Textarea]"))
  {
   ParseTextArea(key, currentSection, label, isRequired, formFields);
  }
  else if (fieldDef.Contains("[Number]"))
  {
   ParseNumber(key, currentSection, label, fieldDef, isRequired, formFields);
  }
  else if (fieldDef.Contains("[Date]"))
  {
   ParseDate(key, currentSection, label, isRequired, formFields);
  }
  else if (fieldDef.Contains("[Email]"))
  {
   ParseEmail(key, currentSection, label, isRequired, formFields);
  }
  else if (fieldDef.Contains("[Phone]"))
  {
   ParsePhone(key, currentSection, label, isRequired, formFields);
  }
  else if (fieldDef.Contains("___") || fieldDef.Contains("[Text]"))
  {
   ParseText(key, currentSection, label, isRequired, formFields);
  }
  else if (fieldDef.Contains(","))
  {
   ParseSelect(key, currentSection, label, fieldDef, isRequired, formFields);
  }
 }

 private static void ParseRadioButtonsOrCheckbox(string key, string currentSection, string label, string fieldDef, bool isRequired, FormFieldList formFields)
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
   Required = isRequired
  });
 }

 private static void ParseMultiselect(string key, string currentSection, string label, string fieldDef, bool isRequired, FormFieldList formFields)
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
   Required = isRequired
  });
 }

 private static void ParseRating(string key, string currentSection, string label, string fieldDef, bool isRequired, FormFieldList formFields)
 {
  var rangeString = fieldDef.Replace("[Rating]", "").Trim();
  var options = ParseRangeOptions(rangeString);

  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Rating,
   Options = options,
   Required = isRequired
  });
 }

 private static void ParseTextArea(string key, string currentSection, string label, bool isRequired, FormFieldList formFields)
 {
  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.TextArea,
   Required = isRequired
  });
 }

 private static void ParseNumber(string key, string currentSection, string label, string fieldDef, bool isRequired, FormFieldList formFields)
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
   Value = min,
   Required = isRequired
  });
 }

 private static void ParseDate(string key, string currentSection, string label, bool isRequired, FormFieldList formFields)
 {
  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Date,
   Required = isRequired
  });
 }

 private static void ParseEmail(string key, string currentSection, string label, bool isRequired, FormFieldList formFields)
 {
  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Email,
   Required = isRequired
  });
 }

 private static void ParsePhone(string key, string currentSection, string label, bool isRequired, FormFieldList formFields)
 {
  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Phone,
   Required = isRequired
  });
 }

 private static void ParseText(string key, string currentSection, string label, bool isRequired, FormFieldList formFields)
 {
  formFields.Add(new FormField
  {
   Key = key,
   Label = label,
   Type = FieldType.Text,
   Required = isRequired
  });
 }

 private static void ParseSelect(string key, string currentSection, string label, string fieldDef, bool isRequired, FormFieldList formFields)
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
   Required = isRequired
  });
 }

 private static void ParseStandaloneField(string line, ref int fieldCounter, FormFieldList formFields)
 {
  if (line.Contains("___") || line.Contains("[Text]"))
  {
   ParseStandaloneText(line, ref fieldCounter, formFields);
  }
  else if (line.Contains("[Textarea]"))
  {
   ParseStandaloneTextArea(line, ref fieldCounter, formFields);
  }
  else if (line.Contains("[Number]"))
  {
   ParseStandaloneNumber(line, ref fieldCounter, formFields);
  }
  else if (line.Contains("[Date]"))
  {
   ParseStandaloneDate(line, ref fieldCounter, formFields);
  }
  else if (line.Contains("[Email]"))
  {
   ParseStandaloneEmail(line, ref fieldCounter, formFields);
  }
  else if (line.Contains("[Phone]"))
  {
   ParseStandalonePhone(line, ref fieldCounter, formFields);
  }
 }

 private static void ParseStandaloneText(string line, ref int fieldCounter, FormFieldList formFields)
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

 private static void ParseStandaloneTextArea(string line, ref int fieldCounter, FormFieldList formFields)
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

 private static void ParseStandaloneNumber(string line, ref int fieldCounter, FormFieldList formFields)
 {
  var tempLine = line.Replace("[Number]", "|||[Number]|||");
  var parts = tempLine.Split(new[] { "|||[Number]|||" }, StringSplitOptions.None);
  var label = parts[0].TrimEnd(':').Trim();
  var rangeString = parts.Length > 1 ? parts[1].Trim() : "";
  var key = $"field_{fieldCounter++}_{label.Replace(" ", "_")}";

  var (min, max) = ParseMinMax(rangeString);

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

 private static void ParseStandaloneDate(string line, ref int fieldCounter, FormFieldList formFields)
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

 private static void ParseStandaloneEmail(string line, ref int fieldCounter, FormFieldList formFields)
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

 private static void ParseStandalonePhone(string line, ref int fieldCounter, FormFieldList formFields)
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
