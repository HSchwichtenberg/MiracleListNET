using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using BlazorTests.Mocks;
using Bunit;
using Bunit.TestDoubles;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiracleList;
using Telerik.JustMock;
using Web;
using Xunit;

// Testen der BD-Lösung
//using Pages = BD.Web.Pages;
// oder z.B. Blazor Server
using Pages = Web.Pages;

namespace MiracleListTests
{
 // DEMO: 71. Razor Component Tests mit BUnit
 public class MiracleListLoginTest : TestContext
 {

  //myserver ist beliebiges Wort. Es wird kein HTTP-Request dahin gesendet!
  string UriBefore = "http://myserver/login";
  string UriAfter = "http://myserver/main";

  //public MockJSRuntimeInvokeHandler jsMock { get; set; }

  /// <summary>
  /// Setup the test: DI of mocking classes
  /// </summary>
  public MiracleListLoginTest()
  {

  }
  TestContext ctx = new TestContext();

  private IRenderedComponent<MLBlazorRCL.Login.Login> ArrangeAndAct(string name, string kennwort)
  {

   ctx.JSInterop.Mode = JSRuntimeMode.Loose; // https://bunit.dev/docs/test-doubles/emulating-ijsruntime.html
   ctx.Services.AddSingleton<IWebHostEnvironment>(new MockWebHostEnvironment());
   ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager(UriBefore));
   //jsMock = Services.AddMockJSRuntime(JSRuntimeMockMode.Loose); // loose is default
   ctx.Services.AddSingleton<BlazorUtil>(new BlazorUtil(this.JSInterop.JSRuntime, new MockNavigationManager(UriBefore), null));
   ctx.Services.AddScoped<IMLAuthenticationStateProvider, MockAuthenticationStateProvider>();
   ctx.Services.AddScoped<AuthenticationStateProvider, MockAuthenticationStateProvider>();
   IConfiguration c = Mock.Create<IConfiguration>();
   ctx.Services.AddSingleton<IConfiguration>(c);
   ctx.Services.AddSingleton<IHostEnvironment>(Mock.Create<IHostEnvironment>());
   ctx.Services.AddSingleton<AppState>();
   //Services.AddSingleton<BD.AppState>(); // nur bei BD
   ctx.Services.AddBlazoredLocalStorage(); // ggf. ersetzen durch Mock. Bisher nicht notwendig :-)
   IMiracleListProxy mockProxy = Mock.Create<IMiracleListProxy>();
   ctx.Services.AddSingleton(mockProxy);
   ctx.Services.AddScoped<IAppState, AppState>();
   JSInterop.Mode = JSRuntimeMode.Loose; // Loose mode configures the implementation to just return the default value when it receives an invocation that has not been explicitly set up

   //var state = Mock.Create<Task<AuthenticationState>>();

   var cut = ctx.RenderComponent<MLBlazorRCL.Login.Login>(); // parameters => parameters.Add(p => p.authenticationStateTask, state));

   // Prüfung 1: Richtiges Fenster geladen?
   cut.Find("h2").MarkupMatches(@"<h2 class='Headline'>Benutzeranmeldung</h2>");
   // Alternative:
   Assert.Equal("Headline", cut.Find("h2").ClassName);
   Assert.Equal("Benutzeranmeldung", cut.Find("h2").TextContent);

   // Setze Werte
   cut.Find("#username").Change(name);
   cut.Find("#password").Change(kennwort);
   cut.Find("#login").Click();
   #region SO NICHT: Das geht nicht, die Werte landen nicht in der Component!
   //cut.Find("#name").SetAttribute("value", name);
   //cut.Find("#password").SetAttribute("value", kennwort);
   // oder  (cut.Find("#password") as AngleSharp.Html.Dom.IHtmlInputElement).Value = "xxx";
   #endregion

   #region für debugging des Tests 
   //cut.MarkupMatches((cut.Find("#password") as AngleSharp.Html.Dom.IHtmlInputElement).Value);
   #endregion
   return cut;
  }

  [Fact] // Pattern "Arrange-Act-Assert"
  public void LoginOK()
  {
   string name = "testuser " + Guid.NewGuid();
   string kennwort = Guid.NewGuid().ToString();

   var authContext = ctx.AddTestAuthorization();
   authContext.SetAuthorized(name);
   var stateProvider = Services.GetService<AuthenticationStateProvider>();

   // Arrange and Act
   IRenderedComponent<MLBlazorRCL.Login.Login> cut = ArrangeAndAct(name, kennwort);

   // Prüfung 1: Konsolenausgabe
   Assert.Contains(this.JSInterop.Invocations["console.info"], x => x.Arguments[0].ToString() == "BLAZOR: Login.DoLogin: Login OK!");

   // Prüfung 2: Ist Navigation erfolgt?
   Assert.Equal(UriAfter, cut.Instance.NavigationManager.Uri);
  }

  [Fact]
  public void LoginError()
  {
   string name = "testusers " + DateTime.Now;
   string kennwort = "";

   // Arrange and Act
   IRenderedComponent<MLBlazorRCL.Login.Login> cut = ArrangeAndAct(name, kennwort);

   // Prüfung 1: Konsolenausgabe
   var console = this.JSInterop.Invocations["console.info"];
   Assert.Contains(console, x => x.Arguments[0].ToString() == "BLAZOR: Login.DoLogin: Login Error!");

   // Prüfung 2: Konsolenausgabe
   Assert.Contains("Login Error: not ok", cut.Find("#errorMsg").TextContent);

   // Prüfung 3: Navigation
   Assert.Equal(UriBefore, cut.Instance.NavigationManager.Uri);
  }


 }
}