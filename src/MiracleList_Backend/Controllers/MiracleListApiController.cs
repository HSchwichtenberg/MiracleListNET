using BL;
using BO;
using ITVisions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MiracleList.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MiracleList.Controllers
{

 /// <summary>
 /// API v1
 /// </summary>
 [Route("")]
 [ApiExplorerSettings(GroupName = "v1")]
 public class MiracleListApiController : Controller
 {
  private TelemetryClient telemetry;
  TaskManager tm;
  UserManager um;
  CategoryManager cm;

  private IConfigurationRoot Configuration;
  private IWebHostEnvironment Env;

  public MiracleListApiController(IConfigurationRoot configuration, IWebHostEnvironment env, TelemetryConfiguration tc)
  {
   this.Configuration = configuration;
   this.Env = env;
   this.telemetry = new TelemetryClient(tc);
  }

  /// <summary>
  /// Helper for all actions to check the token and save telemetry data
  /// </summary>
  private bool CheckToken(string token, [CallerMemberName] string caller = "?")
  {
   if (token == null || token.Length < 2)
   {
    // save telemetry data
    var p2 = new Dictionary<string, string>();
    p2.Add("token", token);
    telemetry.TrackEvent("TOKENERROR_" + caller, p2);
    new LogManager().Log(Event.TokenCheckError, Severity.Warning, "Ungültiges Token", caller, token);
    throw new Exception("Ungültiges Token!");
   }

   // validate token
   um = new UserManager(token);
   var checkResult = um.IsValid();
   if (checkResult != UserManager.TokenValidationResult.Ok)
   {
    // save telemetry data
    var p2 = new Dictionary<string, string>();
    p2.Add("token", token);
    p2.Add("checkResult", checkResult.ToString());
    telemetry.TrackEvent("USERERROR_" + caller, p2);

    new LogManager().Log(Event.TokenCheckError, Severity.Warning, checkResult.ToString(), caller, token, um.CurrentUser?.UserID);

    throw new Exception(checkResult.ToString());
   }
   um.InitDefaultTasks();

   // Create manager objects
   cm = new CategoryManager(um.CurrentUser.UserID);
   tm = new TaskManager(um.CurrentUser.UserID);

   // save telemetry data
   var p = new Dictionary<string, string>();
   p.Add("token", token);
   p.Add("user", um.CurrentUser.UserName);
   telemetry.TrackEvent(caller, p);

   new LogManager().Log(Event.TokenCheckOK, Severity.Information, null, caller, token, um.CurrentUser?.UserID);
   return true;
  }

  /// <summary>
  /// About this server
  /// </summary>
  /// <returns></returns>
  [Route("/About")]
  [HttpGet]
  public IEnumerable<string> About()
  {
   IEnumerable<String> s = (HttpContext.RequestServices.GetService(typeof(MiracleListEnvInfo)) as MiracleListEnvInfo).GetAll();
   s = s.Append("API-Version: v1");
   return s;

  }

  /// <summary>
  /// Get version of server
  /// </summary>
  /// <returns></returns>
  [Route("/Version")]
  [HttpGet]
  public string Version()
  {
   return
   Assembly.GetEntryAssembly()
 .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
 .InformationalVersion.ToString();
  }

  /// <summary>
  /// Nur für einen Test
  /// </summary>
  /// <returns></returns>
  [Route("/About2")]
  [ApiExplorerSettings(IgnoreApi = true)]
  [HttpGet]
  public JsonResult GetAbout2()
  {
   var v = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
   var e = new string[] { "MiracleListBackend", "(C) Dr. Holger Schwichtenberg, www.IT-Visions.de", "Version: " + v };
   var r = new JsonResult(e);
   this.Response.Headers.Add("X-Version", v);
   r.StatusCode = 202;
   return r;
  }

  /// <summary>
  /// Login with a client ID, username and password. This operation sends back a GUID as a session token, to be used in all following operations.
  /// </summary>
  [HttpPost("Login")] // neu
  public async System.Threading.Tasks.Task<LoginInfo> Login([FromBody] LoginInfo loginInfo)
  {
   bool reinit = false;
   const string MAGICSTRING = "+init";
   // Trick für Demo-Zwecke: wenn Kennwort auf "+init" endet, werden alle Aufgaben gelöscht und neue Beispielaufgaben angelegt
   // Das Kennwort davor muss aber stimmen!
   if (!String.IsNullOrEmpty(loginInfo.Password) && loginInfo.Password.EndsWith(MAGICSTRING))
   {
    reinit = true;
    loginInfo.Password = loginInfo.Password.Replace(MAGICSTRING, "");
   }

   var cm = new ClientManager();
   if (loginInfo == null)
   {
    return new LoginInfo()
    {
     Message = "LoginInfo is null!"
    };
   }
   if (String.IsNullOrEmpty(loginInfo.ClientID)) {
    return new LoginInfo()
    {
     Message = "No Client ID!"
    };
   }
   var e = cm.CheckClient(loginInfo.ClientID);
   if (e.CheckClientResultCode != ClientManager.CheckClientResultCode.Ok)
   {
    new LogManager().Log(Event.LogginError, Severity.Warning, Enum.GetName(typeof(ClientManager.CheckClientResultCode), e.CheckClientResultCode) + "\n" + e.client?.ToNameValueString(), "ClientIDCheck", "", um?.CurrentUser?.UserID);
    return new LoginInfo()
    {
     Message = "Client-ID-Check: " + Enum.GetName(typeof(ClientManager.CheckClientResultCode), e.CheckClientResultCode) + " - Please register a new Client-ID at http://miraclelistbackend.azurewebsites.net/clientid"
    };
   }

   User u;
   if (string.IsNullOrEmpty(loginInfo.Password))
   {
    // Token set? Then revalidate token!
    if (string.IsNullOrEmpty(loginInfo.Token))
    {
     const string ErrorText = "password and token empty";
     new LogManager().Log(Event.LogginError, Severity.Warning, "", ErrorText);
     throw new Exception(ErrorText);
    }
    // Re-Login with existing token!
    u = new UserManager(loginInfo.Token).CurrentUser;
   }
   else
   { // Login with username and password
    u = new UserManager(loginInfo.Username, loginInfo.Password).CurrentUser;
   }

   if (u == null)
   {
    new LogManager().Log(Event.LogginError, Severity.Warning, loginInfo.ToNameValueString() + "\n" + e.client?.ToNameValueString(), "UserCheck", u?.Token, um?.CurrentUser?.UserID);
    return new LoginInfo() { Message = "Access denied!" };
   }

   // username and password OK

   // Magic Password "+init" == remove all data and reinit
   if (reinit)
   {
    var um = new UserManager(loginInfo.Username, loginInfo.Password);
    um.ClearAllData();
    um.InitDefaultTasks();
    loginInfo.Message = "";
   }

   // Set token and username in Login-Response, do not return the password!
   loginInfo.Token = u.Token;
   loginInfo.Username = u.UserName;
   loginInfo.Password = "";

   new LogManager().Log(Event.LoginOK, Severity.Information, null, "UserCheck", u.Token, u.UserID);

   return loginInfo;
  }

  /// <summary>
  /// Delete token
  /// </summary>
  [HttpGet("Logoff/{token}")]
  public bool Logoff(string token)
  {
   return UserManager.Logout(token);
  }

  /// <summary>
  /// Get a list of all categories
  /// </summary>
  [HttpGet("CategorySet/{token}")]
  public IEnumerable<Category> GetCategorySet(string token)
  {
   if (!CheckToken(token)) return null;
   return cm.GetCategorySet();
  }

  /// <summary>
  /// Get a list of tasks in one category
  /// </summary>
  [HttpGet("TaskSet/{token}/{id}")]
  public IEnumerable<BO.Task> GetTaskSet(string token, int id)
  {
   if (id <= 0) throw new Exception("Invalid ID!");
   if (!CheckToken(token)) return null;
   return tm.GetTaskSet(id);
  }

  /// <summary>
  /// Get details of one task
  /// </summary>
  [HttpGet("Task/{token}/{id}")]
  public BO.Task Task(string token, int id)
  {
   if (id <= 0) throw new Exception("Invalid ID!");
   if (!CheckToken(token)) return null;
   return tm.GetTask(id);
  }

  /// <summary>
  /// Search in tasks and subtasks
  /// </summary>
  [HttpGet("Search/{token}/{text}")]
  public IEnumerable<Category> Search(string token, string text)
  {
   if (!CheckToken(token)) return null;
   return tm.Search(text);
  }

  /// <summary>
  /// Returns all tasks due, including tomorrow, grouped by category, sorted by date
  /// </summary>
  [HttpGet("DueTaskSet/{token}")]
  public IEnumerable<Category> GetDueTaskSet(string token)
  {
   if (!CheckToken(token)) return null;
   return tm.GetDueTaskSet();
  }

  /// <summary>
  /// Create a new category
  /// </summary>
  [HttpPost("CreateCategory/{token}/{name}")]
  public Category CreateCategory(string token, string name)
  {
   if (!CheckToken(token)) return null;
   return cm.CreateCategory(name);
  }

  /// <summary>
  /// Create a task to be submitted in body in JSON format (including subtasks)
  /// </summary>
  /// <param name="token"></param>
  /// <param name="t"></param>
  /// <returns></returns>
  [HttpPost("CreateTask/{token}")] // neu
  public BO.Task CreateTask(string token, [FromBody] BO.Task t)
  {
   if (!CheckToken(token)) return null;
   return tm.New(t);
  }

  /// <summary>
  /// Change a task to be submitted in body in JSON format (including subtasks)
  /// </summary>
  [HttpPut("ChangeTask/{token}")] // geändert
  public BO.Task ChangeTask(string token, [FromBody] BO.Task t)
  {
   if (!CheckToken(token)) return null;
   return tm.ChangeTask(t);
  }

  /// <summary>
  /// Set a task to "done"
  /// </summary>
  [HttpPut("ChangeTaskDone/{token}")]
  public BO.Task ChangeTaskDone(string token, int id, bool done)
  {
   throw new UnauthorizedAccessException("du kommst hier nicht rein!");
  }

  /// <summary>
  /// Change a subtask
  /// </summary>
  [HttpPut("ChangeSubTask/{token}")]
  public SubTask ChangeSubTask(string token, [FromBody]SubTask st)
  {
   throw new UnauthorizedAccessException("du kommst hier nicht rein!");
  }

  /// <summary>
  /// Delete a task with all subtasks
  /// </summary>
  [HttpDelete("DeleteTask/{token}/{id}")]
  public void DeleteTask(string token, int id)
  {
   if (!CheckToken(token)) return;
   tm.RemoveTask(id);
  }

  /// <summary>
  /// Delete a category with all tasks and subtasks
  /// </summary>
  [HttpDelete("[action]/{token}/{id}")]
  public void DeleteCategory(string token, int id)
  {
   if (!CheckToken(token)) return;
   cm.RemoveCategory(id);
  }
 }
}
