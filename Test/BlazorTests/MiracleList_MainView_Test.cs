using System;
using System.Collections.Generic;
using System.Linq;
using BlazorContextMenu;
using BlazorContextMenu.Services;
using Blazored.LocalStorage;
using Blazored.Toast.Services;
using BlazorTests.Mocks;
using BO;
using ITVisions;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MiracleList;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using Web;

// Testen der BD-Lösung
//using Pages = BD.Web.Pages;
// oder z.B. Blazor Server

namespace MiracleListTests;

// DEMO: 71. Razor Component Tests mit BUnit
public class MiracleList_MainView_Test : TestContext
{
 IMiracleListProxy mockProxy;

 /// <summary>
 /// Setup the test: DI of mocking classes
 /// </summary>
 public MiracleList_MainView_Test()
 {
  #region Blazor-Dienste
  this.JSInterop.Mode = JSRuntimeMode.Loose; // https://bunit.dev/docs/test-doubles/emulating-ijsruntime.html
  this.Services.AddSingleton<IWebHostEnvironment>(new MockWebHostEnvironment());
  //this.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
  this.Services.AddSingleton<BlazorUtil>(new BlazorUtil(this.JSInterop.JSRuntime, new MockNavigationManager(), null));
  this.Services.AddScoped<AuthenticationStateProvider, MockAuthenticationStateProvider>();
  IConfiguration c = Mock.Create<IConfiguration>();
  this.Services.AddSingleton<IConfiguration>(c);
  this.Services.AddSingleton<IHostEnvironment>(Mock.Create<IHostEnvironment>());
  #endregion

  #region Dienste für fremde Zusatzkomponenten
  this.Services.AddSingleton<IToastService>(Mock.Create<IToastService>());
  this.Services.AddSingleton<IInternalContextMenuHandler>(Mock.Create<IInternalContextMenuHandler>());
  var bcms = new BlazorContextMenuSettings();
  this.Services.AddSingleton<BlazorContextMenuSettings>(bcms);
  this.Services.AddSingleton<IMenuTreeTraverser>(Mock.Create<IMenuTreeTraverser>());
  this.Services.AddSingleton<ContextMenu>(Mock.Create<ContextMenu>());
  this.Services.AddSingleton<IContextMenuStorage>(Mock.Create<IContextMenuStorage>());
  #endregion

  #region Dienste für eigene Dienste
  this.Services.AddScoped<IMLAuthenticationStateProvider, MockAuthenticationStateProvider>();
  this.Services.AddSingleton<AppState>();
  //Services.AddSingleton<BD.AppState>(); // nur bei BD
  this.Services.AddBlazoredLocalStorage(); // ggf. ersetzen durch Mock. Bisher nicht notwendig :-)
  mockProxy = Mock.Create<IMiracleListProxy>();
  this.Services.AddSingleton(mockProxy);
  this.Services.AddScoped<IAppState, AppState>();
  #endregion

  // neuer Benutzer mit Standardkategorien
  string name = "testuser " + Guid.NewGuid();
  var authContext = this.AddTestAuthorization();
  authContext.SetAuthorized(name);
 }

