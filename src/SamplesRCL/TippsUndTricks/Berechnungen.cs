using ITVisions;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Samples
{

 /// <summary>
 /// TODO: Mittlerweile auch Verfügbar in ITVAppUtil ITVisions.Maths
 /// </summary>
 class Berechnungen
 {

  /// <summary>
  /// Wikipedia: Die Fibonacci-Folge ist die unendliche Folge natürlicher Zahlen, die (ursprünglich) mit zweimal der Zahl 1 beginnt oder (häufig, in moderner Schreibweise) zusätzlich mit einer führenden Zahl 0 versehen ist.[1] Im Anschluss ergibt jeweils die Summe zweier aufeinanderfolgender Zahlen die unmittelbar danach folgende Zahl.
  /// </summary>
  private int Fibonacci(int n)
  {
   int a = 0;
   int b = 1;
   // In N steps compute Fibonacci sequence iteratively.
   for (int i = 0; i < n; i++)
   {
    int temp = a;
    a = b;
    b = temp + b;
   }
   return a;
  }

  public long Start(int repeat = 100000, int start = 0, int end = 42, bool useMemory = false)
  {
   long count = 0;
   try
   {
    Stopwatch sw = new Stopwatch();
    sw.Start();
    List<int> results = new List<int>();

    for (int j = 0; j < repeat; j++)
    {
     for (int i = start; i < end; i++)
     {
      var f = Fibonacci(i);
      if (useMemory) results.Add(f); // just for optional memory pressure ;-)
      count++;
     }
    }
    sw.Stop();
    Console.WriteLine(count + " Berechnungsergebnisse");
    return sw.ElapsedMilliseconds;

   }
   catch (Exception ex)
   {
    Console.WriteLine("Abbruch bei " + count + " / " + MathUtil.GetUsedRAM() + ": " + ex.ToString());
    throw;
   }
  }

  public async System.Threading.Tasks.Task<long> StartJS(IJSRuntime jsRuntime, int repeat = 100000, int start = 0, int end = 42)
  {
   try
   {
    Stopwatch sw = new Stopwatch();
    sw.Start();

    var skript = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "/_content/SamplesRCL/SamplesTS.js");
    var r = await skript.InvokeAsync<long>("TSUtil.FibonacciMany", repeat, start, end, false);

    sw.Stop();

    Console.WriteLine(r + " Berechnungsergebnisse");
    return sw.ElapsedMilliseconds;
   }
   catch (Exception ex)
   {
    Console.WriteLine("Fehler / " + MathUtil.GetUsedRAM() + ": " + ex.ToString());
    throw;
   }
  }
 }
}
