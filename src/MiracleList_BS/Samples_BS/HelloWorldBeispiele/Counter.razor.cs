using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Samples_BS.HelloWorldBeispiele
{
 public partial class Counter
 {
  int currentCount = 0;

  void IncrementCount()
  {
   currentCount++;
  }

  protected override void OnInitialized()
  {
   
  }

 }
}
