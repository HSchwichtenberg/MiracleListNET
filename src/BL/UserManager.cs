using System;
using System.Collections.Generic;
using System.Linq;
using BO;
using BO.NonEntityClass;
using DA;
using ITVisions;
using ITVisions.EFCore;
using ITVisions.Network;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace BL;

public class UserManager : EntityManagerBase<Context, User>
{
 // Simple DI without DI Framework ;-)
 public IEnv Env = new Env();

 public static TimeSpan TokenValidity = new TimeSpan(7, 0, 0, 0); // 7 Days
 public User CurrentUser = null;
 private CategoryManager cm;
 private TaskManager tm;

 // nur für Test
 public UserManager(Context ctx)
 {
  this.ctx = ctx;
 }

 public UserManager()
 {

 }

 public UserManager(int id)
 {
  this.CurrentUser = ctx.UserSet.SingleOrDefault(x => x.UserID == id);
  this.CurrentUser.PasswordHash = null;
  this.CurrentUser.Salt = null;
 }

 public static bool IsTokenValid(string token)
 {
  return (new BL.UserManager(token).CurrentUser != null);
 }

 /// <summary>
 /// Create an instance of the class with user tokens. Saves last use in User.LastActivity.
 /// Statefull object with Current User in CurrentUser!
 /// </summary>
 public UserManager(string token, bool CreateIfNotExists = false, bool PasswordReset = false, IEnv env = null)
 {
  if (env != null) this.Env = env;
  this.CurrentUser = GetUserByToken(token, CreateIfNotExists, PasswordReset);
  if (this.CurrentUser == null) return; // wrong token!
  this.CurrentUser.PasswordHash = null;
  this.CurrentUser.Salt = null;
  if (this.CurrentUser != null)
  {
   cm = new CategoryManager(this.CurrentUser.UserID);
   tm = new TaskManager(this.CurrentUser.UserID);
  }
 }

 /// <summary>
 /// Create an instance of the class with username and password. If the data is valid, the user is logged in and receives a new token (a GUID!)
 /// Special feature ONLY FOR DEMO APPLICATION: If the user does not exist, a new user is created with the user name and password
 /// </summary>
 public UserManager(string username, string password, string token = "", IEnv env = null, string clientID = null)
 {
  if (env != null) this.Env = env;
  if (String.IsNullOrEmpty(password)) { this.CurrentUser = null; return; }
  this.CurrentUser = GetOrCreateUser(username, password, clientID: clientID);
  if (this.CurrentUser == null) return; // wrong pasword!

  // we don't want to send these to the client!
  this.CurrentUser.PasswordHash = null;
  this.CurrentUser.Salt = null;

  if (this.CurrentUser != null)
  {
   cm = new CategoryManager(this.CurrentUser.UserID);
   tm = new TaskManager(this.CurrentUser.UserID);
   // always set up standard tasks for new users and those who no longer have any categories!
   InitDefaultTasks();
  }
 }

 public static bool Logout(User user)
 {
  try
  {
   if (user == null) return false;

   // TODO: Das geht bisher nicht, weil mehrere Sitzungen eines Benutzers das gleiche Token haben
   //var ctx = new Context();
   //var u = ctx.UserSet.SingleOrDefault(x => x.UserID == user.UserID);
   ////u.Token = ""; 
   //ctx.SaveChanges();
   return true;
  }
  catch
  {
   return false;
  }
 }

 /// <summary>
 /// Log off the user who has the named token. The token is deleted in DB!
 /// </summary>
 /// <param name="token"></param>
 /// <returns></returns>
 public static bool Logout(string token)
 {
  try
  {
   // Abmelden = aktuelles Token löschen
   var ctx = new Context();
   var u = ctx.UserSet.SingleOrDefault(x => x.Token.ToLower() == token.ToLower());
   return Logout(u);
  }
  catch
  {
   return false;
  }
 }

 private User GetUserByToken(string token, bool CreateIfNotExists = true, bool PasswordReset = false)
 {
  //Guid guid;
  //if (!Guid.TryParse(token, out guid)) return null;
  var ctx = new Context();

  var u = ctx.UserSet.SingleOrDefault(x => x.Token.ToLower() == token.ToLower());

  if (u != null)
  {
   // Token will be invalid after certain TimeSpan of Inactivity
   if ((Env.Now - u.LastActivity) > TokenValidity) return null;
   u.LastActivity = Env.Now;
   ctx.SaveChanges();
   return u;
  }
  if (!CreateIfNotExists) return null;
  // just for demo: If there are no tokens, AdHoc will create a new user for it
  return GetOrCreateUser(token, token, token, PasswordReset);
 }

