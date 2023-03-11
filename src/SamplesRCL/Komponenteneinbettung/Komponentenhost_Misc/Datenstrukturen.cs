using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Komponenteneinbettung.Komponentenhost_Misc
{
 public class CounterDataClass
 {
  int counter = 0;
  public int Counter
  {
   get
   {
    Console.WriteLine("CounterData.Get=" + counter);
    return counter;
   }
   set
   {
    Console.WriteLine("CounterData.Set=" + value);
    this.counter = value;
   }
  }
 }

 public struct CounterDataStruct
 {
  int counter;
  public int Counter
  {
   get
   {
    Console.WriteLine("CounterDataStruct.Get=" + counter);
    return counter;
   }
   set
   {
    Console.WriteLine("CounterDataStruct.Set=" + value);
    this.counter = value;
   }
  }
  public object Clone()
  {
   return this.MemberwiseClone();//boxing into object
  }
 }

 public record CounterDataRecord
 {
  int counter = 0;
  public int Counter
  {
   get
   {
    Console.WriteLine("CounterDataRecord.Get=" + counter);
    return counter;
   }
   set
   {
    Console.WriteLine("CounterDataRecord.Set=" + value);
    this.counter = value;
   }
  }
 }
}
