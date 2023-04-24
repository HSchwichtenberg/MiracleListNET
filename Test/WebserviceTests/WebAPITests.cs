using System.Diagnostics;
using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using MiracleList;

namespace WebserviceTests;

public class WebAPITests
{
 public WebAPITests()
 {
  Proxy = new MiracleListProxy(new HttpClient());
  Proxy.BackendUrl = "http://miraclelistbackend.azurewebsites.net"; // "http://localhost:8889/";
 }

 public MiracleListProxy Proxy { get; }


 [Fact]
 private async Task<LoginInfo> Login()
 {
  string username = Guid.NewGuid().ToString();
  string password = username;
  string clientID = "11111111-1111-1111-1111-111111111111";

  var loginInfo = new LoginInfo() { ClientID = clientID, Username = username, Password = password };

  var loginResult = await Proxy.LoginAsync(loginInfo);

  Assert.True(String.IsNullOrEmpty(loginResult.Message), "Login Error " + loginResult.Message); // OK
  Assert.Equal(username, loginResult.Username);
  Assert.False(String.IsNullOrEmpty(loginResult.Token));
  return loginResult;
 }

 [Fact]
 public async Task CategoriesWithTasks()
 {
  LoginInfo loginResult = await Login();

  var categorySet = await Proxy.CategorySetAsync(loginResult.Token);

  // Prüfung Anzahl der automatisch angelegten Kategorien und Aufgaben 
  Assert.Equal(4, categorySet.Count);
  Assert.Equal(12, categorySet.Select(c => c.TaskSet.Count).Sum());

 }


 [Fact]
 public async Task ChangeTask()
 {
  var neuerTitel = DateTime.Now.ToString();
  LoginInfo loginResult = await Login();

  var categorySet = await Proxy.CategorySetAsync(loginResult.Token);

  var task = categorySet[0].TaskSet[0];

  task.Title = neuerTitel;
  var taskChanged = await Proxy.ChangeTaskAsync(task, loginResult.Token);
  Assert.Equal(task.TaskID, taskChanged.TaskID);
  Assert.Equal(neuerTitel, taskChanged.Title);

  var taskReloaded = await Proxy.TaskAsync(task.TaskID, loginResult.Token);
  Assert.Equal(task.TaskID, taskReloaded.TaskID);
  Assert.Equal(neuerTitel, taskChanged.Title);

 }

}