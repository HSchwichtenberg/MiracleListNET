using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using BL;
using BO;
using DA;
using Microsoft.EntityFrameworkCore;
using Xunit;

// XUNIT: https://xunit.github.io/docs/getting-started-dotnet-core.html

namespace UnitTests
{
 public class TaskManagerTest
 {

  public TaskManagerTest()
  {
   Util.Init();
  }

  [SkippableFact] // NUGET: Xunit.SkippableFact https://github.com/AArnott/Xunit.SkippableFact
  [Trait("Category", "Integration")]
  public void GetImportantTaskTest()
  {
   Skip.If(Util.IsInMemory, "Only runs as integration test as the InMem-DB does not support SQL!");
   var um = new UserManager("test", "test");
   Assert.NotNull(um.CurrentUser);
   var stat = new TaskManager(um.CurrentUser.UserID).GetImportantTaskSet();
   Assert.True(stat.Count > 0);
  }

  [Theory]
  [InlineData("test3")]
  [InlineData("test2")]
  [InlineData("test1")]
  public void GetTaskTest(string name)
  {
   if (!Util.IsInMemory)
   {
    new DA.Context().Database.ExecuteSqlInterpolated($"delete from [dbo].[User] where token = {name}");
   }

   var um = new UserManager(name, true);
   um.InitDefaultTasks();

   var tm = new TaskManager(um.CurrentUser.UserID);
   var cm = new CategoryManager(um.CurrentUser.UserID);
   var cset1 = cm.GetCategorySet();

   var c = cset1[1];
   var tset = tm.GetTaskSet(c.CategoryID);
   foreach (var t in tset)
   {
    var t2 = tm.GetTask(t.TaskID);
    Assert.Equal(t.TaskID, t2.TaskID);
    Assert.True(t2.SubTaskSet.Count > 1, "Fehlende Subtastsks bei Task: " + t2.TaskID.ToString());
   }
  }

  private static BO.Task CreateTaskDueIn3Days()
  {
   var um = new UserManager("CreateTaskTestUser", "CreateTaskTestUser");
   Assert.NotNull(um.CurrentUser);
   um.InitDefaultTasks();
   var tm = new TaskManager(um.CurrentUser.UserID);
   var cm = new CategoryManager(um.CurrentUser.UserID);
   var t = new BO.Task();
   t.CategoryID = cm.GetCategorySet().ElementAt(0).CategoryID;
   t.Due = DateTime.Now.AddDays(3);
   tm.CreateTask(t);
   Assert.True(t.TaskID > 0);
   return t;
  }

  [SkippableFact] // NUGET: Xunit.SkippableFact https://github.com/AArnott/Xunit.SkippableFact
  [Trait("Category", "Integration")]
  public void CreateTaskDueInDaysTest()
  {
   Skip.If(Util.IsInMemory, "Only runs as integration test as the InMem-DB does not support Computed Columns!");

   BO.Task t = CreateTaskDueIn3Days();

   Assert.Equal(3, t.DueInDays);// Computed Column Test: not supported in InMem-DB
  }

  [SkippableFact] // NUGET: Xunit.SkippableFact https://github.com/AArnott/Xunit.SkippableFact
  [Trait("Category", "Integration")]
  public void DefaultTitleTest()
  {
   Skip.If(Util.IsInMemory, "Only runs as integration test as the InMem-DB does not support Default Values!");

   BO.Task t = CreateTaskDueIn3Days();

   Assert.Equal(BO.Task.DefaultTitle, t.Title); // Default Value Test: not supported in InMem-DB
  }

