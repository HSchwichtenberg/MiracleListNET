using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

using Microsoft.Data.SqlClient;

namespace MiracleList.Util
{

 /// <summary>
 /// Ergebnis des Refactoring am 12.2.2019
 /// </summary>
 public class MiracleListEnvInfo
 {

  private IConfigurationRoot configuration;
  private IWebHostEnvironment hostingEnv;
  private HttpContext context;

  public string AppName = "MiracleListBackend";
  public string Copyright = "(C) Dr.Holger Schwichtenberg, www.IT-Visions.de, 2017-" + DateTime.Now.Year;

  public MiracleListEnvInfo(IConfigurationRoot configuration, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
  {
   this.configuration = configuration;
   this.hostingEnv = env;
   this.context = httpContextAccessor?.HttpContext;
  }


  public List<string> GetAll()
  {

   var t = new Stopwatch();
   t.Start();

   // TODO: https://blogs.msdn.microsoft.com/martijnh/2010/07/15/sql-serverhow-to-quickly-retrieve-accurate-row-count-for-table/
   // SELECT CONVERT(bigint, rows)
   //   FROM sysindexes
   //WHERE id = OBJECT_ID('Transactions')
   //AND indid< 2

   string DbVersion = "?";
   string DbName = "?";
   string DbStatus = "?";
   var userCount = -1;
   var taskCount = -1;
   var clientCount = -1;
   var logCount = -1;

   try
   {
    var ctx = new DA.Context();
    ctx.Database.SetCommandTimeout(new TimeSpan(0, 0, 10));
    userCount = ctx.UserSet.Count();
    taskCount = ctx.TaskSet.Count();
    clientCount = ctx.ClientSet.Count();
    logCount = 0;
    //logCount = ctx.LogSet.Count();

    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ctx.Database.GetDbConnection().ConnectionString);
    DbName = builder?.DataSource;
    var conn = ctx.Database.GetDbConnection();
    conn.Open();
    DbVersion = ctx.Database.GetDbConnection()?.ServerVersion;
    conn.Close();
    DbStatus = "OK";
   }
   catch (Exception ex)
   {
    DbStatus = "Error: " + ex.Message;
   }

   t.Stop();



   var e = new List<string>()
   {
    "DateTime: " + DateTime.Now.ToString(),
    AppName,
    Copyright,
   "System Name: " + System.Environment.MachineName,
   "CurrentDirectory=" + System.Environment.CurrentDirectory
  };

   e.Add("ApplicationName: " + this.hostingEnv.ApplicationName);
   e.Add("Application Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
   e.Add("Application Informational Version: " + Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);

    e.Add(".NET Version: " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription + " ("+  ITVisions.CLRInfo.GetClrVersion() + ")");
    e.Add("OS Version: " + System.Environment.OSVersion);
   

   e.Add("Database Status: " + DbStatus);
   e.Add("Database Name: " + DbName);
   e.Add("Database Version: " + DbVersion);
   e.Add("Clients: " + clientCount);
   e.Add("Users: " + userCount);
   e.Add("Tasks: " + taskCount);
   e.Add("Log Entries: " + logCount);
   e.Add("Data Access Duration: " + t.ElapsedMilliseconds + "ms");

   if (context != null)
   {
    var httpConnectionFeature = context.Features.Get<IHttpConnectionFeature>();
    e.Add("Server-IP: " + httpConnectionFeature?.LocalIpAddress);
    e.Add("Client-IP: " + httpConnectionFeature?.RemoteIpAddress);
   }

   if (hostingEnv != null)
   {
    e.Add("Environment: " + this.hostingEnv.EnvironmentName);
    e.Add("WebRootPath=" + this.hostingEnv.WebRootPath);
    e.Add("ContentRootPath=" + this.hostingEnv.ContentRootPath);
   }

   e.Add("ApplicationBasePath=" + AppContext.BaseDirectory);

   if (configuration != null)
   {
    //e.Add("Release-Date: " + this.configuration["AppInfo:ReleaseDate"]);

    foreach (var p in configuration.Providers)
    {
     e.Add("Config Source: " + p.ToString());
    }
    foreach (var l1 in configuration.GetChildren())
    {
     foreach (var l2 in l1.GetChildren())
     {
      e.Add(ITVisions.RegEx.RegExUtil.ReplacePasswordInConnectionString(l2.Key + "=" + l2.Value));
     }
    }
   }

   return e;
  }
 }
}
