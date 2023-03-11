using BL;
using BO;
using System;
using Xunit;


// XUNIT: https://xunit.github.io/docs/getting-started-dotnet-core.html

namespace UnitTests
{

 public class ContextManagerTest
 {

  public ContextManagerTest()
  {
   Util.Init();
  }

  [SkippableFact]
  [Trait("Category", "Integration")]
  public void CheckConnectionString()
  {

   if (Util.IsInMemory)
   {
    Assert.Same("", DA.Context.ConnectionString);

   }
   else
   {
    Assert.True(DA.Context.ConnectionString.Length > 0);
   }
  }
 }

 public class UserManagerTest
 {

  public UserManagerTest()
  {
   Util.Init();
  }

  [SkippableFact]
  [Trait("Category", "Integration")]
  public void GetLatestUsersTest()
  {
   Skip.If(Util.IsInMemory, "Only runs as integration test as the InMem-DB does not support SQL!");
   var um = new UserManager("test", true);
   var stat = UserManager.GetLatestUserSet();
   Assert.True(stat.Count > 0);
  }

  [SkippableFact]
  //[Trait("Category", "Integration")] // kann seit EFC 2.1 als Unit Test laufen -)
  public void GetUserStatistics()
  {
   //Skip.If(Util.GetConnectionString() == "", "Only runs as integration test as the InMem-DB does not support SQL!");
   var um = new UserManager("test", true);
   var stat = UserManager.GetUserStatistics();
   Assert.True(stat.Count > 0);
  }

  [Fact]
  public void NewUserTest()
  {
   var name = Guid.NewGuid().ToString(); // GUID als Token und dann auch User Name
   var um = new UserManager(name, true);

   um.InitDefaultTasks();
   var cm = new CategoryManager(um.CurrentUser.UserID);
   var cset = cm.GetCategorySet();
   Assert.True(cset.Count == 4);
   Assert.All<Category>(cset, x => Assert.Equal(x.UserID, um.CurrentUser.UserID));
  }


  // DEMO: 70. Unit Tests mit XUnit
  /// <summary>
  /// Teste Login-Logik
  /// </summary>
  [Fact]
  public void LoginOKTest()
  {
   string name = "testuser " + Guid.NewGuid();
   string kennwort = Guid.NewGuid().ToString();

   // Arrange: Neuen User anlegen!
   var um = new UserManager(name, kennwort);
   // Arrange: einige Aufgaben speichern
   um.InitDefaultTasks();

   // Act
   var um2 = new UserManager(name, kennwort);
   var cm = new CategoryManager(um2.CurrentUser.UserID);
   var cset = cm.GetCategorySet();

   // Assert
   Assert.Equal(um2.CurrentUser.UserName, name);
   Assert.Equal(4, cset.Count);
   Assert.All<Category>(cset, x => Assert.Equal(x.UserID, um.CurrentUser.UserID));
  }

  [Fact]
  public void NoEmptyPasswords()
  {
   string name = "testuser " + Guid.NewGuid();
   string kennwort = "";

   // Vorbereitung: Das soll nicht klappen, wenn Kennwort leer!
   var um = new UserManager(name, kennwort);
   Assert.NotNull(um);
   Assert.Null(um.CurrentUser);
  }

  [Fact]
  public void LoginFailedTest()
  {
   string name = "testuser " + Guid.NewGuid();
   string kennwort = Guid.NewGuid().ToString();

   // Arrange: Neuen User anlegen!
   var um = new UserManager(name, kennwort);
   // Arrange: einige Aufgaben speichern
   um.InitDefaultTasks();

   // Act
   string wrongPassword = Guid.NewGuid().ToString();
   var um2 = new UserManager(name, wrongPassword);

   // Assert
   Assert.NotNull(um2);
   Assert.Null(um2.CurrentUser);
  }

  [Fact]
  public void TokenValidity()
  {
   string name = "testuser " + Guid.NewGuid();
   string kennwort = Guid.NewGuid().ToString();

   // Arrange: Neuen User anlegen!
   var env1 = new DummyEnv(DateTime.Now, "", "");
   var um1 = new UserManager(name, kennwort, "", env1);
   Assert.NotNull(um1.CurrentUser); 
   string token = um1.CurrentUser.Token;
   Assert.True(Guid.TryParse(token, out _));

   // Token-Nutzung eine Minute vor Ablauf
   var env2 = new DummyEnv(DateTime.Now.Add(UserManager.TokenValidity).AddMinutes(-1), "", "");
   var um2 = new UserManager(um1.CurrentUser.Token, env: env2);
   Assert.NotNull(um2.CurrentUser);

   // Token-Nutzung 8 Tage nach der letzten Aktivität
   var env3 = new DummyEnv(DateTime.Now.Add(UserManager.TokenValidity).AddDays(8), "", "");
   var um3 = new UserManager(um1.CurrentUser.Token, env: env3);
   Assert.NotNull(um3);
   Assert.Null(um3.CurrentUser);
  }

  [Theory]
  [InlineData("test3")]
  [InlineData("test2")]
  [InlineData("test1")]
  public void ExtistingUserTest(string name)
  {
   var um = new UserManager(name, true);
   var cm = new CategoryManager(um.CurrentUser.UserID);
   cm.RemoveAll();

   um.InitDefaultTasks();

   var cset = cm.GetCategorySet();
   Assert.True(cset.Count >= 3);
   Assert.All<Category>(cset, x => Assert.Equal(x.UserID, um.CurrentUser.UserID));
  }
 }
}