using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BO;

/// <summary>
/// Geschäftsobjektklasse für einen Benutzer
/// Wird nur auf dem Server verwendet, dort aber in allen Schichten von EFCore bis hin zum WebAPI
/// </summary>
public class User
{
 public int UserID { get; set; }

 [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
 public Guid UserGUID { get; set; }

 public string UserName { get; set; }
 public string PasswordHash { get; set; }

 [StringLength(38)] // Guid.NewGuid().ToString("B") => 38 characters (Braces)
 public string Token { get; set; }

 public byte[] Salt { get; set; }

 public DateTime Created { get; set; } = DateTime.Now;
 public DateTime? LastActivity { get; set; } = DateTime.Now;
 public string Memo { get; set; }
 public int? MaxTasks { get; set; }

 // -------------- Navigation Properties
 public List<Category> CategorySet { get; set; }
 public Client Client { get; set; }
 public Guid? ClientID
 {
  get; set;
 }
}