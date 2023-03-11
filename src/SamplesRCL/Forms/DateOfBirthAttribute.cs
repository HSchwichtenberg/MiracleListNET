using System;
using System.ComponentModel.DataAnnotations;

namespace Web
{
 public class DayOfBirthAttribute : ValidationAttribute
 {
  private byte maxAge = 120;

  public DayOfBirthAttribute(byte maxAge = 120, string ErrorMessage = "Too young or too old!")
 : base(ErrorMessage)
  {
   this.maxAge = maxAge;
  }

  public override bool IsValid(object value)
  {
   DateTime result;
   bool parsed = DateTime.TryParse(value.ToString(), out result);
   if (!parsed) return false;
   if (result > DateTime.Now) return false;
   if (result < DateTime.Now.AddYears(-this.maxAge)) return false;
   return true;
  }
 }
}
