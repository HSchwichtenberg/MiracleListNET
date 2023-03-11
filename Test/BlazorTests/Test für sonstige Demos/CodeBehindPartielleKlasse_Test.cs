using System;
using Xunit;
using Bunit;
using Samples.Komponentendateien;
using Microsoft.Extensions.DependencyInjection;
using ITVisions.Blazor;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using System.Linq;
using System.Globalization;
using BlazorTests;
using BlazorTests.Mocks;

namespace BlazorTests
{

 public class CodeBehindPartielleKlasse_Test : TestContext
 {

  //public MockJSRuntimeInvokeHandler jsMock { get; set; }
  public CodeBehindPartielleKlasse_Test()
  {
   Services.AddSingleton<NavigationManager>(new MockNavigationManager(Uri));
   //jsMock = Services.AddMockJSRuntime(JSRuntimeMockMode.Loose); // loose is default
   Services.AddSingleton<BlazorUtil>(new BlazorUtil(this.JSInterop.JSRuntime, new MockNavigationManager(Uri), null));

   JSInterop.Mode = JSRuntimeMode.Loose; // Loose mode configures the implementation to just return the default value when it receives an invocation that has not been explicitly set up
  }

  string Uri = "http://myserver/CodeBehindPartielleKlasse";

  // https://github.com/egil/razor-components-testing-library/wiki/C%23-test-examples
  [Fact]
  public void AddTest()
  {

   var cut = RenderComponent<CodeBehindPartielleKlasse>();

   cut.Markup.Contains(@"<h2>Code-Behind mit partieller Klasse</h2>");
   cut.Markup.Contains(@"Sum: 0");

   // 1. Klick auf Add
   cut.Find("button").Click();

   // Es sollte in JavaScript nun SetTitle aufgerufen worden sein
   Assert.Equal("3,57", this.JSInterop.VerifyInvoke("SetTitle").Arguments.Single().ToString());
   // Eine der Log-Aufrufe sollte die aktuelle URL beinhalten
   Assert.Contains(this.JSInterop.Invocations["console.info"], x => x.Arguments[0].ToString() == "BLAZOR: LogURL(): " + Uri);

   // Prüfe URL ist gleich geblieben
   Assert.Equal(Uri, cut.Instance.NavigationManager.Uri);

   // Es sollte zwei Änderungen im DOM geben
   Assert.Equal(2, cut.GetChangesSinceFirstRender().Count);
   // Prüfen der Änderungen
   // a) Volltextsuche 
   Assert.Contains("Sum: 3,57", cut.Markup);
   // b) markup-Vergleich
   cut.Find("#x").MarkupMatches(@"<input id=""x"" type=""number"" value=""3.57"">");
   // c) Prüfen eines HTML-Attributes
   Assert.Equal("3.57", cut.Find("#x").GetAttribute("value"));

   // 2. Klick auf Add
   cut.Find("button").Click();
   var invokes = this.JSInterop.Invocations;
   Assert.Contains("Sum: 5,91", cut.Markup);
   cut.Find("#x").MarkupMatches(@"<input id=""x"" type=""number"" value=""5.91"">");
   Assert.Equal("5.91", cut.Find("#x").GetAttribute("value"));


   // Wird komplettes DOM ausgeben im Fehler: cut.MarkupMatches("2");
   // Sieht man beim Debugging im "Output"-Fenster:
   Console.WriteLine(cut.Markup);
   Trace.WriteLine(cut.Markup);


  }

  [Fact]
  public void ParameterTest()
  {
   decimal erwartet = 3.0m;
   string erwartetDOMFormat = erwartet.ToString(new CultureInfo("en-us"));

   var cut = RenderComponent<CodeBehindPartielleKlasse>(("X", 1.0m), ("Y", 2.0m));

   cut.Markup.Contains(@"<h2>Code-Behind mit partieller Klasse</h2>");
   cut.Markup.Contains(@"Sum: 0");

   // 1. Klick auf Add
   cut.Find("button").Click();

   // Es sollte in JavaScript nun SetTitle aufgerufen worden sein
   Assert.Equal(erwartet.ToString(), this.JSInterop.VerifyInvoke("SetTitle").Arguments.Single().ToString());
   // Eine der Log-Aufrufe sollte die aktuelle URL beinhalten
   Assert.Contains(this.JSInterop.Invocations["console.info"], x => x.Arguments[0].ToString() == "BLAZOR: LogURL(): " + Uri);

   // Prüfe URL ist gleich geblieben
   Assert.Equal(Uri, cut.Instance.NavigationManager.Uri);

   // Es sollte zwei Änderungen im DOM geben
   Assert.Equal(2, cut.GetChangesSinceFirstRender().Count);
   // Prüfen der Änderungen
   // a) Volltextsuche 
   Assert.Contains("Sum: " + erwartet, cut.Markup);
   // b) markup-Vergleich
   cut.Find("#x").MarkupMatches(@$"<input id=""x"" type=""number"" value=""{erwartetDOMFormat}"">");
   // c) Prüfen eines HTML-Attributes
   Assert.Equal(erwartetDOMFormat, cut.Find("#x").GetAttribute("value"));


  }
 }
}