 /// <summary>
 /// Get existing user or creates a new one
 /// </summary>
 private User GetOrCreateUser(string name, string password, string token = "", bool PasswordReset = false, string clientID = null)
 {
  this.StartTracking();

  var u = ctx.UserSet.SingleOrDefault(x => x.UserName.ToLower() == name.ToLower());

  if (u != null) // username found!
  {
   // is the password correct?
   var hashObj = ITVisions.Security.Hashing.HashPassword(password, u.Salt);

   if (u.PasswordHash != hashObj.HashedText)
   {
    // wrong hash
    if (!PasswordReset) return null;
   }

   u.PasswordHash = hashObj.HashedText;
   u.Salt = hashObj.Salt;

   // create a new token and store in DB in user record    
   if (String.IsNullOrEmpty(u.Token) || ((Env.Now - u.LastActivity) > TokenValidity)) u.Token = Guid.NewGuid().ToString("D");
   u.Memo += "Login " + Env.Now + "/" + u.Token + "\n";
   u.LastActivity = DateTime.Now;
   ctx.SaveChanges();
   this.SetTracking();
   return u; // return existing user
  }
  else
  { // there is no user with this name -> create him/her!
   u = new User();
   u.UserName = name;

   // Create password hash with salt
   var hashObj = ITVisions.Security.Hashing.HashPassword(password);
   u.PasswordHash = hashObj.HashedText;
   u.Salt = hashObj.Salt;
   u.Created = Env.Now;

   Guid clientIDGUID;
   if (Guid.TryParse(clientID, out clientIDGUID))
   {
    u.ClientID = clientIDGUID;
   }

   if (token == "") token = Guid.NewGuid().ToString("D"); // 38 chrs including { and -
   u.Token = token;
   u.Memo = "Created " + Env.Now + "/" + password + "\n";
   this.New(u);

   if (ITVisions.Network.MailUtil.IsValidEmail(u.UserName))
   {
    var text = ITVisions.TextGenerator.TextGen.GetGreeting() + " " + u.UserName + ",";
    text = text.Add("\n\n", "Ihr Zugang zu MiracleList:");
    text = text.Add("\n\n", "E-Mail-Adresse: " + u.UserName);
    text = text.Add("\n", "Kennwort: " + password);
    text = text.Add("\n\n", "Haben Sie Fragen zu MiracleList? http://www.miraclelist.net");
    text = text.Add("\n\n", "Mit besten Grüßen");
    text = text.Add("\n", "Ihr Expertenteam bei www.IT-Visions.de");
    text = text.Add("\n\n", "Fachbücher: https://www.IT-Visions.de/Buecher");
    text = text.Add("\n", "Schulungen: https://www.IT-Visions.de/Schulungen");
    text = text.Add("\n", "Beratung: https://www.IT-Visions.de/Beratung");
    text = text.Add("\n", "Technischer Support: https://www.IT-Visions.de/Support");
    text = text.Add("\n", "Softwareentwicklung: https://www.IT-Visions.de/Softwareentwicklung");

    new MailUtil().SendMail("do-not-reply@mail.miraclelist.net", u.UserName, "Ihr Zugang zu MiracleList", text);
   }

   return u; // return new user
  }
 }

 public void ClearAllData()
 {
  ctx.CategorySet.Where(x => x.UserID == CurrentUser.UserID).Delete();
 }

