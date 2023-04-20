using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BO
{

 /// <summary>
 /// Entity class representing a task
 /// Used on the server on all layers up to the WebAPI and also in .NET clients
 /// Corresponding proxy class in TypeScript is used on client
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

  [NotMapped]
  [JsonIgnore]  //Use JsonIgnore from System.Text.Json.Serialization namespace. (JsonIgnore from Newtonsoft.Json will NOT work )
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

  [NotMapped]
  [JsonIgnore]  //Use JsonIgnore from System.Text.Json.Serialization namespace. (JsonIgnore from Newtonsoft.Json will NOT work )
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
  //public int DueInDays2 {  get { return (this.Due.GetValueOrDefault() - System.DateTime.Now).Days;  } }

  // -------------- Navigation Properties
  public List<SubTask> SubTaskSet { get; set; } // 1:N
  [Newtonsoft.Json.JsonIgnore] // Do not serialize 
  public Category Category { get; set; }
  public int CategoryID { get; set; } // optional: FK Property
 }
}