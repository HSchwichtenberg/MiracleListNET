using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BO;

/// <summary>
/// Gesch�ftsobjektklasse f�r eine Aufgabe
/// Wird auf dem Server auf allen Schichten von EFCore bis hin zum WebAPI und auch in .NET-Clients verwendet
/// Auf dem JavaScript-basierten Clients wird eine hieraus generiert TypeScript-Proxy-Klasse  verwendet
/// </summary>
public class Task
{
 public const string DefaultTitle = "???";
 public int TaskID { get; set; } // PK per Konvention
 [MaxLength(250)] // alias: StringLength
 [MinLength(3)]
 [Required]
 public string Title { get; set; }
 public DateTime Created { get; set; } = DateTime.Now;
 public DateTime? Due { get; set; }
 public Importance? Importance { get; set; }
 public string Note { get; set; }

 /// <summary>
 /// NN = not Nullable --> wird verwendet f�r Datenbindung in Blazor
 /// </summary>
 [NotMapped]
 [JsonIgnore] // JsonIgnore from System.Text.Json.Serialization namespace. (JsonIgnore from Newtonsoft.Json will NOT work )
 public Importance ImportanceNN
 {
  get
  {
   return this.Importance ?? BO.Importance.A;
  }
  set
  {
   this.Importance = value;
  }
 }

 /// <summary>
 /// NN = not Nullable --> wird verwendet f�r Datenbindung in Blazor
 /// </summary>
 [NotMapped]
 [JsonIgnore] // JsonIgnore from System.Text.Json.Serialization namespace. (JsonIgnore from Newtonsoft.Json will NOT work )
 public DateTime DueNN
 {
  get
  {
   return this.Due ?? DateTime.Now;
  }
  set
  {
   this.Due = value;
  }
 }

 public bool Done { get; set; }
 public decimal? Effort { get; set; }
 public int Order { get; set; }
 public int? DueInDays { get; set; } // Computed Column, must be nullable as Due is nullable!
 //Alternativ berechnen im Client statt DB: public int DueInDays2 {  get { return (this.Due.GetValueOrDefault() - System.DateTime.Now).Days;  } }

 // -------------- Navigation Properties
 public List<SubTask> SubTaskSet { get; set; } // 1:N
 [Newtonsoft.Json.JsonIgnore] // Do not serialize 
 public Category Category { get; set; }
 public int CategoryID { get; set; } // optional: FK Property
}