using System;
using Xunit.Abstractions;

namespace BlazorTests.CounterTests;

public class CounterTests : BunitContext
{
 private readonly ITestOutputHelper output;

 public CounterTests(ITestOutputHelper output)
 {
  this.output = output;
 }

 /// <summary>
 /// Wait bei Counter.razor nicht notwendig, da Komponente nicht asynchron arbeitet!
 /// </summary>
 [Fact]
 public void CounterTest()
 {
  // Arrange (cut = Component under Test)
  var cut = Render<Counter>();

  // Act
  cut.Find("button").Click();

  // Assert: Ungünstig und falsch :-(
  //Assert.Equal("<p>Current count: 1</p>", cut.Find("p").OuterHtml);

  // Zur Diagnose: Haltepunkt nach der Zuweisung!
  //var html = cut.Find("div").OuterHtml;
  //output.WriteLine(html);

  // Assert: Ungünstig, aber richtig :-)
  // Assert.Equal("<p>\n Current count: \n 1</p>", cut.Find("p").OuterHtml);

  // Raw Literal String macht \r\n statt nur \n
  //Assert.Equal("""
  //<p>
  // Current count:
  // 1 </p>
  //""", cut.Find("p").OuterHtml);

  // Assert: besser und richtig!
  cut.Find("p").MarkupMatches(@"<p>Current count: 1</p>");
 }


 /// <summary>
 /// Wait bei CounterAsync.razor zwingend notwendig, da Komponente asynchron arbeitet!
 /// </summary>
 [Fact]
 public void CounterTestmitWait()
 {
  // Arrange (cut = Component under Test)
  var cut = Render<CounterAsync>();

  // Act
  cut.Find("button").Click();

  // Wait: Ungünstig, aber richtig
  var html = cut.Find("p").OuterHtml; // zur Diagnose               
  // Achtung: Das ist falsch, da nur einmal ausgewertet
  //cut.WaitForState(() => html == ("<p>\n Current count: \n 1</p>"));

  // Richtig: WaitForState() sorgt für neue Auswertung nach jedem Rendern!
  // cut.WaitForState(() => cut.Find("p").OuterHtml == ("<p>\n Current count: \n 1</p>"));

  // Wait: besser und richtig
   cut.WaitForAssertion(() => cut.Find("p").MarkupMatches(@"<p>Current count: 1 </p>"));
 }
}