 /// <summary>
 /// Creates some standard tasks for the current user. It is always called at every operation to ensure that a user always deals with some tasks
 /// </summary>
 public void InitDefaultTasks()
 {
  if (CurrentUser == null) return;
  if (ctx.CategorySet.Where(x => x.UserID == CurrentUser.UserID).Count() > 0) return;

  var st01 = new SubTask();
  st01.Title = "Aufgaben in Kategorie Beruf ansehen";
  var st02 = new SubTask();
  st02.Title = "Aufgaben in Kategorie Haushalt ansehen";
  var st03 = new SubTask();
  st03.Title = "Aufgaben in Kategorie Freizeit ansehen";

  var c0 = cm.CreateCategory("Über die App");
  var t01 = tm.CreateTask(c0.CategoryID, "Beispielaufgaben erforschen", "Jeder neue Benutzer erhält automatisch einige Beispielaufgaben in vier Kategorien. ACHTUNG: Wenn Sie die letzte Aufgabe löschen, werden die Beispielaufgaben automatisch beabsichtigt alle wieder angelegt :-)", Env.Now.AddHours(3), Importance.A, 1, new List<SubTask>() { st01, st02, st03 });

  var st02a = new SubTask() { Title = "Mithelfen, das Beispiel besser zu machen: https://github.com/HSchwichtenberg/MiracleListClient" };

  var t02 = tm.CreateTask(c0.CategoryID, "Verstehen, dass MiracleList eine Beispiel-Anwendung ist und kein fertiges Produkt.", "Es geht in diesem Beispiel darum, möglichste viele Techniken zu zeigen und nicht darum, bis wie bei einem echten Produkt exakt und rein zu programmieren.", Env.Now.AddHours(3), Importance.A, 1, new List<SubTask>() { st02a });

  var t04 = tm.CreateTask(c0.CategoryID, "Web- und Cross-Platform-Techniken lernen", "Wenn Sie die hier eingesetzen Techniken (.NET Core, C#, ASP.NET Core WebAPI, Entity Framework Core, SQL Azure, Azure Web App, Swagger, HTML, CSS, TypeScript, Angular, Bootstrap, MomentJS, angular2-moment, angular2-contextmenu, angular2-modal, Electron, Cordova etc.) lernen wollen, besuchen Sie www.IT-Visions.de/ST", Env.Now.AddDays(30), Importance.B, 40, null);

  var st03a = new SubTask() { Title = "Client-ID beantragen: https://miraclelistbackend.azurewebsites.net/client" };
  var st03b = new SubTask() { Title = "Technik für Client auswählen, z.B. Angular, React, Vue.js, Blazor, WPF, WinUI, PHP etc." };
  var st03c = new SubTask() { Title = "Client programmieren" };

  var t03 = tm.CreateTask(c0.CategoryID, "Selbst einen eigenen MiracleList-Client schreiben", "Das Backend steht Ihnen dafür zur Verfügung: https://miraclelistbackend.azurewebsites.net - Sie müssen sich dort registriert für eine Client-ID.", Env.Now.AddDays(60), Importance.C, 100, new List<SubTask>() { st03a, st03b, st03c });

  var st1 = new SubTask();
  st1.Title = "Teil 1";
  var st2 = new SubTask();
  st2.Title = "Teil 2";
  var st3 = new SubTask();
  st3.Title = "Teil 3";

  var c1 = cm.CreateCategory("Beruf");
  var t10 = tm.CreateTask(c1.CategoryID, "MiracleList-Tutorial schreiben", "Teil 1: Einrichten eines Projekts, Datenabruf von REST-Diensten, Rendern von Daten per Template\nTeil 2: Routing, Formulare für das Einfügen, Ändern und Löschen von Daten, Senden von Daten an REST-Dienste\nTeil 3: Menü, Kontextmenü, Dialogfenster, Animationen, Benutzeranmeldung und Auslieferung des Projekts", Env.Now.AddDays(30), Importance.A, 40, new List<SubTask>() { st1, st2, st3 });

  st1 = new SubTask();
  st1.Title = "Planen";
  st2 = new SubTask();
  st2.Title = "Ausführen";

  var t11 = tm.CreateTask(c1.CategoryID, "Projektplan erstellen", "Beispielaufgabe", Env.Now.AddDays(-2), Importance.A, 2, new List<SubTask>() { st1, st2 });

  st1 = new SubTask();
  st1.Title = "Planen";
  st2 = new SubTask();
  st2.Title = "Ausführen";

  var t12 = tm.CreateTask(c1.CategoryID, "Teambesprechung abhalten", "Beispielaufgabe", Env.Now.AddDays(7), Importance.B, 3, new List<SubTask>() { st1, st2 });

  st1 = new SubTask();
  st1.Title = "Planen";
  st2 = new SubTask();
  st2.Title = "Ausführen";
  var t13 = tm.CreateTask(c1.CategoryID, "Schulungen buchen", "siehe www.IT-Visions.de/Schulungen", Env.Now.AddDays(-10), Importance.B, 1, new List<SubTask>() { st1, st2 });

  var c2 = cm.CreateCategory("Haushalt");
  st1 = new SubTask();
  st1.Title = "Planen";
  st2 = new SubTask();
  st2.Title = "Ausführen";

  var t21 = tm.CreateTask(c2.CategoryID, "Saugen", "Beispielaufgabe", Env.Now.AddDays(2), Importance.B, 1.25m, new List<SubTask>() { st1, st2 });

  st1 = new SubTask();
  st1.Title = "Planen";
  st2 = new SubTask();
  st2.Title = "Ausführen";

  var t22 = tm.CreateTask(c2.CategoryID, "Müll herausbringen", "Beispielaufgabe",
Env.Now.AddDays(1), Importance.A, 0.5m, new List<SubTask>() { st1, st2 });

  var c3 = cm.CreateCategory("Freizeit");
  st1 = new SubTask();
  st1.Title = "Planen";
  st2 = new SubTask();
  st2.Title = "Ausführen";
  var t31 = tm.CreateTask(c3.CategoryID, "Trainieren für MTB-Marathon", "Beispielaufgabe", Env.Now.AddDays(1), Importance.A, 120, new List<SubTask>() { st1, st2 });

  st1 = new SubTask();
  st1.Title = "Planen";
  st2 = new SubTask();
  st2.Title = "Ausführen";
  var t32 = tm.CreateTask(c3.CategoryID, "Kino", "Beispielaufgabe", Env.Now.AddDays(14), Importance.B, 3.5m, new List<SubTask>() { st1, st2 });

 }

