using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
namespace ITVisions.Blazor.Controls;

public partial class FormElementRenderer
{

 [Parameter]
 public EventCallback<FormElementList> OnValuesChanged { get; set; }

 [Parameter]
 public EventCallback<FormElementList> OnSubmitted { get; set; }

 [Parameter]
 public bool ShowSubmitButton { get; set; } = true;

 [Parameter]
 public string SubmitButtonText { get; set; } = "Absenden";

 [Parameter]
 public FormElementList FormElements { get; set; } = new();
 private string ValidationErrorMessage { get; set; } = "";
 private HashSet<string> CollapsedSections { get; set; } = new();
 private List<FormPage> Pages { get; set; } = new();
 private int CurrentPageIndex { get; set; } = 0;
 private HashSet<string> InvalidFieldKeys { get; set; } = new();

 private class FormPage
 {
  public string PageName { get; set; }
  public FormElement ChapterField { get; set; }
  public List<FormElement> Fields { get; set; } = new();
 }

 protected override void OnParametersSet()
 {
  if (FormElements != null && FormElements.Any())
  {
   // Wenn FormElements direkt übergeben wurden (ohne Template)
   BuildPages();
  }
 }

 #region ----------------- Rendering
 private void BuildPages()
 {
  Pages.Clear();
  CurrentPageIndex = 0;

  FormPage currentPage = null;

  foreach (var field in FormElements)
  {
   if (field.Type == FormElementType.Chapter)
   {
    // Neue Seite für jedes Chapter beginnen
    currentPage = new FormPage
    {
     PageName = field.Label,
     ChapterField = field
    };
    Pages.Add(currentPage);
   }
   else
   {
    // Wenn noch keine Seite existiert, erstelle eine Standard-Seite
    if (currentPage == null)
    {
     currentPage = new FormPage
     {
      PageName = "Formular",
      ChapterField = null
     };
     Pages.Add(currentPage);
    }

    // Füge Feld zur aktuellen Seite hinzu (auch Headlines)
    currentPage.Fields.Add(field);
   }
  }

  // Wenn keine Seiten erstellt wurden, erstelle eine Standard-Seite
  if (Pages.Count == 0)
  {
   Pages.Add(new FormPage
   {
    PageName = "Formular",
    ChapterField = null,
    Fields = FormElements.Where(f => f.Type != FormElementType.Chapter).ToList()
   });
  }
 }

 private FormElement GetParentHeadline(FormElement field, List<FormElement> fieldsInPage)
 {
  // Finde die letzte Headline vor diesem Feld
  var fieldIndex = fieldsInPage.IndexOf(field);
  if (fieldIndex == -1) return null;

  for (int i = fieldIndex - 1; i >= 0; i--)
  {
   if (fieldsInPage[i].Type == FormElementType.Headline)
   {
    return fieldsInPage[i];
   }
  }

  return null;
 }

 private void NextPage()
 {
  if (CurrentPageIndex < Pages.Count - 1)
  {
   // Validierung der aktuellen Seite vor dem Wechsel
   if (!ValidateCurrentPage())
   {
    StateHasChanged();
    return;
   }

   ValidationErrorMessage = ""; // Fehlermeldung zurücksetzen
   InvalidFieldKeys.Clear(); // Fehlerhafte Felder zurücksetzen
   CurrentPageIndex++;
   CollapsedSections.Clear(); // Zurücksetzen der eingeklappten Sektionen
   StateHasChanged();
  }
 }

 private void PreviousPage()
 {
  if (CurrentPageIndex > 0)
  {
   ValidationErrorMessage = ""; // Fehlermeldung zurücksetzen beim Zurückgehen
   InvalidFieldKeys.Clear(); // Fehlerhafte Felder zurücksetzen
   CurrentPageIndex--;
   CollapsedSections.Clear(); // Zurücksetzen der eingeklappten Sektionen
   StateHasChanged();
  }
 }

