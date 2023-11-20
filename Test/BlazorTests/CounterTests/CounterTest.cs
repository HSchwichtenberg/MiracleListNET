using System;

namespace BlazorTests.CounterTests;

public class CounterTests : TestContext
{
 [Fact]
 public void CounterTest()
 {
  // Arrange
  var cut = RenderComponent<Counter>();

  // Act
  cut.Find("button").Click();

  // Assert: Ungünstig und falsch!!!
  //Assert.Equal("<p>Current count: 1</p>", cut.Find("p").OuterHtml);
  // Assert: Ungünstig, aber richtig!!!
  Assert.Equal("<p>\n Current count: \n 1</p>", cut.Find("p").OuterHtml);

  // Zur Diagnose: Haltepunkt nach der Zuweisung!
  var html = cut.Find("p").OuterHtml;
  Assert.Equal("<p>\n Current count: \n 1</p>", html);

  // Assert: besser und richtig!
  cut.Find("p").MarkupMatches(@"<p> Current count: 1 </p>");
 }


 /// <summary>
 /// Wait bei Counter nicht notwendig, da Komponente nicht asynchron arbeitet!
 /// </summary>
 [Fact]
 public void CounterTestmitWait()
 {
  // Arrange
  var cut = RenderComponent<CounterAsync>();

  // Act
  cut.Find("button").Click();

  // Wait: Ungünstig, aber richtig
  var html = cut.Find("p").OuterHtml; // zur Diagnose
  cut.WaitForState(() => cut.Find("p").OuterHtml == ("<p>\n Current count: \n 1</p>"));
  // Achtung: Das wäre falsch, da nur einmal ausgewertet
  // cut.WaitForState(() => html == ("<p>\n Current count: \n 1</p>"));
  // Neue Auswertung bei WaitForState() nach jedem Rendern!

  // Wait: besser und richtig
  cut.WaitForAssertion(() => cut.Find("p").MarkupMatches(@"<p>Current count: 1 </p>"));
 }
}