 public TokenValidationResult IsValid()
 {
  if (this.CurrentUser == null) return TokenValidationResult.AccessDenied;
  return TokenValidationResult.Ok;
 }

 public enum TokenValidationResult
 {
  Ok, AccessDenied, UserLocked
 }

 public static List<User> GetLatestUserSet()
 {
  using (var ctx = new Context())
  {
   var r = ctx.UserSet.FromSqlRaw("Select * from [User]").OrderByDescending(x => x.Created).Take(10).ToList();

   return r;
  }
 }

 public static List<UserStatistics> GetUserStatistics()
 {
  using (var ctx = new Context())
  {
   // ctx.Log((x) =>
   //{
   // System.Diagnostics.Debug.WriteLine(x);
   // using (StreamWriter sw = File.AppendText(@"c:\temp\EFCLog.txt"))
   // {
   //  sw.WriteLine(x);
   // }

   //}
   // );

   // wird ab EFC 2.1 korrekt in SQL umgesetzt

   //    SELECT[t].[userID],
   //       [t].[Count],
   //       [u].[UserName]
   //  FROM[User] AS[u]
   //       INNER JOIN(SELECT TOP(10 /* @__p_0 */) [p.Category].[UserID] AS[userID],
   //                                      COUNT(*)              AS[Count]
   //                   FROM[Task] AS[p]
   //                       INNER JOIN[Category] AS[p.Category]
   //                            ON[p].[CategoryID] = [p.Category].[CategoryID]
   //  GROUP BY[p.Category].[UserID]
   //  ORDER BY[Count] DESC) AS[t]
   //ON[u].[UserID] = [t].[userID]
   var groups = (from u in ctx.UserSet
                 join x in ((from p in ctx.TaskSet
                             group p by p.Category.UserID into g
                             select new { userID = g.Key, Count = g.Count() }).OrderByDescending(x => x.Count).Take(10))
                  on u.UserID equals x.userID
                 select new { u.UserName, x.Count });

   var r = new List<UserStatistics>();
   foreach (var g in groups)
   {
    r.Add(new UserStatistics() { UserName = g.UserName, NumberOfTasks = g.Count });
   }

   //var SQL = @"SELECT[User].UserName, COUNT(Task.TaskID) AS NumberOfTasks FROM Category INNER JOIN
   //                      Task ON Category.CategoryID = Task.CategoryID INNER JOIN
   //                      [User] ON Category.UserID = [User].UserID
   //                      GROUP BY[User].UserName";

   //var r = ctx.UserStatistics.FromSql(SQL).OrderByDescending(x => x.NumberOfTasks).Take(10).ToList();

   return r;
  }
 }
}