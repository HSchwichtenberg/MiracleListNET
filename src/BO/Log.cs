
using System.ComponentModel.DataAnnotations;

namespace BO
{
public enum Severity
 {
  Information = 1, Warning = 2, Error = 3, Critical = 4
 }

 public enum Event
 {
  ClientCreated = 10, 
 
  LoginOK = 20, LogginError = 30,
  TokenCheckOK = 30, TokenCheckError = 40,
  Call = 50

 }

 public partial class Log
 {
  public int LogID { get; set; }
  public System.DateTime DateTime { get; set; } = System.DateTime.Now;
  public Severity Severity { get; set; }
  public Event Event { get; set; }
  public string Token { get; set; }
  public string Operation { get; set; }
  public int? UserID { get; set; }
  public string Text { get; set; }
  public string Note { get; set; }
  [StringLength(15)]
  public string Client { get; set; }
  public string ClientDetails { get; set; }
 }

 
}
