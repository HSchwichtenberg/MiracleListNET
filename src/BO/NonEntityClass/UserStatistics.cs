using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BO
{
 public class UserStatistics
 {
  [Key] // The entity type 'UserStatistics' requires a primary key to be defined.
  public string UserName { get; set; }
  public int NumberOfTasks { get; set; }

 }
}
