using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Blazored.LocalStorage;
using BlazorTests.Mocks;
using Bunit;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiracleList;
using MLBlazorRCL.Others;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using Web;
using Xunit;

// Testen der BD-Lösung
//using Pages = BD.Web.Pages;
// oder z.B. Blazor Server
// using Pages = Web.Pages;

namespace MiracleListTests;

public class MiracleTaskGridTest : TestContext {
 string UriBefore = "http://myserver/Login";

 TestStore store = new TestStore(); 

 /// <summary>
 /// Setup the test: DI of mocking classes
 /// </summary>
 public MiracleTaskGridTest() {
  Services.AddSingleton<IWebHostEnvironment>(new MockWebHostEnvironment());
  Services.AddSingleton<NavigationManager>(new MockNavigationManager(UriBefore));
  //jsMock = Services.AddMockJSRuntime(JSRuntimeMockMode.Loose); // loose is default
  Services.AddSingleton<BlazorUtil>(new BlazorUtil(this.JSInterop.JSRuntime, new MockNavigationManager(UriBefore), null));
  Services.AddSingleton<AuthenticationStateProvider, MockAuthenticationStateProvider>();
  Services.AddBlazoredLocalStorage(); // ggf. ersetzen durch Mock. Bisher nicht notwendig :-)
  JSInterop.Mode = JSRuntimeMode.Loose; // Loose mode configures the implementation to just return the default value when it receives an invocation that has not been explicitly set up
  Services.AddSingleton(new HttpClient()); // aus der Projektvorlage von Microsoft!
  Services.AddSingleton<IHostEnvironment>(Mock.Create<IHostEnvironment>());
  // Mocking des Proxies mit Teststore
  IMiracleListProxy mockProxy = Mock.Create<IMiracleListProxy>();
  Mock.Arrange(() => mockProxy.CategorySetAsync(Arg.IsAny<string>())).Returns(Task.FromResult(store.Data));
  Mock.Arrange(() => mockProxy.TaskSetAsync(Arg.IsAny<int>(), Arg.IsAny<string>())).Returns((int cat) => Task.FromResult(store.Data.FirstOrDefault(x=>x.CategoryID == cat).TaskSet));

  // var x = mockProxy.CategorySetAsync("").Result;

  Services.AddSingleton(mockProxy);

  IConfiguration c = Mock.Create<IConfiguration>();
  Services.AddSingleton<IConfiguration>(c);
  Services.AddSingleton<IAppState, AppState>();

  DA.Context.ConnectionString = ""; // In Memory-DB!
 }

 private IRenderedComponent<TaskGrid> Prepare() {

  // Razor-Komponente laden
  var cut = RenderComponent<TaskGrid>();
  Console.WriteLine(cut.Markup); // für Test-Debugging
  // Prüfung, ob alle erwarteten Aufgaben geladen
  Assert.Equal("All Tasks View: 15 Tasks", cut.Find("h3").TextContent);
  return cut;
 }

 [Fact]
 public void FormatAndContent() {
  IRenderedComponent<TaskGrid> cut = Prepare();

  // Tabelleninhalt suchen
  var tbody = cut.Find("tbody");
  Assert.NotNull(tbody);
  Assert.Equal(10, tbody.Children.Count());    // 10 Zeilen?

  foreach (IElement row in tbody.Children) {
   //Importance-Spalte
   var spalteImportance = row.Children[3];
   //Gerenderte HTML-Struktur ist: <td style = "width:5%" ><span class="rz-cell-data" title=""><span class="badge badge-important" title="Wichtigkeit: A">A</span>
   var inhalt3 = spalteImportance.GetElementsByTagName("span")[0].GetElementsByTagName("span")[0];
   // Importance-Spalte: Prüfung Inhalt und Layout
   Assert.Equal("badge badge-important", inhalt3.ClassName);
   Assert.True(inhalt3.TextContent.Contains("A") || inhalt3.TextContent.Contains("B") || inhalt3.TextContent.Contains("C"));

   // Due-Spalte
   var spalteDue = row.Children[4];
   var inhaltDue = spalteDue.GetElementsByTagName("div")[0];
   // Due-Spalte: Prüfung Inhalt
   Assert.Contains("Due", inhaltDue.TextContent);
   // Due-Spalte: Prüfung der Farbe
   if (inhaltDue.TextContent.Contains("Due since")) Assert.Equal("color:red", inhaltDue.Attributes["style"].Value);
   if (inhaltDue.TextContent.Contains("Due since")) Assert.Contains("color: rgba(255, 0, 0, 1)", inhaltDue.ComputeCurrentStyle().CssText);
  }
 }

 [Fact]
 public void EditTitle() {
  IRenderedComponent<TaskGrid> cut = Prepare();
  var tbody = cut.Find("tbody");

  // Spalte in Edit-Mode versetzen
  TestUtil.ClickCommand(tbody, 0, 0, "i", "edit"); // <button><i>edit...</i></button>

  // Prüfe bisherigen Wert
  IHtmlInputElement textBox = TestUtil.GetCell(tbody, 0, 2, "input") as IHtmlInputElement;
  Assert.Equal("Testaufgabe 1", textBox.Value);

  // Setze neuen Wert
  var newValue = Guid.NewGuid().ToString();
  textBox.Change(newValue);

  // Speichern
  TestUtil.ClickCommandButtonByTitle(tbody, 0, 0, "button", "Save");

  // Prüfen anhand aktuellem Wert in Teststore

  var taskSet = store.Data[0].TaskSet; // Erste Kategorie, erste Aufgabe
  Assert.Equal(newValue, taskSet[0].Title);
 }
}