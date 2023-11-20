using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bunit;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Samples.Komponentendateien;
using Xunit;
using static Samples.Komponenteneinbettung.Komponentenhost_Misc.CounterPanel;

namespace BlazorTests.TestSonstigeBeispiele
{

 public class CounterPanel_Test : TestContext
 {


  [Fact]
  public void OneClickTest()
  {
   var cut = RenderComponent<Samples.Komponenteneinbettung.Komponentenhost_ValueType.CounterPanel>();

   for (int i = 1; i <= 10; i++)
   {
    cut.Find("button").Click();
    cut.GetChangesSinceFirstRender().ShouldHaveSingleTextChange("Current count: " + i);
   }
  }

  List<int> ValueList = new List<int>();
  [Fact]
  public void ParameterTest()
  {

   const int StartWert = 100;
   var cut = RenderComponent<Samples.Komponenteneinbettung.Komponentenhost_ValueType.CounterPanel>
    (parameters => parameters.Add(x => x.StartValue, StartWert).Add(x => x.ValueHasChanged, (int x) => ValueHasChanged(x)));

   //(nameof(Samples.Komponenteneinbettung.Komponentenhost_ValueType.CounterPanel.StartValue), StartWert),
   //   new EventCallback(nameof(Samples.Komponenteneinbettung.Komponentenhost_ValueType.CounterPanel.ValueHasChanged), )
   //);

   cut.Find("button").Click();
   cut.GetChangesSinceFirstRender().ShouldHaveSingleTextChange("Current count: " + (StartWert + 1));
   cut.Find("button").Click();
   cut.GetChangesSinceFirstRender().ShouldHaveSingleTextChange("Current count: " + (StartWert + 2));

   Assert.Equal(100, ValueList[0]); // erster Aufruf von ValueHasChanged f�r den StartWert!
   Assert.Equal(101, ValueList[1]);
   Assert.Equal(102, ValueList[2]);
  }

  void ValueHasChanged(int x)
  {
   // geht hier nicht:    Assert.Equal(101, x);
   ValueList.Add(x);
  }
 }
}