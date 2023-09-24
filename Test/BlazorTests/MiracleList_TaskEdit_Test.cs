using System;
using System.Linq;
using System.Security.Claims;
using BL;
using Blazored.LocalStorage;
using BlazorTests.Mocks;
using Bunit;
using Bunit.TestDoubles;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MLBlazorRCL.MainView;
using Xunit;

// Testen der BD-Lösung
//using Pages = BD.Web.Pages;
// oder z.B. Blazor Server
using Pages = Web.Pages;

namespace MiracleListTests
{

 public class MiracleTaskEditTest : TestContext
 {
  string UriBefore = "http://myserver/Login";

  /// <summary>
  /// Setup the test: DI of mocking classes
  /// </summary>
  public MiracleTaskEditTest()
  {
   Services.AddSingleton<IWebHostEnvironment>(new MockWebHostEnvironment());
   Services.AddSingleton<NavigationManager>(new MockNavigationManager(UriBefore));
   //jsMock = Services.AddMockJSRuntime(JSRuntimeMockMode.Loose); // loose is default
   Services.AddSingleton<BlazorUtil>(new BlazorUtil(this.JSInterop.JSRuntime, new MockNavigationManager(UriBefore), null));
   Services.AddSingleton<AuthenticationStateProvider, MockAuthenticationStateProvider>();
   Services.AddBlazoredLocalStorage(); // ggf. ersetzen durch Mock. Bisher nicht notwendig :-)
   JSInterop.Mode = JSRuntimeMode.Loose; // Loose mode configures the implementation to just return the default value when it receives an invocation that has not been explicitly set up

   DA.Context.ConnectionString = ""; // In Memory-DB!
  }

  /// <summary>
  /// Vorbereiten der In-Mem-DB und der Authentifizierung
  /// </summary>
  /// <returns></returns>
  private BO.Task Prepare()
  {
   string test_userName = Guid.NewGuid().ToString();

   // DB einrichten (in Memory)
   var um = new UserManager(test_userName.ToString(), "geheim");
   var tm = new TaskManager(um.CurrentUser.UserID);
   var cm = new CategoryManager(um.CurrentUser.UserID);
   var task = new BO.Task() { Title = "Test" }; // weil https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-6.0/whatsnew#in-memory-database-validate-required-properties-are-not-null
   task.CategoryID = cm.GetCategorySet().ElementAt(0).CategoryID;
   tm.CreateTask(task);

   // Auth. faken
   var authContext = this.AddTestAuthorization();
   authContext.SetAuthorized("testuser");
   authContext.SetClaims(
       new Claim(ClaimTypes.Sid, um.CurrentUser.UserID.ToString()),
       new Claim("Test", "true"));
   return task;
  }

  [Fact]
  public void SaveTest()
  {
   BO.Task task = Prepare();

   // Diese Komponente laden
   var cut = RenderComponent<TaskEdit>(
    p => p
     .Add(x => x.Task, task)
     .Add(x => x.TaskHasChanged, (saved) => { Assert.True(saved); })
    );

   Assert.Contains("date", cut.Markup);

   var neuerTitle = "Title " + Guid.NewGuid().ToString();
   cut.Find("#tasktitle").Change(neuerTitle);
   cut.Find("#save").Click();

   Assert.Equal(neuerTitle, task.Title);
  }

  [Fact]
  public void CancelTest()
  {
   BO.Task task = Prepare();

   // Diese Komponente laden
   var cut = RenderComponent<TaskEdit>(
    p => p
     .Add(x => x.Task, task)
     .Add(x => x.TaskHasChanged, (saved) => { Assert.False(saved); })
    );

   Assert.Contains("date", cut.Markup);

   var neuerTitle = "Title " + Guid.NewGuid().ToString();
   cut.Find("#tasktitle").Change(neuerTitle);
   cut.Find("#cancel").Click();

   Assert.Equal(neuerTitle, task.Title);
  }
 }
}