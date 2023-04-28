using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BO;

/// <summary>
/// Geschäftsobjektklasse für eine Aufgabenkategorie
/// Wird auf dem Server auf allen Schichten von EFCore bis hin zum WebAPI und auch in .NET-Clients verwendet
/// Auf dem JavaScript-basierten Clients wird eine hieraus generiert TypeScript-Proxy-Klasse  verwendet
/// </summary>
public class Category
{
 public int CategoryID { get; set; } // PK

 [MaxLength(50)] 
 public string Name { get; set; }

 public DateTime Created { get; set; } = DateTime.Now;

 // -------------- Navigation Properties
 public List<Task> TaskSet { get; set; }
 [Newtonsoft.Json.JsonIgnore] // Do not serialize 
 public User User { get; set; }
 [Newtonsoft.Json.JsonIgnore] // Do not serialize 
 public int UserID { get; set; }
 
}