using BL;
using BO;
using ITVisions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
 /// Zweite API-Version
 /// </summary>
 [ApiExplorerSettings(GroupName = "v2")]
 [Route("v2")]
 public class MiracleListApiV2Controller : Controller
 {
  private TelemetryConfiguration telemetryConfiguration;
  private TelemetryClient telemetry;
  TaskManager tm;
  CategoryManager cm;

  private IConfigurationRoot Configuration;
  private IWebHostEnvironment Env;

  public MiracleListApiV2Controller(IConfigurationRoot configuration, IWebHostEnvironment env, TelemetryConfiguration tc)
  {
   this.Configuration = configuration;
   this.Env = env;
   this.telemetryConfiguration = tc;
   this.telemetry = new TelemetryClient(tc);
  }

  /// <summary>
  /// Hilfsroutine für alle Actions mit auth
  /// Hier wird der angemeldete User aus ASP.NET Core an die BL weitergereicht
  /// damit die BL keine Abhängigkeit von ASP.NET Core haben muss!
  /// </summary>
  private void Init()
  {
   var userID = Int32.Parse(HttpContext.User.Identity.Name);
   cm = new CategoryManager(userID);
   tm = new TaskManager(userID);
  }

  /// <summary>
  /// Informationen über den Server
  /// </summary>
  /// <returns></returns>
  [Route("About")]
  [HttpGet]
  public IEnumerable<string> About()
  {
   var httpConnectionFeature = HttpContext.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpConnectionFeature>();

  IEnumerable<String> s = (HttpContext.RequestServices.GetService(typeof(MiracleListEnvInfo)) as MiracleListEnvInfo).GetAll();
   s = s.Append("API-Version: v2");
   return s;
  }

  /// <summary>
  /// Liefert die Version des Servers als Zeichenkette
  /// </summary>
  /// <returns></returns>
  [Route("Version")]
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
  [Route("About2")]
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
  /// Login with a client ID, username and password. This operation / login sends back a GUID as a session token, to be used in all following operations.
  /// </summary>
  /// <param name="loginInfo"></param>
  /// <returns></returns>
  [HttpPost("Login")] // neu https://server/v2/login POST
  public async System.Threading.Tasks.Task<LoginInfo> Login([FromBody] LoginInfo loginInfo)
  {
   // Delegation an v1
   var v1 = new MiracleListApiController(this.Configuration, this.Env, this.telemetryConfiguration);
   return await v1.Login(loginInfo);
  }

  /// <summary>
  ///  Delete token
  /// </summary>
  /// <returns></returns>
  //[ApiExplorerSettings(GroupName = "Security")]
  [HttpGet("Logoff")] // neu
  public bool Logoff(string token)
  {
   return UserManager.Logout(token);
  }

  /// <summary>
  /// Get a list of all categories
  /// </summary>

  /// <returns></returns>
  [Authorize(AuthenticationSchemes = "MLToken")]// Auth Policy
  [HttpGet("CategorySet")]
  public IEnumerable<Category> GetCategorySet()
  {
   Init();
   return cm.GetCategorySet();
  }

  /// <summary>
  /// Get a list of tasks in one category
  /// </summary>

  /// <param name="categoryId"></param>
  /// <returns></returns>
  [HttpGet("TaskSet/{categoryId}")]
  [Authorize(AuthenticationSchemes = "MLToken")] // Auth Policy
  public IEnumerable<BO.Task> GetTaskSet(int categoryId)
  {
   if (categoryId <= 0) throw new Exception("Ungültig ID!");
   Init();
   return tm.GetTaskSet(categoryId);
  }

  /// <summary>
  /// Get details of one task
  /// </summary>

  /// <param name="id"></param>
  /// <returns></returns>
  [HttpGet("Task/{id}")]
  [Authorize(AuthenticationSchemes = "MLToken")]
  public BO.Task Task(int id)
  {
   if (id <= 0) throw new Exception("Ungültig ID!");
   Init();
   return tm.GetTask(id);
  }

  /// <summary>
  /// Search in tasks and subtasks
  /// </summary>

  /// <param name="text"></param>
  /// <returns></returns>
  [HttpGet("Search/{text}")]
  [Authorize(AuthenticationSchemes = "MLToken")]
  public IEnumerable<Category> Search(string text)
  {
   Init();
   return tm.Search(text);
  }

  /// <summary>
  /// Returns all tasks due, including tomorrow, grouped by category, sorted by date
  /// </summary>

  /// <returns></returns>
  [HttpGet("DueTaskSet")]
  [Authorize(AuthenticationSchemes = "MLToken")]
  public IEnumerable<Category> GetDueTaskSet()
  {
   Init();
   return tm.GetDueTaskSet();
  }

  /// <summary>
  /// Create a new category
  /// </summary>
  /// <param name="name">name of new category</param>
  /// <returns></returns>
  [HttpPost("CreateCategory/{name}")]
  [Authorize(AuthenticationSchemes = "MLToken")]
  public Category CreateCategory(string name)
  {
   Init();
   return cm.CreateCategory(name);
  }

  /// <summary>
  /// Create a task to be submitted in body in JSON format (including subtasks)
  /// </summary>

  /// <param name="t"></param>
  /// <returns></returns>
  [HttpPost("CreateTask")] // neu
  [Authorize(AuthenticationSchemes = "MLToken")]
  public BO.Task CreateTask([FromBody] BO.Task t)
  {
   Init();
   return tm.CreateTask(t);
  }

  /// <summary>
  /// Change a task to be submitted in body in JSON format (including subtasks)
  /// </summary>

  /// <param name="t"></param>
  /// <returns></returns>
  [HttpPut(nameof(ChangeTask))] // geändert
  [Authorize(AuthenticationSchemes = "MLToken")]
  public BO.Task ChangeTask([FromBody] BO.Task t)
  {
   Init();
   return tm.ChangeTask(t);
  }

  /// <summary>
  /// Change a task to be submitted in body in JSON format (including subtasks)
  /// </summary>
  /// <param name="categoryID"></param>
  /// <param name="orderderedTaskIDSet"></param>
  /// <returns></returns>
  [HttpPut(nameof(ChangeTaskOrder))] // geändert
  [Authorize(AuthenticationSchemes = "MLToken")]
  public int ChangeTaskOrder([FromQuery] int categoryID, [FromBody] List<int> orderderedTaskIDSet)
  {
   Init();
   var anzahlerfolgreiche = cm.ChangeTaskOrder(categoryID, orderderedTaskIDSet);
   return anzahlerfolgreiche;
  }

  /// <summary>
  /// Set a task to "done"
  /// </summary>

  /// <param name="id"></param>
  /// <param name="done"></param>
  /// <returns></returns>
  [HttpPut("ChangeTaskDone")]
  [Authorize(AuthenticationSchemes = "MLToken")]
  public BO.Task ChangeTaskDone(int id, bool done)
  {
   Init();
   tm.ChangeTaskDone(id, done);
   return null; 
  }

  /// <summary>
  /// Change a subtask
  /// </summary>

  /// <param name="st"></param>
  /// <returns></returns>
  [HttpPut("ChangeSubTask")]
  [Authorize(AuthenticationSchemes = "MLToken")]
  public SubTask ChangeSubTask([FromBody]SubTask st)
  {
   throw new UnauthorizedAccessException("du kommst hier nicht rein!");
  }

  /// <summary>
  /// Delete a task with all subtasks
  /// </summary>

  /// <param name="id"></param>
  [HttpDelete("DeleteTask/{id}")]
  [Authorize(AuthenticationSchemes = "MLToken")]
  public void DeleteTask(int id)
  {
   Init();
   tm.Remove(id);
  }

  /// <summary>
  /// Delete a category with all tasks and subtasks
  /// </summary>

  /// <param name="id"></param>
  [HttpDelete("[action]/{id}")]
  [Authorize(AuthenticationSchemes = "MLToken")]
  public void DeleteCategory(int id)
  {
   Init();
   cm.Remove(id);
  }
 }
}
