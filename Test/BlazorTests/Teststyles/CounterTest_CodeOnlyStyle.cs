using System;

namespace BlazorTests.Teststyles;

public class CounterTest_CodeOnlyStyle : TestContext
{
 [Fact]
 public void CounterTest()
 {
  // Arrange: render the Counter.razor component
  var cut = RenderComponent<Counter>();

  // Act: find and click the <button> element to increment
  // the counter in the <p> element
  cut.Find("button").Click();

  cut.WaitForAssertion(() => cut.Find("p").MarkupMatches(@"<p>Current count: 1 </p>"));

  var html = cut.Find("p").OuterHtml;
  Assert.Equal("<p>\n Current count: 1</p>", html);

  cut.WaitForState(() => cut.Find("p").OuterHtml == ("<p>\n Current count: 1</p>"));







  // Assert: first find the <p> element, then verify its content
  cut.Find("p").MarkupMatches(@"<p> Current count: 1 </p>");


 }
}