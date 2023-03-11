using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.RazorSyntax
{
 partial class Variablennutzung
 {

  int field3 = 3;
  int prop3 { get; set; } = 3;
  protected override void OnInitialized()
  {
   //field1 = 22; // geht nicht
   //prop1 = 22; // geht nicht
   base.OnInitialized();
  }
 }
}
