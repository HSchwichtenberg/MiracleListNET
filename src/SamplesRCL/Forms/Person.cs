using System;
using System.ComponentModel.DataAnnotations;

namespace Web;

/// <summary>
/// Aufzählungstyp für Datenklasse "Person"
/// </summary>
public enum JobTitle
{ Trainer, Consultant, Softwarearchitect, Developer, Tester, ProductManager, Author }

/// <summary>
/// Datenklasse für Blazor-Formularbeispiele
/// </summary>
public class Person
{
 [Required]
 [StringLength(20, ErrorMessage = "Name is too long: Max 20 letters!")]
 public string Name { get; set; }

 [Required]
 [EmailAddress]
 public string EMail { get; set; }

 [DayOfBirth(100, "Too young or older than 100 years!")]
 public DateTime DayOfBirth { get; set; }

 [Range(0, 10)]
 public int Children { get; set; }
 public bool Newsletter { get; set; }
 public string Notes { get; set; }

 public JobTitle JobTitle { get; set; }
 public JobTitle[] OtherRoles { get; set; }

 /// <summary>
 /// Problem: Blazor kann nicht an Enum-Zahlwert binden, nur an Zeichenkette
 /// Falls Bindung an Zahlwert erwünscht: Property-Wrapper für ID von JobTitle
 /// </summary>
 public string JobTitle_IDString
 {
  get => ((int)this.JobTitle).ToString();
  set
  {
   this.JobTitle = (JobTitle)Enum.Parse(typeof(JobTitle), value);
  }
 }
}
