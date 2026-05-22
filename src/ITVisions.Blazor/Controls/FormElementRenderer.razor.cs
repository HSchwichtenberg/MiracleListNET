using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace ITVisions.Blazor.Controls;

public partial class FormElementRenderer
{
 [Parameter]
 public string Template { get; set; }

 [Parameter]
 public EventCallback<FormElementList> OnValuesChanged { get; set; }

 [Parameter]
 public EventCallback<FormElementList> OnSubmited { get; set; }

 [Parameter]
 public bool ShowSubmitButton { get; set; } = true;

 [Parameter]
 public string SubmitButtonText { get; set; } = "Absenden";

 private FormElementList FormElements { get; set; } = new();
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
  if (!string.IsNullOrEmpty(Template))
  {
   FormElements = TextTemplateFormParser.Parse(Template);
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
  builder.AddAttribute(9, "onchange", EventCallback.Factory.CreateBinder<string>(this, async value =>
  {
   field.ValueString = value;
   await OnValuesChanged.InvokeAsync(FormElements);
  }, field.ValueString));
  builder.SetUpdatesAttributeName("value");
  builder.CloseElement();
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
 /// Absenden mit vorherige Validierung
 /// </summary>
 private async Task OnSubmit()
 {
  if (!ValidateFieldSet(FormElements))
  {
   StateHasChanged();
   return;
  }

  await OnValuesChanged.InvokeAsync(FormElements);
  await OnSubmited.InvokeAsync(FormElements);
 }

 /// <summary>
 /// Validierung beim Seitenwechel
 /// </summary>
 private bool ValidateCurrentPage()
 {
  if (Pages.Count == 0 || CurrentPageIndex >= Pages.Count)
   return true;

  return ValidateFieldSet(Pages[CurrentPageIndex].Fields);
 }

 /// <summary>
 /// Valiedert eine Liste von Eingabefeldern
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
 /// Valiedert ein einzelnes Eingabefeld
 /// </summary>
 private void ValidateField(FormElement field, List<string> missingFields, List<string> invalidFields)
 {
  var isEmpty = string.IsNullOrWhiteSpace(field.ValueString);

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

 #endregion

}