using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BO
{
 /// <summary>
 /// Entity class representing a category of tasks
 /// Used on the server up to the WebAPI
 /// Corresponding proxy class in TypeScript is used on client
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
}