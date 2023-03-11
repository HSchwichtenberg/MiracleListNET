using System;

namespace Samples.Komponenteneinbettung.Komponentenhost_ReferenceType
{

 /// <summary>
 /// Datenstruktur für den Datenaustausch zw. KomponentenHost und CounterPanel
 /// </summary>
 public class CounterData
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
}