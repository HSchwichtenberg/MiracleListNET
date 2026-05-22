using System;
using System.Collections.Generic;
using System.Linq;

namespace ITVisions.Blazor.Controls;

public static class TextTemplateFormParser
{
 /// <summary>
 /// Parsen eines Texttemplates in eine strukturierte Liste von FormElement-Objekten, die verschiedene Elementtypen und Optionen unterstützen.
 /// </summary>
 public static FormElementList Parse(string template)
 {
  var FormElements = new FormElementList();

  if (string.IsNullOrEmpty(template))
  {
   return FormElements;
  }

  var lines = template.Split('\n', StringSplitOptions.TrimEntries);
  string currentSection = null;
  int fieldCounter = 0;

  foreach (var line in lines)
  {
   if (string.IsNullOrWhiteSpace(line)) continue;

   // Chapter mit ##
   if (line.StartsWith("##"))
   {
    currentSection = ParseChapter(line, ref fieldCounter, FormElements);
    continue;
   }

   // Headline mit #
   if (line.StartsWith("#"))
   {
    currentSection = ParseHeadline(line, ref fieldCounter, FormElements);
    continue;
   }

   // Zeile mit Feld (- oder +)
   if (line.StartsWith("-") || line.StartsWith("+"))
   {
    ParseField(line, currentSection, ref fieldCounter, FormElements);
   }
   else
   {
    // Zeilen ohne - oder + werden als Info-Text (Markdown) behandelt
    ParseInfoLine(line, ref fieldCounter, FormElements);
   }
  }

  return FormElements;
 }

 /// <summary>
 /// Chapter auswerten (##)
 /// </summary>
 private static string ParseChapter(string line, ref int fieldCounter, FormElementList FormElements)
 {
  var chapterText = line.TrimStart('#').Trim();
  FormElements.Add(new FormElement
  {
   Key = $"chapter_{fieldCounter++}",
   Label = chapterText,
   Type = FormElementType.Chapter
  });
  return chapterText;
 }

 /// <summary>
 /// Überschrift auswerten
 /// </summary>
 private static string ParseHeadline(string line, ref int fieldCounter, FormElementList FormElements)
 {
  var headlineText = line.TrimStart('#').Trim();
  FormElements.Add(new FormElement
  {
   Key = $"headline_{fieldCounter++}",
   Label = headlineText,
   Type = FormElementType.Headline
  });
  return headlineText;
 }

 /// <summary>
 /// Info-Zeile (Markdown-Text ohne Eingabefeld) auswerten
 /// </summary>
 private static void ParseInfoLine(string line, ref int fieldCounter, FormElementList FormElements)
 {
  FormElements.Add(new FormElement
  {
   Key = $"info_{fieldCounter++}",
   Label = line.Trim(),
   Type = FormElementType.Info
  });
 }

 /// <summary>
 /// Feld auswerten
 /// </summary>
 private static void ParseField(string line, string currentSection, ref int fieldCounter, FormElementList FormElements)
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
  if (FormElements.Any(f => f.Label == label))
  {
   label = $"{label}_!DOPPELT!_{Guid.NewGuid():N}";
  }

  var baseKey = $"{currentSection}_{label}".Replace(" ", "_").Replace(":", "");
  var key = baseKey;

  // Falls Key bereits vergeben ist, hänge GUID an
  if (FormElements.Any(f => f.Key == key))
  {
   key = $"{baseKey}_{Guid.NewGuid():N}";
  }

  fieldCounter++;

