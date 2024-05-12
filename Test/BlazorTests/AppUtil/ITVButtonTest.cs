
using System;
using System.Threading.Tasks;
using BlazorTests.Mocks;
using ITVisions;
using ITVisions.Blazor;
using ITVisions.Blazor.Controls;
using Telerik.JustMock;

namespace BlazorTests.BlazorUtilTests;

/// <summary>
/// These tests are written entirely in C#.
/// Learn more at https://bunit.dev/docs/getting-started/writing-tests.html#creating-basic-tests-in-cs-files
/// </summary>
public class ITVButtonTest : TestContext
{
 public ITVButtonTest()
 {

  Services.AddSingleton<BlazorUtil>(new BlazorUtil(this.JSInterop.JSRuntime, new MockNavigationManager()));
 }
 [Fact]
 public void EmptyButton()
 {
  // Arrange
  var cut = RenderComponent<ITVButton>();

  // Assert that content of the paragraph shows counter at zero
  cut.Find("button").Text().MarkupMatches("");
 }

 [Fact]
 public void ButtonWithHTMLContent()
 {
  string content = "Test<b>button</b>";
  // Arrange
  var cut = RenderComponent<ITVButton>(p => p.AddChildContent(content));

  // Assert that the counter was incremented
  cut.Find("button").Html().MarkupMatches(content);
 }

 [Fact]
 public async Task ButtonClick()
 {
  //JSInterop.Mode = JSRuntimeMode.Loose;
  JSInterop.SetupVoid("console.error", @"BLAZOR: System.ApplicationException: Testfehler");
  JSInterop.SetupVoid("ShowAlert", @"System.ApplicationException: Testfehler");

  int delay = 2;
  string content = "Test<b>button</b>";
  // Arrange

  Action<MouseEventArgs> onClickHandler = (x) =>
  {

  };

  var cut = RenderComponent<ITVButton>(p => p.AddChildContent(content).Add(x => x.AnimationSeconds, (byte)delay).Add(x => x.onClick, onClickHandler));

  Assert.Equal(1, cut.RenderCount);

  IElement b = cut.Find("button");
  b.Html().MarkupMatches(content);
  Assert.Null(b.GetAttribute("disabled"));

  //b.Click();
  // oder:
  var mea = new MouseEventArgs();
  mea.CtrlKey = true;
  b.TriggerEvent("onclick", mea);

  // Warten bis Schaltfläche inaktiv ist
  cut.WaitForState(() => b.GetAttribute("disabled").IsNotNull(), new System.TimeSpan(0, 0, 10));

  // Warten bis Schaltfläche wieder aktiv ist
  cut.WaitForState(() => b.GetAttribute("disabled").IsNull(), new System.TimeSpan(0, 0, 10));

  // jetzt sollte die Animation fertig sein
  b.Html().MarkupMatches(content);
 }

 [Fact]
 public async Task ButtonClickWithErrorHandler()
 {
  int delay = 3;
  string content = "Test<b>button</b>";

  // Arrange
  Action<MouseEventArgs> onClickHandler = (x) =>
  {
   throw new ApplicationException("Testfehler");
  };

  Action<Exception> onErrorHandler = (ex) =>
  {
   Assert.IsType<ApplicationException>(ex);
   Assert.Equal("Testfehler", ex.Message);
  };

  var cut = RenderComponent<ITVButton>(p => p.AddChildContent(content).Add(x => x.AnimationSeconds, (byte)delay).Add(x => x.onClick, onClickHandler).Add(x => x.onError, onErrorHandler));

  Assert.Equal(1, cut.RenderCount);

  IElement b = cut.Find("button");
  b.Html().MarkupMatches(content);
  Assert.Null(b.GetAttribute("disabled"));

  b.Click();

  IElement b2 = cut.Find("button");
  var t = b2.Html();

  // Warten bis Schaltfläche deaktiviert
  cut.WaitForState(() => b.GetAttribute("disabled").IsNotNull(), new TimeSpan(0, 0, 5));
  // Warten bis Schaltflächeninhalt korrekt

  cut.WaitForState(() => b2.Html().StartsWith("<img"), new TimeSpan(0, 0, 5));

  // Animation sollte nun aktiv sein
  b.Html().MarkupMatches("<img src=\"/_content/ITVisions.Blazor/img/ITVButtonProgress.gif\" width=\"20\" style=\"margin-right:8px;\">" + content);

  // Warten bis Schaltfläche wieder aktiv ist
  cut.WaitForState(() => b.GetAttribute("disabled").IsNull(), new System.TimeSpan(0, 0, delay));

  // jetzt sollte die Animation fertig sein
  b.Html().MarkupMatches(content);

 }

 [Fact]
 public async Task ButtonClickWithoutErrorHandler()
 {
  //JSInterop.Mode = JSRuntimeMode.Loose;
  JSInterop.SetupVoid("console.error", @"BLAZOR: System.ApplicationException: Testfehler");
  JSInterop.SetupVoid("ShowAlert", @"System.ApplicationException: Testfehler");

  int delay = 2;
  string content = "Test<b>button</b>";
  // Arrange

  Action<MouseEventArgs> onClickHandler = (x) =>
  {
   throw new ApplicationException("Testfehler");
  };


  var cut = RenderComponent<ITVButton>(p => p.AddChildContent(content).Add(x => x.AnimationSeconds, (byte)delay).Add(x => x.onClick, onClickHandler));

  Assert.Equal(1, cut.RenderCount);

  IElement b = cut.Find("button");
  b.Html().MarkupMatches(content);
  Assert.Null(b.GetAttribute("disabled"));

  b.Click();

  // Warten bis Schaltfläche wieder aktiv ist
  cut.WaitForState(() => b.GetAttribute("disabled").IsNull(), new System.TimeSpan(0, 0, 10));
  cut.Render();

  // jetzt sollte die Animation fertig sein
  b.Html().MarkupMatches(content);

 }
}