 /// <summary>
 /// Rendert ein einzelnes FormElement 
 /// </summary>
 private RenderFragment RenderEditableFormElement(FormElement field) => builder =>
 {
  // Label mit Required-Stern und Info-Icon
  builder.AddContent(0, RenderLabel(field));

  // Eingabe-Element basierend auf Typ
  switch (field.Type)
  {
   case FormElementType.RadioButtons:
    builder.OpenElement(11, "div");
    builder.AddAttribute(12, "class", "d-flex gap-3");
    foreach (var option in field.Options)
    {
     var currentOption = option;
     builder.AddContent(13, RenderFormCheckItem(
      inputType: "radio",
      fieldKey: field.Key,
      optionId: $"{field.Key}_{option}",
      optionLabel: option,
      isChecked: field.ValueString == option,
      isDisabled: field.ReadOnly,
      onChange: EventCallback.Factory.Create(this, () =>
      {
       field.ValueString = currentOption;
       NotifyValuesChanged();
      }),
      nameAttribute: field.Key
     ));
    }
    builder.CloseElement(); // d-flex div
    break;

   case FormElementType.CheckBox:
    var checkboxLabel = field.Options?.Count > 0 ? field.Options[0] : field.Label;
    builder.AddContent(28, RenderFormCheckItem(
     inputType: "checkbox",
     fieldKey: field.Key,
     optionId: field.Key,
     optionLabel: checkboxLabel,
     isChecked: !string.IsNullOrEmpty(field.ValueString),
     isDisabled: field.ReadOnly,
     onChange: EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
     {
      var label = field.Options?.Count > 0 ? field.Options[0] : field.Label;
      field.ValueString = ((bool)e.Value) ? label : "";
      NotifyValuesChanged();
     })
    ));
    break;

   case FormElementType.Select:
    builder.OpenElement(41, "select");
    builder.AddAttribute(42, "class", GetFieldCssClass(field, "form-select"));
    builder.AddAttribute(43, "value", field.ValueString);
    builder.AddAttribute(44, "required", field.Required);
    builder.AddAttribute(45, "disabled", field.ReadOnly);
    builder.AddAttribute(46, "onchange", EventCallback.Factory.CreateBinder<string>(this, async value =>
    {
     field.ValueString = value;
     await OnValuesChanged.InvokeAsync(FormElements);
    }, field.ValueString));
    builder.SetUpdatesAttributeName("value");

    builder.OpenElement(47, "option");
    builder.AddAttribute(48, "value", "");
    builder.AddContent(49, "Bitte wählen...");
    builder.CloseElement();

    foreach (var option in field.Options)
    {
     builder.OpenElement(50, "option");
     builder.AddAttribute(51, "value", option);
     builder.AddContent(52, option);
     builder.CloseElement();
    }

    builder.CloseElement(); // select
    break;

   case FormElementType.Multiselect:
    builder.OpenElement(53, "div");
    builder.AddAttribute(54, "class", "d-flex flex-column gap-2");
    foreach (var option in field.Options)
    {
     var isSelected = field.ValueString?.Split('|').Select(v => v.Trim()).Contains(option) ?? false;
     var currentOption = option;
     builder.AddContent(55, RenderFormCheckItem(
      inputType: "checkbox",
      fieldKey: field.Key,
      optionId: $"{field.Key}_{option}",
      optionLabel: option,
      isChecked: isSelected,
      isDisabled: field.ReadOnly,
      onChange: EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
      {
       HandleMultiselectChange(field, currentOption, (bool)e.Value);
      })
     ));
    }
    builder.CloseElement(); // d-flex flex-column div
    break;

   case FormElementType.Rating:
    builder.OpenElement(68, "div");
    builder.AddAttribute(69, "class", "d-flex gap-2 align-items-center");
    foreach (var option in field.Options)
    {
     var isSelected = field.ValueString == option;
     builder.OpenElement(70, "button");
     builder.AddAttribute(71, "type", "button");
     builder.AddAttribute(72, "class", "btn btn-outline-secondary");
     builder.AddAttribute(73, "style", "min-width: 40px;");
     builder.AddAttribute(74, "disabled", field.ReadOnly);
     var currentOption = option;
     builder.AddAttribute(75, "onclick", EventCallback.Factory.Create(this, () =>
     {
      field.ValueString = currentOption;
      NotifyValuesChanged();
     }));
     builder.AddContent(76, isSelected ? "★" : "☆");
     builder.AddContent(77, option);
     builder.CloseElement();
    }
    if (!string.IsNullOrEmpty(field.ValueString))
    {
     builder.OpenElement(78, "button");
     builder.AddAttribute(79, "type", "button");
     builder.AddAttribute(80, "class", "btn btn-sm btn-link");
     builder.AddAttribute(81, "disabled", field.ReadOnly);
     builder.AddAttribute(82, "onclick", EventCallback.Factory.Create(this, () =>
     {
      field.ValueString = "";
      NotifyValuesChanged();
     }));
     builder.AddContent(83, "Zurücksetzen");
     builder.CloseElement();
    }
    builder.CloseElement(); // d-flex div
    break;

