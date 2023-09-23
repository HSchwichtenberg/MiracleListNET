using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Blazored.LocalStorage;
using BlazorTests;
using BlazorTests.Mocks;
using Bunit;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Samples.Komponentendateien;
using Telerik.JustMock; // Mocking mit Just Mock von Progress Telerik
using Web;
using Xunit;
using static Bunit.ComponentParameterFactory;
// Testen der BD-Lösung
//using Pages = BD.Web.Pages;
// oder z.B. Blazor Server
using Pages = Web.Pages;

namespace MiracleListTests
{
 public class MiracleListAboutTest : TestContext
 {
  string UriBefore = "http://myserver/Login";

  //public  MockJSRuntimeInvokeHandler jsMock { get; set; }

  /// <summary>
  /// Setup the test: DI of mocking classes
  /// </summary>
  public MiracleListAboutTest()
  {

   Services.AddSingleton<IHttpContextAccessor>(Mock.Create<IHttpContextAccessor>());
   Services.AddSingleton<IServer>(Mock.Create<IServer>());
   // oder:
   //Services.AddSingleton<IHttpContextAccessor>(new MockHttpContextAccessor());

   Services.AddSingleton<NavigationManager>(new MockNavigationManager(UriBefore));
   Services.AddSingleton<IWebHostEnvironment>(Mock.Create<IWebHostEnvironment>());
   //jsMock = Services.AddMockJSRuntime(JSRuntimeMode.Loose); // loose is default
   Services.AddSingleton<BlazorUtil>(new BlazorUtil(this.JSInterop.JSRuntime, new MockNavigationManager(UriBefore)));

   //Services.AddSingleton<BD.AppState>();

   JSInterop.Mode = JSRuntimeMode.Loose; // Loose mode configures the implementation to just return the default value when it receives an invocation that has not been explicitly set up
  }

  [Fact]
  public void HeadlineOK()
  {
   // Diese Komponente laden
   var cut = RenderComponent<Pages.About>();
   Console.WriteLine(cut.Markup);
   Debug.WriteLine(cut.Markup);
   Trace.WriteLine(cut.Markup);

   // Headline vorhanden?
   var e = cut.Find("h2");
   Assert.NotNull(e);

   // Teste Text in Headline
   e.MarkupMatches(@"<h2>Technische Daten</h2>");

   // Teste weiteren Text
   Assert.Contains(".NET", cut.Markup);
   Assert.Contains("Browser", cut.Markup);

  }
 }
}