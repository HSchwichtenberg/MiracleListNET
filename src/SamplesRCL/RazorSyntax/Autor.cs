using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web
{
 public class Autor
 {
  public int ID { get; set; } = 1;
  public string Name { get; set; } = "HS";
  public string Website { get; set; }
  public Autor()
  {

  }
  public Autor(string Name)
  {
   this.Name = Name;
  }
  public string GetInfo<T>()
  {
   if (typeof(T) == typeof(System.String)) return Name;
   else return ID.ToString();
  }
 }
}
