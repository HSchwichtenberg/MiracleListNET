using System;
using ITVisions.EFCore;

namespace BL
{
 /// <summary>
 /// Cleanup delete from [dbo].[User] where userid > 1000
 /// </summary>
 public class DataGenerator
 {

  // Names of German politians 
  static string[] PilotSurnames =
  {
   "Gysi", "Seehofer", "Wagenknecht", "Steinmeyer", "Schulz", "Merkel", "Lindner", "Gabriel", "Lafontaine", "Özdemir",
   "Roth"
  };

  static string[] PilotFirstnames =
  {
   "Horst", "Sahra", "Martin", "Angela", "Joschka", "Christian", "Gregor", "Sigmar", "Frank-Walter", "Oskar", "Cem", "Claudia"
  };

  static Random rnd = new Random(DateTime.Now.Millisecond);

  public static void Run(int userCount = 100, int catPerUserCount = 15, int tasksPerUserCount = 100)
  {
   for (int i = 0; i < userCount; i++)
   {
    string Firstname = PilotFirstnames[rnd.Next(0, PilotFirstnames.Length - 1)];
    string Surname = PilotSurnames[rnd.Next(0, PilotSurnames.Length - 1)];
    var um = new UserManager();
    var UserName = Firstname + " " + Surname;
    var u = um.New(new BO.User() { UserName = UserName });
    Console.WriteLine(u.UserID + ": " + u.UserName);

    for (int k = 0; k < rnd.Next(catPerUserCount); k++)
    {

     var c = new CategoryManager(u.UserID).CreateCategory("Test #" + k);

     for (int j = 0; j < rnd.Next(tasksPerUserCount); j++)
     {
      var t = new TaskManager(u.UserID).CreateTask(c.CategoryID, "Test", "", DateTime.Now, BO.Importance.B, 1);
     }
    }
   }

  }
 }
}