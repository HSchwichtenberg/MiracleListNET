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

 private bool ValidateCurrentPage()
 {
  ValidationErrorMessage = "";
  InvalidFieldKeys.Clear();

  if (Pages.Count == 0 || CurrentPageIndex >= Pages.Count)
   return true;

  var currentPage = Pages[CurrentPageIndex];
  var missingFields = new List<string>();
  var invalidFields = new List<string>();

  foreach (var field in currentPage.Fields.Where(f => f.Type != FormElementType.Headline && f.Type != FormElementType.Chapter && f.Type != FormElementType.Info))
  {
   var isEmpty = string.IsNullOrWhiteSpace(field.ValueString);

   // Prüfe Pflichtfelder
   if (field.Required && isEmpty)
   {
    missingFields.Add(field.Label);
    InvalidFieldKeys.Add(field.Key);
    continue;
   }

   // Prüfe Format-Validierung für gefüllte Felder
   if (!isEmpty)
   {
    var isValid = ValidateFieldFormat(field);
    if (!isValid)
    {
     invalidFields.Add(field.Label);
     InvalidFieldKeys.Add(field.Key);
    }
   }
  }

  var errors = new List<string>();
  if (missingFields.Any())
  {
   errors.Add($"Bitte füllen Sie folgende Pflichtfelder aus: {string.Join(", ", missingFields)}");
  }
  if (invalidFields.Any())
  {
   errors.Add($"Folgende Felder haben ein ungültigen Wert: {string.Join(", ", invalidFields)}");
  }

  if (errors.Any())
  {
   ValidationErrorMessage = string.Join("<br>", errors);
   return false;
  }

  return true;
 }

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

 private string GetFieldCssClass(FormElement field, string baseClass)
 {
  var cssClass = baseClass;
  if (InvalidFieldKeys.Contains(field.Key))
  {
   cssClass += " is-invalid";
  }
  return cssClass;
 }

 private async Task OnSubmit()
 {
  // Validierung aller Pflichtfelder und Formate beim finalen Submit
  ValidationErrorMessage = "";
  InvalidFieldKeys.Clear();
  var missingFields = new List<string>();
  var invalidFields = new List<string>();

  foreach (var field in FormElements.Where(f => f.Type != FormElementType.Headline && f.Type != FormElementType.Chapter && f.Type != FormElementType.Info))
  {
   var isEmpty = string.IsNullOrWhiteSpace(field.ValueString);

   // Prüfe Pflichtfelder
   if (field.Required && isEmpty)
   {
    missingFields.Add(field.Label);
    InvalidFieldKeys.Add(field.Key);
    continue;
   }

   // Prüfe Format-Validierung für gefüllte Felder
   if (!isEmpty)
   {
    var isValid = ValidateFieldFormat(field);
    if (!isValid)
    {
     invalidFields.Add(field.Label);
     InvalidFieldKeys.Add(field.Key);
    }
   }
  }

  var errors = new List<string>();
  if (missingFields.Any())
  {
   errors.Add($"Bitte füllen Sie folgende Pflichtfelder aus: {string.Join(", ", missingFields)}");
  }
  if (invalidFields.Any())
  {
   errors.Add($"Folgende Felder haben ein ungültiges Format: {string.Join(", ", invalidFields)}");
  }

  if (errors.Any())
  {
   ValidationErrorMessage = string.Join("<br>", errors);
   StateHasChanged();
   return;
  }

  await OnValuesChanged.InvokeAsync(FormElements);
  await OnSubmited.InvokeAsync(FormElements);
 }

 public FormElementList GetFormElements()
 {
  return FormElements;
 }

 private string RenderMarkdown(string text)
 {
  if (string.IsNullOrWhiteSpace(text))
   return text;

  // Einfache Markdown-Konvertierung
  var html = text;

  // **Bold**
  html = System.Text.RegularExpressions.Regex.Replace(html, @"\*\*(.*?)\*\*", "<strong>$1</strong>");

  // *Italic*
  html = System.Text.RegularExpressions.Regex.Replace(html, @"\*(.*?)\*", "<em>$1</em>");

  // [Link](url)
  html = System.Text.RegularExpressions.Regex.Replace(html, @"\[(.*?)\]\((.*?)\)", "<a href=\"$2\" target=\"_blank\">$1</a>");

  // `Code`
  html = System.Text.RegularExpressions.Regex.Replace(html, @"`(.*?)`", "<code>$1</code>");

  // Line breaks
  html = html.Replace("\n", "<br>");

  return html;
 }
}