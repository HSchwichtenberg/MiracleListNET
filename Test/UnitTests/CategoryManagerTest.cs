using System;
using System.Collections.Generic;
using System.Text;
using BL;
using BO;
using Xunit;

namespace UnitTests
{
 public class CategoryManagerTest
 {
  private const string testcat = "testcat";

  public CategoryManagerTest()
  {
   Util.Init();
  }

  [Fact]
  public void Log()
  {
   var um = new UserManager("cattestuser " + System.DateTime.Now.Ticks, true);

   var c = new Category();
   c.Name = testcat;
   c.UserID = um.CurrentUser.UserID;
   var cm = new CategoryManager(um.CurrentUser.UserID);
   var c2 = cm.New(c);
   Assert.Equal(testcat, c2.Name);
  }

 }
}