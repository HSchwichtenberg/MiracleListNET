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
 /// Ein Eingabefeld auswerten
 /// </summary>
 private static void ParseField(string line, string currentSection, ref int fieldCounter, FormElementList FormElements)
 {
  ParseFieldContext parseContext = CreateParseFieldContext(line, currentSection, FormElements);
  if (parseContext == null)
  {
   return;
  }

  fieldCounter++;

  var key = parseContext.Key;
  var label = parseContext.Label;
  var fieldDef = parseContext.FieldDefinition;
  var isRequired = parseContext.IsRequired;
  var defaultValue = parseContext.DefaultValue;
  var tooltipNote = parseContext.TooltipNote;
  var regexPattern = parseContext.RegexPattern;

  if (fieldDef.Contains("* "))
  {
   ParseCheckboxOrRadioButtons(key, currentSection, label, fieldDef, isRequired, defaultValue, tooltipNote, FormElements);
   return;
  }

  if (fieldDef.Contains("[Multiselect]", StringComparison.OrdinalIgnoreCase))
  {
   ParseMultiselect(key, currentSection, label, fieldDef, isRequired, defaultValue, tooltipNote, FormElements);
   return;
  }

  if (fieldDef.Contains("[Rating]", StringComparison.OrdinalIgnoreCase))
  {
   ParseRating(key, currentSection, label, fieldDef, isRequired, defaultValue, tooltipNote, FormElements);
   return;
  }

  if (fieldDef.Contains("[Range]", StringComparison.OrdinalIgnoreCase))
  {
   ParseRange(key, currentSection, label, fieldDef, isRequired, defaultValue, tooltipNote, FormElements);
   return;
  }

  if (fieldDef.Contains("[Textarea]", StringComparison.OrdinalIgnoreCase) || fieldDef.Contains("[Textarea:", StringComparison.OrdinalIgnoreCase))
  {
   ParseTextArea(key, currentSection, label, fieldDef, isRequired, defaultValue, tooltipNote, FormElements);
   return;
  }

  if (fieldDef.Contains("[Number]", StringComparison.OrdinalIgnoreCase))
  {
   ParseNumber(key, currentSection, label, fieldDef, isRequired, defaultValue, tooltipNote, FormElements);
   return;
  }

  if (fieldDef.Contains("[Date]", StringComparison.OrdinalIgnoreCase))
  {
   ParseDate(key, currentSection, label, isRequired, defaultValue, tooltipNote, FormElements);
   return;
  }

  if (fieldDef.Contains("[Time]", StringComparison.OrdinalIgnoreCase))
  {
   ParseTime(key, currentSection, label, isRequired, defaultValue, tooltipNote, FormElements);
   return;
  }

  if (fieldDef.Contains("[Email]", StringComparison.OrdinalIgnoreCase))
  {
   ParseEmail(key, currentSection, label, isRequired, defaultValue, tooltipNote, FormElements);
   return;
  }

  if (fieldDef.Contains("[Password]", StringComparison.OrdinalIgnoreCase))
  {
   ParsePassword(key, currentSection, label, isRequired, defaultValue, tooltipNote, FormElements);
   return;
  }

  if (fieldDef.Contains("[Url]", StringComparison.OrdinalIgnoreCase))
  {
   ParseUrl(key, currentSection, label, isRequired, defaultValue, tooltipNote, FormElements);
   return;
  }

  if (fieldDef.Contains("[Phone]", StringComparison.OrdinalIgnoreCase))
  {
   ParsePhone(key, currentSection, label, isRequired, defaultValue, tooltipNote, FormElements);
   return;
  }

  if (fieldDef.Contains("___") || fieldDef.Contains("[Text]", StringComparison.OrdinalIgnoreCase))
  {
   ParseText(key, currentSection, label, isRequired, defaultValue, tooltipNote, regexPattern, FormElements);
   return;
  }

  if (fieldDef.Contains("|"))
  {
   ParseSelect(key, currentSection, label, fieldDef, isRequired, defaultValue, tooltipNote, FormElements);
  }
 }

 /// <summary>
 /// Zusatzinformationen aus der Zeile extrahieren (z.B. ob das Feld erforderlich ist, Standardwert, Tooltip-Notiz, Regex-Pattern) und Kontext für die weitere Verarbeitung erstellen
 /// </summary>
 private static ParseFieldContext CreateParseFieldContext(string line, string currentSection, FormElementList FormElements)
 {
  var isRequired = line.StartsWith("+");
  var fieldLine = line.TrimStart('-', '+').Trim();
  var parts = fieldLine.Split(':', 2);

  if (!TryGetLabelAndFieldDefinition(parts, out var label, out var fieldDef))
  {
   return null;
  }

  var defaultValue = ExtractDefaultValueFromLabel(parts, ref label);
  var tooltipNote = ExtractTooltipNote(ref label, ref defaultValue);
  var regexPattern = ExtractRegexPattern(ref fieldDef);
  ExtractDefaultValueFromFieldDefinition(ref fieldDef, ref defaultValue);
  label = EnsureUniqueLabel(label, FormElements);
  var key = CreateUniqueKey(currentSection, label, FormElements);

  return new ParseFieldContext
  {
   CurrentSection = currentSection,
   Key = key,
   Label = label,
   FieldDefinition = fieldDef,
   IsRequired = isRequired,
   DefaultValue = defaultValue,
   TooltipNote = tooltipNote,
   RegexPattern = regexPattern
  };
 }

 private static bool TryGetLabelAndFieldDefinition(string[] parts, out string label, out string fieldDef)
 {
  label = null;
  fieldDef = null;

  if (parts.Length == 1)
  {
   label = parts[0].Trim();
   fieldDef = "[Text]";
   return true;
  }

  if (parts.Length == 2)
  {
   label = parts[0].Trim();
   fieldDef = parts[1].Trim();
   return true;
  }

  return false;
 }

 private static string ExtractDefaultValueFromLabel(string[] parts, ref string label)
 {
  if (parts.Length != 1 || !label.Contains(" = "))
  {
   return null;
  }

  var defaultParts = label.Split(new[] { " = " }, StringSplitOptions.None);
  if (defaultParts.Length < 2)
  {
   return null;
  }

  label = defaultParts[0].Trim();
  return string.Join(" = ", defaultParts.Skip(1)).Trim().Trim('"');
 }

 private static string ExtractTooltipNote(ref string label, ref string defaultValue)
 {
  if (!label.Contains("{") || !label.Contains("}"))
  {
   return null;
  }

  var startIndex = label.IndexOf('{');
  var endIndex = label.IndexOf('}');
  if (endIndex <= startIndex)
  {
   return null;
  }

  var tooltipNote = label.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();
  var labelRemainder = label.Substring(endIndex + 1).Trim();
  label = label.Substring(0, startIndex).Trim();

  if (labelRemainder.Contains(" = "))
  {
   var defaultParts = labelRemainder.Split(new[] { " = " }, StringSplitOptions.None);
   if (defaultParts.Length >= 2)
   {
    defaultValue = string.Join(" = ", defaultParts.Skip(1)).Trim().Trim('"');
   }
  }

  return tooltipNote;
 }

 private static string ExtractRegexPattern(ref string fieldDef)
 {
  if (string.IsNullOrWhiteSpace(fieldDef))
  {
   return null;
  }

  var trimmedFieldDef = fieldDef.Trim();

  // Suche nach Anführungszeichen am Ende der Felddefinition
  var lastQuoteIndex = trimmedFieldDef.LastIndexOf('"');
  if (lastQuoteIndex <= 0)
  {
   return null;
  }

  // Suche nach dem öffnenden Anführungszeichen
  var firstQuoteIndex = trimmedFieldDef.LastIndexOf('"', lastQuoteIndex - 1);
  if (firstQuoteIndex < 0)
  {
   return null;
  }

  // Extrahiere den Regex-Pattern
  var regexPattern = trimmedFieldDef.Substring(firstQuoteIndex + 1, lastQuoteIndex - firstQuoteIndex - 1);

  // Entferne den Regex-Teil aus der Felddefinition
  fieldDef = trimmedFieldDef.Substring(0, firstQuoteIndex).Trim();

  return regexPattern;
 }

 private static void ExtractDefaultValueFromFieldDefinition(ref string fieldDef, ref string defaultValue)
 {
  if (!fieldDef.Contains(" = "))
  {
   return;
  }

  var defaultParts = fieldDef.Split(new[] { " = " }, StringSplitOptions.None);
  if (defaultParts.Length != 2)
  {
   return;
  }

  fieldDef = defaultParts[0].Trim();
  defaultValue = defaultParts[1].Trim().Trim('"');
 }

 private static string EnsureUniqueLabel(string label, FormElementList FormElements)
 {
  if (!FormElements.Any(f => f.Label == label))
  {
   return label;
  }

  return $"{label}_!DOPPELT!_{Guid.NewGuid():N}";
 }

 /// <summary>
 /// Eindeutiger Schlüssel pro Feld aus dem Kapitelnamen und dem Label (wenn diese nicht eindeutig sind, wird zusätzlich eine GUID eingefügt in die ID) erzeugen
 /// </summary>
 private static string CreateUniqueKey(string currentSection, string label, FormElementList FormElements)
 {
  var baseKey = $"{currentSection}_{label}".Replace(" ", "_").Replace(":", "");
  if (!FormElements.Any(f => f.Key == baseKey))
  {
   return baseKey;
  }

  return $"{baseKey}_{Guid.NewGuid():N}";
 }

 private sealed class ParseFieldContext
 {
  public string CurrentSection { get; set; }
  public string Key { get; set; }
  public string Label { get; set; }
  public string FieldDefinition { get; set; }
  public bool IsRequired { get; set; }
  public string DefaultValue { get; set; }
  public string TooltipNote { get; set; }
  public string RegexPattern { get; set; }
 }

 private static void ParseCheckboxOrRadioButtons(string key, string currentSection, string label, string fieldDef, bool isRequired, string defaultValue, string tooltipNote, FormElementList FormElements)
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

 private static void ParseText(string key, string currentSection, string label, bool isRequired, string defaultValue, string tooltipNote, string regexPattern, FormElementList FormElements)
 {
  FormElements.Add(new FormElement
  {
   Key = key,
   Label = label,
   Type = FormElementType.Text,
   Required = isRequired,
   Value = defaultValue,
   Note = tooltipNote,
   Regex = regexPattern
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
