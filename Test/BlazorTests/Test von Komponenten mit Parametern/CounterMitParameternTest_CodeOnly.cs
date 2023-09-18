namespace BlazorTests.Test_von_Komponenten_mit_Parametern;

/// <summary>
/// These tests are written entirely in C#.
/// Learn more at https://bunit.dev/docs/getting-started/writing-tests.html#creating-basic-tests-in-cs-files
/// </summary>
public class CounterMitParameternTest_CodeOnly : TestContext
{
 [Fact]
 public void CounterStartsAtZero()
 {
  // Arrange
  var cut = RenderComponent<CounterMitParametern>();

  // Assert that content of the paragraph shows counter at zero
  cut.Find("p").MarkupMatches("<p>Current count: 0</p>");
 }

 [Fact]
 public void ClickingButtonIncrementsCounter()
 {
  // Arrange
  var cut = RenderComponent<CounterMitParametern>(p => p.Add(x => x.Increment, 2).Add(x => x.CurrentCount, 40));

  // Act - click button to increment counter
  cut.Find("button").Click();

  // Assert that the counter was incremented
  cut.Find("p").MarkupMatches("<p>Current count: 42</p>");
 }
}