   case FormElementType.Range:
    builder.OpenElement(84, "div");
    builder.AddAttribute(85, "class", "d-flex gap-3 align-items-center");

    builder.OpenElement(86, "input");
    builder.AddAttribute(87, "type", "range");
    builder.AddAttribute(88, "class", "form-range");
    builder.AddAttribute(89, "style", "flex: 1;");
    builder.AddAttribute(90, "value", field.ValueString);
    builder.AddAttribute(91, "min", field.Min);
    builder.AddAttribute(92, "max", field.Max);
    builder.AddAttribute(93, "required", field.Required);
    builder.AddAttribute(94, "disabled", field.ReadOnly);
    builder.AddAttribute(95, "onchange", EventCallback.Factory.CreateBinder<string>(this, async value =>
    {
     field.ValueString = value;
     await OnValuesChanged.InvokeAsync(FormElements);
    }, field.ValueString));
    builder.SetUpdatesAttributeName("value");
    builder.CloseElement();

    builder.OpenElement(96, "span");
    builder.AddAttribute(97, "class", "badge bg-primary");
    builder.AddAttribute(98, "style", "min-width: 50px;");
    builder.AddContent(99, field.ValueString);
    builder.CloseElement();

    builder.CloseElement(); // d-flex div
    break;

   case FormElementType.TextArea:
    var rows = field.Options?.Count > 0 && int.TryParse(field.Options[0], out int r) ? r : 4;
    builder.OpenElement(100, "textarea");
    builder.AddAttribute(101, "class", GetFieldCssClass(field, "form-control"));
    builder.AddAttribute(102, "rows", rows);
    builder.AddAttribute(103, "value", field.ValueString);
    builder.AddAttribute(104, "required", field.Required);
    builder.AddAttribute(105, "readonly", field.ReadOnly);
    builder.AddAttribute(106, "disabled", field.ReadOnly);
    builder.AddAttribute(107, "onchange", EventCallback.Factory.CreateBinder<string>(this, async value =>
    {
     field.ValueString = value;
     await OnValuesChanged.InvokeAsync(FormElements);
    }, field.ValueString));
    builder.SetUpdatesAttributeName("value");
    builder.CloseElement();
    break;

   case FormElementType.Text:
    builder.AddContent(108, RenderInput(field, "text", field.Regex));
    break;

   case FormElementType.Number:
    builder.AddContent(109, RenderInput(field, "number", null, null, field.Min, field.Max));
    break;

   case FormElementType.Date:
    builder.AddContent(110, RenderInput(field, "date"));
    break;

   case FormElementType.Time:
    builder.AddContent(111, RenderInput(field, "time"));
    break;

   case FormElementType.Email:
    builder.AddContent(112, RenderInput(field, "email"));
    break;

   case FormElementType.Password:
    builder.AddContent(113, RenderInput(field, "password"));
    break;

   case FormElementType.Phone:
    builder.AddContent(114, RenderInput(field, "tel", @"[0-9\s\-\+\(\)\/]+", "Nur Zahlen und Sonderzeichen (+, -, /, Leerzeichen, Klammern) erlaubt"));
    break;