  // Prüfe auf verschiedene Feldtypen
  if (fieldDef.Contains("* "))
  {
   ParseRadioButtonsOrCheckbox(key, currentSection, label, fieldDef, isRequired, defaultValue, tooltipNote, FormElements);
  }
  else if (fieldDef.Contains("[Multiselect]", StringComparison.OrdinalIgnoreCase))
  {
   ParseMultiselect(key, currentSection, label, fieldDef, isRequired, defaultValue, tooltipNote, FormElements);
  }
  else if (fieldDef.Contains("[Rating]", StringComparison.OrdinalIgnoreCase))
  {
   ParseRating(key, currentSection, label, fieldDef, isRequired, defaultValue, tooltipNote, FormElements);
  }
  else if (fieldDef.Contains("[Range]", StringComparison.OrdinalIgnoreCase))
  {
   ParseRange(key, currentSection, label, fieldDef, isRequired, defaultValue, tooltipNote, FormElements);
  }
  else if (fieldDef.Contains("[Textarea]", StringComparison.OrdinalIgnoreCase) || fieldDef.Contains("[Textarea:", StringComparison.OrdinalIgnoreCase))
  {
   ParseTextArea(key, currentSection, label, fieldDef, isRequired, defaultValue, tooltipNote, FormElements);
  }
  else if (fieldDef.Contains("[Number]", StringComparison.OrdinalIgnoreCase))
  {
   ParseNumber(key, currentSection, label, fieldDef, isRequired, defaultValue, tooltipNote, FormElements);
  }
  else if (fieldDef.Contains("[Date]", StringComparison.OrdinalIgnoreCase))
  {
   ParseDate(key, currentSection, label, isRequired, defaultValue, tooltipNote, FormElements);
  }
  else if (fieldDef.Contains("[Time]", StringComparison.OrdinalIgnoreCase))
  {
   ParseTime(key, currentSection, label, isRequired, defaultValue, tooltipNote, FormElements);
  }
  else if (fieldDef.Contains("[Email]", StringComparison.OrdinalIgnoreCase))
  {
   ParseEmail(key, currentSection, label, isRequired, defaultValue, tooltipNote, FormElements);
  }
  else if (fieldDef.Contains("[Password]", StringComparison.OrdinalIgnoreCase))
  {
   ParsePassword(key, currentSection, label, isRequired, defaultValue, tooltipNote, FormElements);
  }
  else if (fieldDef.Contains("[Url]", StringComparison.OrdinalIgnoreCase))
  {
   ParseUrl(key, currentSection, label, isRequired, defaultValue, tooltipNote, FormElements);
  }
  else if (fieldDef.Contains("[Phone]", StringComparison.OrdinalIgnoreCase))
  {
   ParsePhone(key, currentSection, label, isRequired, defaultValue, tooltipNote, FormElements);
  }
  else if (fieldDef.Contains("___") || fieldDef.Contains("[Text]", StringComparison.OrdinalIgnoreCase))
  {
   ParseText(key, currentSection, label, isRequired, defaultValue, tooltipNote, FormElements);
  }
  else if (fieldDef.Contains("|"))
  {
   ParseSelect(key, currentSection, label, fieldDef, isRequired, defaultValue, tooltipNote, FormElements);
  }
 }

 private static void ParseRadioButtonsOrCheckbox(string key, string currentSection, string label, string fieldDef, bool isRequired, string defaultValue, string tooltipNote, FormElementList FormElements)
 {
  var optionParts = fieldDef.Split(new[] { "* " }, StringSplitOptions.RemoveEmptyEntries);
  var options = optionParts
   .Select(o => o.Trim())
   .Where(o => !string.IsNullOrWhiteSpace(o))
   .ToList();

  // Wenn nur eine Option vorhanden ist, als Checkbox behandeln
  var formElementType = options.Count == 1 ? FormElementType.CheckBox : FormElementType.RadioButtons;

  FormElements.Add(new FormElement
  {
   Key = key,
   Label = label,
   Type = formElementType,
   Options = options,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote
  });
 }

 private static void ParseMultiselect(string key, string currentSection, string label, string fieldDef, bool isRequired, string defaultValue, string tooltipNote, FormElementList FormElements)
 {
  var optionsString = System.Text.RegularExpressions.Regex.Replace(fieldDef, "\\[Multiselect\\]", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();

  // Verwende Pipe als Trennzeichen
  var options = optionsString.Split('|')
   .Select(o => o.Trim())
   .Where(o => !string.IsNullOrWhiteSpace(o))
   .ToList();

  FormElements.Add(new FormElement
  {
   Key = key,
   Label = label,
   Type = FormElementType.Multiselect,
   Options = options,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote
  });
 }

 private static void ParseRating(string key, string currentSection, string label, string fieldDef, bool isRequired, string defaultValue, string tooltipNote, FormElementList FormElements)
 {
  var rangeString = System.Text.RegularExpressions.Regex.Replace(fieldDef, "\\[Rating\\]", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
  var options = ParseRangeOptions(rangeString);

  FormElements.Add(new FormElement
  {
   Key = key,
   Label = label,
   Type = FormElementType.Rating,
   Options = options,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote
  });
 }

 private static void ParseRange(string key, string currentSection, string label, string fieldDef, bool isRequired, string defaultValue, string tooltipNote, FormElementList FormElements)
 {
  var rangeString = System.Text.RegularExpressions.Regex.Replace(fieldDef, "\\[Range\\]", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
  var (min, max) = ParseMinMax(rangeString);

  FormElements.Add(new FormElement
  {
   Key = key,
   Label = label,
   Type = FormElementType.Range,
   Min = min,
   Max = max,
   Value = !string.IsNullOrEmpty(defaultValue) ? defaultValue : min?.ToString(),
   Required = isRequired,
   Note = tooltipNote
  });
 }

 private static void ParseTextArea(string key, string currentSection, string label, string fieldDef, bool isRequired, string defaultValue, string tooltipNote, FormElementList FormElements)
 {
  // Extrahiere Zeilenzahl aus [Textarea:8]
  var rows = 4; // Standard: 4 Zeilen
  var match = System.Text.RegularExpressions.Regex.Match(fieldDef, @"\[Textarea:(\d+)\]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
  if (match.Success && int.TryParse(match.Groups[1].Value, out int rowCount))
  {
   rows = rowCount;
  }

  FormElements.Add(new FormElement
  {
   Key = key,
   Label = label,
   Type = FormElementType.TextArea,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote,
   Options = new List<string> { rows.ToString() }
  });
 }

 private static void ParseNumber(string key, string currentSection, string label, string fieldDef, bool isRequired, string defaultValue, string tooltipNote, FormElementList FormElements)
 {
  var rangeString = System.Text.RegularExpressions.Regex.Replace(fieldDef, "\\[Number\\]", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
  var (min, max) = ParseMinMax(rangeString);

  FormElements.Add(new FormElement
  {
   Key = key,
   Label = label,
   Type = FormElementType.Number,
   Min = min,
   Max = max,
   Value = !string.IsNullOrEmpty(defaultValue) ? defaultValue : min?.ToString(),
   Required = isRequired,
   Note = tooltipNote
  });
 }

 private static void ParseDate(string key, string currentSection, string label, bool isRequired, string defaultValue, string tooltipNote, FormElementList FormElements)
 {
  FormElements.Add(new FormElement
  {
   Key = key,
   Label = label,
   Type = FormElementType.Date,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote
  });
 }

 private static void ParseTime(string key, string currentSection, string label, bool isRequired, string defaultValue, string tooltipNote, FormElementList FormElements)
 {
  FormElements.Add(new FormElement
  {
   Key = key,
   Label = label,
   Type = FormElementType.Time,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote
  });
 }

 private static void ParseEmail(string key, string currentSection, string label, bool isRequired, string defaultValue, string tooltipNote, FormElementList FormElements)
 {
  FormElements.Add(new FormElement
  {
   Key = key,
   Label = label,
   Type = FormElementType.Email,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote
  });
 }

 private static void ParsePassword(string key, string currentSection, string label, bool isRequired, string defaultValue, string tooltipNote, FormElementList FormElements)
 {
  FormElements.Add(new FormElement
  {
   Key = key,
   Label = label,
   Type = FormElementType.Password,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote
  });
 }

 private static void ParseUrl(string key, string currentSection, string label, bool isRequired, string defaultValue, string tooltipNote, FormElementList FormElements)
 {
  FormElements.Add(new FormElement
  {
   Key = key,
   Label = label,
   Type = FormElementType.Url,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote
  });
 }

 private static void ParsePhone(string key, string currentSection, string label, bool isRequired, string defaultValue, string tooltipNote, FormElementList FormElements)
 {
  FormElements.Add(new FormElement
  {
   Key = key,
   Label = label,
   Type = FormElementType.Phone,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote
  });
 }

 private static void ParseText(string key, string currentSection, string label, bool isRequired, string defaultValue, string tooltipNote, FormElementList FormElements)
 {
  FormElements.Add(new FormElement
  {
   Key = key,
   Label = label,
   Type = FormElementType.Text,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote
  });
 }

 private static void ParseSelect(string key, string currentSection, string label, string fieldDef, bool isRequired, string defaultValue, string tooltipNote, FormElementList FormElements)
 {
  var trimmedFieldDef = fieldDef.Trim();
  var allowEmpty = trimmedFieldDef.StartsWith("|");

  // Verwende Pipe als Trennzeichen
  var options = fieldDef.Split('|')
   .Select(o => o.Trim())
   .ToList();

  // Wenn nicht allowEmpty, entferne leere Einträge
  if (!allowEmpty)
  {
   options = options.Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
  }

  FormElements.Add(new FormElement
  {
   Key = key,
   Label = label,
   Type = FormElementType.Select,
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
