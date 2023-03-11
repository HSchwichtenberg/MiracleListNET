using System;
using System.Collections.Generic;

// Testen der BD-Lösung
//using Pages = BD.Web.Pages;
// oder z.B. Blazor Server
// using Pages = Web.Pages;

namespace MiracleListTests;

public class TestStore {

 public List<BO.Category> Data = new();

 public TestStore() {

  for (int i = 1; i < 4; i++) {
   var c = new BO.Category() { CategoryID = i, Name = "Testkategorie " + i }; ;
   c.TaskSet = new List<BO.Task>();
   for (int j = 1; j < 6; j++) {
    var t = new BO.Task() { TaskID = j, Title = "Testaufgabe " + j, Importance = BO.Importance.B, Due = DateTime.Now };
    c.TaskSet.Add(t);
   }
   Data.Add(c);
  }
 }
}
