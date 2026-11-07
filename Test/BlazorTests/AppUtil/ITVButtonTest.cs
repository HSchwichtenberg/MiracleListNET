using System;
using System.Linq;
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
public class ITVButtonTest : BunitContext
{
 public ITVButtonTest()
 {

  Services.AddSingleton<BlazorUtil>(new BlazorUtil(this.JSInterop.JSRuntime, new MockNavigationManager()));
 }
 [Fact]
 public void EmptyButton()
 {
  // Arrange
  var cut = Render<ITVButton>();

  // Assert that content of the paragraph shows counter at zero
  cut.Find("button").Text().MarkupMatches("");
 }

 [Fact]
 public void ButtonWithHTMLContent()
 {
  string content = "Test<b>button</b>";
  // Arrange
  var cut = Render<ITVButton>(p => p.AddChildContent(content));

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

  var cut = Render<ITVButton>(p => p.AddChildContent(content)
                                             .Add(x => x.AnimationSeconds, (byte)delay)
                                             .Add(x => x.onClick, onClickHandler)
                                             .AddUnmatched("style","background-color:red"));

  Assert.Equal(1, cut.RenderCount);

  IElement b = cut.Find("button");

  // Prüfe Inhalt
  b.Html().MarkupMatches(content); 
  // Prüfe gesamtes Tag
  b.OuterHtml.MarkupMatches($$"""<button data-toggle="tooltip" data-placement="bottom" style="background-color:red">{{content}}</button>""");
  // oder prüfe einzelne Attribute
  b.GetAttribute("style").MarkupMatches("background-color:red");
  b.GetAttribute("data-toggle").MarkupMatches("tooltip");
  // Button sollte nicht deaktiviert sein
  Assert.Null(b.GetAttribute("disabled"));

  // nun Klick auslösen
  b.Click();
  // oder:
  //var mea = new MouseEventArgs();
  //mea.CtrlKey = true;
  //b.TriggerEvent("onclick", mea);

  // Max 3 Sekunden warten bis Schaltfläche inaktiv ist
  cut.WaitForState(() => b.GetAttribute("disabled").IsNotNull(), new System.TimeSpan(0, 0, 3));

  // jetzt sollte die GIF-Animation zu sehen sein
  b.Html().MarkupMatches("""<img src="/_content/ITVisions.Blazor/img/ITVButtonProgress.gif" width="20" style="margin-right:8px;">""" + content);

  // oder prüfe einzelne Attribute
  //var o = b.ChildNodes[0]; // zur Diagnose
  (b.ChildNodes.Where(x => x.NodeName.Equals("img", StringComparison.InvariantCultureIgnoreCase)).Single() as IElement).GetAttribute("src").EndsWith("ITVButtonProgress.gif");

  // Max 5 Sekunden warten bis Schaltfläche wieder aktiv ist
  cut.WaitForState(() => b.GetAttribute("disabled").IsNull(), new System.TimeSpan(0, 0, 5));

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

  var cut = Render<ITVButton>(p => p.AddChildContent(content)
                                             .Add(x => x.AnimationSeconds, (byte)delay)
                                             .Add(x => x.onClick, onClickHandler)
                                             .Add(x => x.onError, onErrorHandler));

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
  JSInterop.Mode = JSRuntimeMode.Strict; // Strict mode configures the implementation to throw an exception if it is invoked with a method call it has not been set up to handle explicitly. This is useful if you want to ensure that a component only performs a specific set of IJSRuntime invocations.
  JSInterop.SetupVoid("ShowAlert", @"System.ApplicationException: Testfehler"); // genau diese Parameter erwarten
  JSInterop.SetupVoid("console.error", x=>x.Arguments[0].ToString().Contains("Testfehler") ); // ApplicationException soll im Text vorkommen
  // oder
  //JSInterop.SetupVoid("console.error", x => true); // alle Parameter ok

  int delay = 2;
  string content = "Test<b>button</b>";

  // Arrange: Unsere Aktion löst einen Fehler aus
  Action<MouseEventArgs> onClickHandler = (x) =>
  {
   throw new ApplicationException("Testfehler");
  };

  // wir liefern aber keine Fehlerhandler mit, d.h. die Komponente wird selbst einen Alert-Dialog machen
  var cut = Render<ITVButton>(p => p.AddChildContent(content)
                                             .Add(x => x.AnimationSeconds, (byte)delay)
                                             .Add(x => x.onClick, onClickHandler));
  Assert.Equal(1, cut.RenderCount);

  IElement b = cut.Find("button");
  b.Html().MarkupMatches(content);
  Assert.Null(b.GetAttribute("disabled"));

  b.Click();

  // Warten bis Schaltfläche wieder aktiv ist
  cut.WaitForState(() => b.GetAttribute("disabled").IsNull(), new System.TimeSpan(0, 0, 10));

  // hier geht es nicht ohne etwas Warten
  await Task.Delay(100);

  // Erwarte JS-aufrufe
  cut.WaitForState(() => this.JSInterop.Invocations.Count>0, new System.TimeSpan(0, 0, 10));

  // Prüfe JS-Aufruf
  Assert.Contains(JSInterop.Invocations["ShowAlert"], x => x.Arguments[0].ToString() == "System.ApplicationException: Testfehler");
 }
}