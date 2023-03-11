using ITVisions;
using ITVisions.Blazor;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using Web;

namespace Samples.Forms
{
 public partial class FormSimple
 {
  private Person person { get; set; } = new Person();
  private string Output { get; set; }
  private string OutputError { get; set; }

  [Inject]
  BlazorUtil Util { get; set; }

  protected override void OnInitialized()
  {
   // for easier testing, just some test data
   person = new Person()
   {
    Name = "Dr. Holger Schwichtenberg",
    JobTitle = JobTitle.Softwarearchitect,
    OtherRoles = new JobTitle[] { JobTitle.Trainer, JobTitle.Consultant, JobTitle.Author, JobTitle.Developer },
    Children = 2,
    Newsletter = true,
    EMail = "anfragen@IT-Visions.de",
    Notes = "Man kann mich auch als Berater und Trainer buchen."
   };
  }

  /// <summary>
  /// Klick auf Button
  /// </summary>
  private async void Submit()
  {
   OutputError = "";
   Output = "";

   // Validierung gegen die Data Annotations
   var validationContext = new ValidationContext(person);
   var validationResults = new List<ValidationResult>();
   var result = Validator.TryValidateObject(person, validationContext, validationResults, true);

   if (result) // OK!
   {
    Util.Log("Save");
    Output = "Object has been saved:<br>" + person.ToNameValueString(attributeSeparator: "<br>");
   }
   else // Not OK!
   {
    var alertMessage = "Some values are not valid:\n\n";
    OutputError += "Some values are not valid:<ul>";
    Util.Log($"{validationResults.Count} Validation Errors");
    foreach (var ve in validationResults)
    {
     foreach (var member in ve.MemberNames)
     {
      alertMessage += $"{member}: { ve.ErrorMessage}\n";
      OutputError += $"<li>{member}: { ve.ErrorMessage}</li>";
     }
    }
    OutputError += "</ul>";
    Util.Log(alertMessage);
    await Util.Alert(alertMessage);
   }
  }
 }
}