  /// <summary>
  /// Legt Aufgabe mit 100 Unteraufgaben an und ändert diese (was die 100 Unteraufgaben löscht und neu anlegt)
  /// </summary>
  [Theory]
  [InlineData("test6")]
  [InlineData("test5")]
  //[InlineData("test4")]
  public void ChangeOneTest(string name)
  {
   const int subTaskCount = 100;
   var um = new UserManager(name, name);
   Assert.NotNull(um.CurrentUser);
   var tm = new TaskManager(um.CurrentUser.UserID);
   um.InitDefaultTasks();
   var cm = new CategoryManager(um.CurrentUser.UserID);
   var cset1 = cm.GetCategorySet();

   var testwert = DateTime.Now;

   var catID = cset1.ElementAt(0).CategoryID;
   var tset = tm.GetTaskSet(catID);
   var t = tset[0];

   var subtaskList = new List<SubTask>();
   for (int i = 0; i < subTaskCount; i++)
   {
    var st = new SubTask() { Title = "SubTask #" + i + ": " + testwert.ToString() };
    subtaskList.Add(st);
   }

   Assert.Equal(catID, t.CategoryID);

   // Jetzt ändern
   tm.ChangeTask(t.TaskID, testwert.ToString(), testwert.ToString(), testwert, t.Importance, true, t.Effort,
subtaskList);

   t = tm.GetByID(t.TaskID);
   Assert.Equal(catID, t.CategoryID);
   Assert.Equal(testwert.ToString(), t.Title);
   Assert.Equal(testwert.ToString(), t.Note);
   Assert.True(t.Done);
   Assert.Equal(subTaskCount, t.SubTaskSet.Count);
   Assert.All<SubTask>(t.SubTaskSet, st => Assert.EndsWith(testwert.ToString(), st.Title));
   Assert.All<SubTask>(t.SubTaskSet, st => Assert.Equal(t.TaskID, st.TaskID));

  }

  [Theory]
  [InlineData("Aufgabe1")]
  [InlineData("Aufgabe2")]
  [InlineData("Aufgabe3")]
  public void CreateTaskTest(string name)
  {
   var um = new UserManager(name, name);
   Assert.NotNull(um.CurrentUser);
   var tm = new TaskManager(um.CurrentUser.UserID);
   um.InitDefaultTasks();
   var cm = new CategoryManager(um.CurrentUser.UserID);
   var cset1 = cm.GetCategorySet();

   var dat = DateTime.Now;

   var t = tm.CreateTask(cset1.ElementAt(0).CategoryID, name, name, dat, Importance.A, 1);

   Assert.NotNull(t);
   Assert.Equal(dat, t.Due);
  }

  [Theory]
  [InlineData("Aufgabe1")]
  [InlineData("Aufgabe2")]
  [InlineData("Aufgabe3")]
  public void CreateTaskTestOhneDatum(string name)
  {
   var um = new UserManager(name, name);
   Assert.NotNull(um.CurrentUser);
   var tm = new TaskManager(um.CurrentUser.UserID);
   um.InitDefaultTasks();
   var cm = new CategoryManager(um.CurrentUser.UserID);
   var cset1 = cm.GetCategorySet();


   var t = tm.CreateTask(cset1.ElementAt(0).CategoryID, name, name, null, Importance.A, 1);

   Assert.NotNull(t);
   Assert.Null(t.Due);
  }

  [Theory]
  [InlineData("test6")]
  [InlineData("test5")]
  [InlineData("test4")]
  public void ChangeTaskDoneTest(string name)
  {
   var um = new UserManager(name, name);
   Assert.NotNull(um.CurrentUser);
   var tm = new TaskManager(um.CurrentUser.UserID);
   um.InitDefaultTasks();
   var cm = new CategoryManager(um.CurrentUser.UserID);
   var cset = cm.GetCategorySet();
   var tset = tm.GetTaskSet(cset[0].CategoryID);
   var t = tset[0];
   var statusBefore = t.Done;
   // Änderung speichern
   tm.ChangeTaskDone(t.TaskID, !t.Done);
   // Lade Task erneut
   var t2 = tm.GetTask(t.TaskID);
   // Status sollte nun umgekehrt sein!
   Assert.Equal(!statusBefore, t2.Done);
  }