 [Fact] // Pattern "Arrange-Act-Assert"
 public void MainTest()
 {
  #region Testdaten
  var categorySet = new List<Category>() { new Category() { Name = "Test1", CategoryID = 1 }, new Category() { Name = "Test2", CategoryID = 2 } };
  var taskSet = new List<BO.Task>() { new BO.Task() { Title = "Task1", TaskID = 1 }, new BO.Task() { Title = "Task2", TaskID = 2 }, new BO.Task() { Title = "Task3", TaskID = 3 } };
  #endregion

  #region Einrichten des Mocks für das Backened
  mockProxy.Arrange(x => x.CategorySetAsync(null)).ReturnsAsync(categorySet).MustBeCalled();
  mockProxy.Arrange(x => x.TaskSetAsync(1, null)).ReturnsAsync(taskSet).MustBeCalled();
  mockProxy.Arrange(x => x.TaskSetAsync(2, null)).ReturnsAsync(taskSet).MustBeCalled();
  mockProxy.Arrange(x => x.TaskSetAsync(Arg.IsInRange(3, 13, RangeKind.Inclusive), null)).DoInstead((int categoryID) =>
  {
   var c = categorySet.FirstOrDefault(x => x.CategoryID == categoryID);
   if (c.TaskSet.IsNull()) c.TaskSet = new List<BO.Task>();
  }
  ).ReturnsAsync((int categoryID) => categorySet.FirstOrDefault(x => x.CategoryID == categoryID).TaskSet);
  mockProxy.Arrange(x => x.CreateCategoryAsync("", null)).IgnoreArguments().DoInstead((string name) =>
  {
   categorySet.Add(new Category() { CategoryID = categorySet.Max(x => x.CategoryID) + 1, Name = name });
  });
  mockProxy.Arrange(x => x.CreateTaskAsync(Arg.IsAny<BO.Task>(), null)).DoInstead((BO.Task t) =>
  {
   var c = categorySet.FirstOrDefault(x => x.CategoryID == t.CategoryID);
   if (c.TaskSet.IsNull()) c.TaskSet = new List<BO.Task>();
   c.TaskSet.Add(new Task());
  });
  #endregion

  // Komponente rendern
  IRenderedComponent<MLBlazorRCL.MainView.Main> cut = RenderComponent<MLBlazorRCL.MainView.Main>();

  // Stimmt die angezeigte Anzahl der Kategorien und Aufgaben?
  cut.WaitForState(() => cut.Find("#categoryCount").Text() == "2");
  Assert.Equal("3", cut.Find("#taskCount").Text());

  // Anzahl der li-Elemente in der Kategorienliste (eins mehr, weil "Suche" dabei ist)
  var col1List = cut.Find("#col1 ol");
  Assert.Equal(categorySet.Count + 1, col1List.ChildNodes.Count());

  // 1 bis n-1 sind <li> Elemente (Letztes Element ist <input>)
  Assert.All(col1List.ChildNodes.Take(categorySet.Count), x => Assert.Equal("li", x.NodeName, true));
  Assert.Equal("input", col1List.ChildNodes.Last().NodeName, true);

  #region 10x Neue Kategorie ergänzen
  for (int i = 0; i < 10; i++)
  {
   Console.WriteLine("Kategorie anlegen: " + i);

   Assert.Equal("input", col1List.ChildNodes.ElementAt(categorySet.Count).NodeName, true);
   var inputNewCategoryName = cut.Find("input[name=newCategoryName]");
   inputNewCategoryName.Change("Neue Kategorie");
   inputNewCategoryName.KeyUp("Enter");

   cut.WaitForState(() => cut.Find("#categoryCount").Text() == categorySet.Count.ToString());

   // Neue Kategorie als aktuelle wählen

   // Das kann Probleme machen:  "Bunit.Rendering.UnknownEventHandlerIdException : There is no event handler with ID '44' associated with the 'onclick' event in the current render tree."
   //var liElement = cut.FindAll("#col1 ol li").ElementAt(i + 2);
   //liElement.Click();
   // Lösung: "This ensures that there are no changes to the DOM between Find method and the Click method calls."
   cut.InvokeAsync(() => cut.FindAll("#col1 ol li").ElementAt(i + 2).Click());

   // In der neuen Kategorie gibt es erstmal keine Aufgaben
   cut.WaitForState(() => cut.Find("#taskCount").Text() == "0");

   // Nun eine Aufgabe ergänzen
   var inputnewTaskTitle = cut.Find("input[name=newTaskTitle]");
   inputnewTaskTitle.Change("Neue Aufgabe");
   inputnewTaskTitle.KeyUp("Enter");

   // Nun sollte eine Aufgabe da sein
   cut.WaitForState(() => cut.Find("#taskCount").Text() == "1");

   inputnewTaskTitle.Change("Zweite Aufgabe");
   inputnewTaskTitle.KeyUp("Enter");

   // Nun sollte zwei Aufgaben da sein
   cut.WaitForState(() => cut.Find("#taskCount").Text() == "2");
  }

  #endregion
 }
}

