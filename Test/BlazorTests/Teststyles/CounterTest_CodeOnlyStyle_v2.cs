namespace BlazorTests.Teststyles;


/// <summary>
/// These tests are written entirely in C#.
/// Learn more at https://bunit.dev/docs/getting-started/writing-tests.html#creating-basic-tests-in-cs-files
/// </summary>
public class CounterTest_CodeOnly : BunitContext
{
 [Fact]
 public void CounterStartsAtZero()
 {
  // Arrange
  var cut = Render<Counter>();

  // Assert that content of the paragraph shows counter at zero
  cut.Find("p").MarkupMatches("<p>Current count: 0</p>");
 }

 [Fact]
 public void ClickingButtonIncrementsCounter()
 {
  // Arrange
  var cut = Render<Counter>();

  // Act - click button to increment counter
  cut.Find("button").Click();

  // Assert that the counter was incremented
  cut.Find("p").MarkupMatches("<p>Current count: 1</p>");
 }
}