  [Theory]
  [InlineData("test6")]
  [InlineData("test5")]
  [InlineData("test4")]
  public void ChangeTest(string name)
  {
   var um = new UserManager(name, name);
   Assert.NotNull(um.CurrentUser);
   var tm = new TaskManager(um.CurrentUser.UserID);
   um.InitDefaultTasks();
   var cm = new CategoryManager(um.CurrentUser.UserID);
   var cset1 = cm.GetCategorySet();

   var testwert = DateTime.Now;

   // Schleife über alle Kategorien
   foreach (var c in cset1)
   {
    Assert.Equal(um.CurrentUser.UserID, c.UserID);
    var tset = tm.GetTaskSet(c.CategoryID);

    // Alle Aufgaben
    foreach (var t in tset)
    {
     var st1 = new SubTask() { Title = testwert.ToString() };
     var st2 = new SubTask() { Title = testwert.ToString() };

     Assert.Equal(c.CategoryID, t.CategoryID);
     tm.ChangeTask(t.TaskID, testwert.ToString(), testwert.ToString(), testwert, t.Importance, true, t.Effort,
      new List<SubTask> { st1, st2 });
    }
   }

   // nun prüfen!
   var cset2 = cm.GetCategorySet();

   foreach (var c in cset2)
   {
    Assert.Equal(c.UserID, um.CurrentUser.UserID);
    var tset = tm.GetTaskSet(c.CategoryID);

    foreach (var t in tset)
    {
     Assert.Equal(c.CategoryID, t.CategoryID);
     Assert.Equal(testwert.ToString(), t.Title);
     Assert.Equal(testwert.ToString(), t.Note);
     Assert.True(t.Done);
     Assert.True(t.Done);
     Assert.Equal(2, t.SubTaskSet.Count);
     Assert.All<SubTask>(t.SubTaskSet, st => Assert.Equal(testwert.ToString(), st.Title));
     Assert.All<SubTask>(t.SubTaskSet, st => Assert.Equal(t.TaskID, st.TaskID));
    }
   }
  }

  [Theory]
  [InlineData("ctest3")]
  [InlineData("ctest2")]
  [InlineData("ctest1")]
  public void ChangeTest2(string name)
  {
   var um = new UserManager(name, name);
   Assert.NotNull(um.CurrentUser);
   var tm = new TaskManager(um.CurrentUser.UserID);
   um.InitDefaultTasks();
   var cm = new CategoryManager(um.CurrentUser.UserID);
   var cset1 = cm.GetCategorySet();

   var testwert = DateTime.Now;

   // Werte ändern
   foreach (var c in cset1)
   {
    Assert.Equal(um.CurrentUser.UserID, c.UserID);
    var tset = tm.GetTaskSet(c.CategoryID);


    foreach (var t in tset)
    {
     t.Title = testwert.ToString();
     t.Note = testwert.ToString();
     t.Due = testwert;
     t.Done = true;
     t.SubTaskSet.ForEach(x => x.Title = testwert.ToString());
     tm.ChangeTask(t);
    }
   }

   // nun prüfen!
   var cset2 = cm.GetCategorySet();

   foreach (var c in cset2)
   {
    Assert.Equal(c.UserID, um.CurrentUser.UserID);
    var tset = tm.GetTaskSet(c.CategoryID);

    foreach (var t in tset)
    {
     Assert.Equal(c.CategoryID, t.CategoryID);
     Assert.Equal(testwert.ToString(), t.Title);
     Assert.Equal(testwert.ToString(), t.Note);
     Assert.True(t.Done);

     Assert.All<SubTask>(t.SubTaskSet, st => Assert.Equal(testwert.ToString(), st.Title));
     Assert.All<SubTask>(t.SubTaskSet, st => Assert.Equal(t.TaskID, st.TaskID));
    }
   }
  }
 }
}