using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace BO
{
 /// <summary>
 /// Entity class representing a category of tasks
 /// Used on the server up to the WebAPI
 /// Not used in the client!
 /// </summary>
 public class Client
 {
  public Guid ClientID { get; set; }
  [StringLength(50)]
  public string Name { get; set; }
  [StringLength(50)]
  public string Company { get; set; }
  [StringLength(50)]
  public string EMail { get; set; }
  public DateTime Created { get; set; } = DateTime.Now;
  public DateTime? Deleted { get; set; }
  public string Memo { get; set; }
  [StringLength(10)]
  public string Type { get; set; }
  // -------------- Navigation Properties
  public List<User> UserSet { get; set; }
 }
}