   case FormElementType.Url:
    builder.AddContent(115, RenderInput(field, "url"));
    break;
  }
 };

 private RenderFragment RenderInput(FormElement field, string inputType, string pattern = null, string title = null, int? min = null, int? max = null) => builder =>
 {
  builder.OpenElement(0, "input");
  builder.AddAttribute(1, "type", inputType);
  builder.AddAttribute(2, "class", GetFieldCssClass(field, "form-control"));
  builder.AddAttribute(3, "value", BindConverter.FormatValue(field.ValueString));
  if (!string.IsNullOrWhiteSpace(pattern))
  {
   builder.AddAttribute(4, "pattern", pattern);
  }
  if (!string.IsNullOrWhiteSpace(title))
  {
   builder.AddAttribute(5, "title", title);
  }
  if (min.HasValue)
  {
   builder.AddAttribute(6, "min", min.Value);
  }
  if (max.HasValue)
  {
   builder.AddAttribute(7, "max", max.Value);
  }
  builder.AddAttribute(8, "required", field.Required);
  builder.AddAttribute(9, "disabled", field.ReadOnly);
  builder.AddAttribute(10, "onchange", EventCallback.Factory.CreateBinder<string>(this, async value =>
  {
   field.ValueString = value;
   await OnValuesChanged.InvokeAsync(FormElements);
  }, field.ValueString));
  builder.SetUpdatesAttributeName("value");
  builder.CloseElement();
 };

 /// <summary>
 /// Rendert ein form-check Item (Radio-Button oder Checkbox) mit Input und Label
 /// </summary>
 private RenderFragment RenderFormCheckItem(string inputType, string fieldKey, string optionId, string optionLabel, bool isChecked, bool isDisabled, object onChange, string nameAttribute = null) => builder =>
 {
  builder.OpenElement(0, "div");
  builder.AddAttribute(1, "class", "form-check");

  builder.OpenElement(2, "input");
  builder.AddAttribute(3, "class", "form-check-input");
  builder.AddAttribute(4, "type", inputType);
  if (!string.IsNullOrEmpty(nameAttribute))
  {
   builder.AddAttribute(5, "name", nameAttribute);
  }
  builder.AddAttribute(6, "id", optionId);
  builder.AddAttribute(7, "checked", isChecked);
  builder.AddAttribute(8, "disabled", isDisabled);
  builder.AddAttribute(9, "onchange", onChange);
  builder.CloseElement();

  builder.OpenElement(10, "label");
  builder.AddAttribute(11, "class", "form-check-label");
  builder.AddAttribute(12, "for", optionId);
  builder.AddContent(13, optionLabel);
  builder.CloseElement();

  builder.CloseElement(); // form-check div
 };

 /// <summary>
 /// Rendert das Label eines Felds mit Required-Stern und Info-Icon
 /// </summary>
 private RenderFragment RenderLabel(FormElement field) => builder =>
 {
  if (!string.IsNullOrEmpty(field.Label))
  {
   builder.OpenElement(0, "label");
   builder.AddAttribute(1, "class", "fw-bold");
   builder.AddContent(2, field.Label);

   if (field.Required)
   {
    builder.OpenElement(3, "span");
    builder.AddAttribute(4, "style", "margin-left:5px");
    builder.AddAttribute(5, "title", "Pflichtfeld");
    builder.AddAttribute(6, "class", "text-danger");
    builder.AddContent(7, "*");
    builder.CloseElement();
   }

   if (!string.IsNullOrEmpty(field.Note))
   {
    builder.OpenComponent<InfoIcon>(8);
    builder.AddAttribute(9, "Icon", "ℹ️");
    builder.AddAttribute(10, "Text", field.Note);
    builder.CloseComponent();
   }

   builder.CloseElement(); // label
  }
 };

 private string GetFieldCssClass(FormElement field, string baseClass)
 {
  var cssClass = baseClass;
  if (InvalidFieldKeys.Contains(field.Key))
  {
   cssClass += " is-invalid";
  }
  return cssClass;
 }
 #endregion

 #region ----------------- Steuerelementverhalten
 private void ToggleSection(string sectionKey)
 {
  if (CollapsedSections.Contains(sectionKey))
  {
   CollapsedSections.Remove(sectionKey);
  }
  else
  {
   CollapsedSections.Add(sectionKey);
  }
 }

 private async void NotifyValuesChanged()
 {
  await OnValuesChanged.InvokeAsync(FormElements);
 }

 private void HandleMultiselectChange(FormElement field, string option, bool isChecked)
 {
  var selectedValues = string.IsNullOrEmpty(field.ValueString)
   ? new List<string>()
   : field.ValueString.Split('|').Select(v => v.Trim()).ToList();

  if (isChecked && !selectedValues.Contains(option))
  {
   selectedValues.Add(option);
  }
  else if (!isChecked && selectedValues.Contains(option))
  {
   selectedValues.Remove(option);
  }

  field.ValueString = string.Join("| ", selectedValues);
  NotifyValuesChanged();
 }

 #endregion

 #region ----------------- Validierung

 /// <summary>
 /// Absenden mit vorheriger Validierung
 /// </summary>
 private async Task OnSubmit()
 {
  if (!ValidateFieldSet(FormElements))
  {
   StateHasChanged();
   return;
  }

  await OnValuesChanged.InvokeAsync(FormElements);
  await OnSubmitted.InvokeAsync(FormElements);
 }

 /// <summary>
 /// Validierung beim Seitenwechsel
 /// </summary>
 private bool ValidateCurrentPage()
 {
  if (Pages.Count == 0 || CurrentPageIndex >= Pages.Count)
   return true;

  return ValidateFieldSet(Pages[CurrentPageIndex].Fields);
 }

 /// <summary>
 /// Validiert eine Liste von Eingabefeldern
 /// </summary>
 private bool ValidateFieldSet(IEnumerable<FormElement> elements)
 {
  ValidationErrorMessage = "";
  InvalidFieldKeys.Clear();

  var missingFields = new List<string>();
  var invalidFields = new List<string>();

  var validatableElements = elements.Where(f => f.Type != FormElementType.Headline && f.Type != FormElementType.Chapter && f.Type != FormElementType.Info);
  foreach (var field in validatableElements)
  {
   ValidateField(field, missingFields, invalidFields);
  }

  ValidationErrorMessage = BuildValidationErrorMessage(missingFields, invalidFields);
  return string.IsNullOrEmpty(ValidationErrorMessage);
 }

 /// <summary>
 /// Validiert ein einzelnes Eingabefeld
 /// </summary>
 private void ValidateField(FormElement field, List<string> missingFields, List<string> invalidFields)
 {
  var isEmpty = string.IsNullOrWhiteSpace(field.ValueString);
  if (field.Type == FormElementType.Date)
  {
   // ist leer, wenn 1.1.0001 oder ungültiges Datum
   if (isEmpty || !DateTime.TryParse(field.ValueString, out DateTime dateValue) || dateValue == DateTime.MinValue)
   {
    isEmpty = true;
   }
   else
   {
    isEmpty = false;
   }
  }

  // Pflichtfeld?
  if (field.Required && isEmpty)
  {
   missingFields.Add(field.Label);
   InvalidFieldKeys.Add(field.Key);
   return;
  }

  // Inhalt OK?
  if (!isEmpty && !ValidateFieldFormat(field))
  {
   invalidFields.Add(field.Label);
   InvalidFieldKeys.Add(field.Key);
  }
 }

 /// <summary>
 /// Validierung Inhalt eines Eingabefeldes
 /// </summary>
 private bool ValidateFieldFormat(FormElement field)
 {
  if (string.IsNullOrWhiteSpace(field.ValueString))
   return true;

  // Regex-Validierung (wenn vorhanden)
  if (!string.IsNullOrWhiteSpace(field.Regex))
  {
   try
   {
    return System.Text.RegularExpressions.Regex.IsMatch(field.ValueString, field.Regex);
   }
   catch (System.Text.RegularExpressions.RegexParseException)
   {
    // Ungültiger Regex-Pattern -> ignorieren und mit anderen Validierungen fortfahren
   }
  }

  switch (field.Type)
  {
   case FormElementType.Email:
    return System.Text.RegularExpressions.Regex.IsMatch(field.ValueString,
     @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

   case FormElementType.Phone:
    return System.Text.RegularExpressions.Regex.IsMatch(field.ValueString,
     @"^[0-9\s\-\+\(\)\/]+$");

   case FormElementType.Url:
    return Uri.TryCreate(field.ValueString, UriKind.Absolute, out var uriResult)
     && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

   case FormElementType.Number:
    if (!int.TryParse(field.ValueString, out int numberValue))
     return false;

    if (field.Min.HasValue && numberValue < field.Min.Value)
     return false;

    if (field.Max.HasValue && numberValue > field.Max.Value)
     return false;

    return true;

   default:
    return true;
  }
 }

 /// <summary>
 /// Fehlermeldung erzeugen
 /// </summary>
 private static string BuildValidationErrorMessage(List<string> missingFields, List<string> invalidFields)
 {
  var errors = new List<string>();

  if (missingFields.Any())
  {
   errors.Add($"Bitte füllen Sie folgende Pflichtfelder aus: {string.Join(", ", missingFields)}");
  }

  if (invalidFields.Any())
  {
   errors.Add($"Folgende Felder haben ein ungültiges Format: {string.Join(", ", invalidFields)}");
  }

  return string.Join("<br>", errors);
 }

 /// <summary>
 /// Berechnet optimale Bootstrap-Spaltenbreite für Gruppenfeldanzahl
 /// Zielt darauf ab, möglichst alle 12 Grid-Spalten zu nutzen
 /// </summary>
 private static int GetOptimalColumnSize(int fieldCount)
 {
  return fieldCount switch
  {
   1 => 12,  // 1 Feld: volle Breite
   2 => 6,   // 2 Felder: je 6 Spalten = 12
   3 => 4,   // 3 Felder: je 4 Spalten = 12
   4 => 3,   // 4 Felder: je 3 Spalten = 12
   5 => 2,   // 5 Felder: je 2 Spalten = 10 (besser als 12/5=2 mit nur 10)
   6 => 2,   // 6 Felder: je 2 Spalten = 12
   _ => 1    // 7+ Felder: je 1 Spalte (12 von 12 ab 12 Feldern)
  };
 }

 #endregion

}