using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BL;
using ITVisions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using MiracleList.CustomAuthenticationService;
using MiracleList.Util;
using MiracleList_Backend.Hubs;
using MiracleList_Backend.SwaggerExtensions;

namespace MiracleList;

public class Startup
{
 public IConfigurationRoot Configuration { get; }

 public Startup(IWebHostEnvironment env)
 {
  CUI.Headline("Startup");

  #region Additional Columns added after compilation
  var fileContent = File.ReadAllLines(System.IO.Path.Combine(env.WebRootPath, "AddedColumnsConfig.txt"));
  var additionalColumnSet = fileContent.Where(x => !x.StartsWith("#")).ToList();

  Console.WriteLine("Loading Configuration...");
  // List of additional columns must be set before creating the first instance of the context!
  if (additionalColumnSet.Count > 0)
  {
   DA.Context.AdditionalColumnSet = additionalColumnSet;
   Console.WriteLine("AdditionalColumnSet=" + String.Join("\n", additionalColumnSet));
  }
  #endregion

  #region Load configuration
  //System.Environment.SetEnvironmentVariable("ConnectionStrings:MiracleListDB", "");

  // Get all configuration sources
  // NUGET: Microsoft.Extensions.Configuration.JSON
  // NUGET: Microsoft.Extensions.Configuration.EnvironmentVariables
  var builder = new ConfigurationBuilder()
      .SetBasePath(env.ContentRootPath)
      .AddInMemoryCollection()
      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
      .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
      .AddEnvironmentVariables(); // only loads process env variables https://github.com/aspnet/Configuration/issues/721

  if (env.EnvironmentName == "Development")
  {
   // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
#pragma warning disable CS0618 // Type or member is obsolete
   builder.AddApplicationInsightsSettings(developerMode: true);
#pragma warning restore CS0618 // Type or member is obsolete
   // Connect to EFCore Profiler
   //HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();
  }
  else
  {
   // nothing to do currently
  }

  // build configuration now
  Configuration = builder.Build();

  var CS = Configuration["ConnectionStrings:MiracleListDB"];
  // Inject connection string into DAL
  DA.Context.IsRuntime = true;
  DA.Context.ConnectionString = CS;
  #endregion

  #region testuser

  try
  {
   if (env.EnvironmentName == "Development")
   {
    Console.WriteLine("Creating user 'test'..");
    var um = new UserManager("test", true, true);
    um.InitDefaultTasks();
   }
   else
   {
    Console.WriteLine("Testing Database Access...");
    UserManager.GetUserStatistics();
    CUI.PrintSuccess("OK");

   }
  }
  catch (Exception ex)
  {

   CUI.PrintError("ERROR: " + ex.Message);
  }

  #endregion
 }

 /// <summary>
 /// Called by ASP.NET Core during startup
 /// </summary>
 public void ConfigureServices(IServiceCollection services)
 {
  CUI.Headline("ConfigureServices");

  #region Configuration/Settings
  // Make configuration available everywhere
  services.AddSingleton(Configuration);
  #endregion

  #region Enable Auth service for MLToken in the HTTP header
  services.AddAuthentication().AddMLToken();
  #endregion

  #region Enable App Insights
  services.AddApplicationInsightsTelemetry(Configuration);
  #endregion

  #region JSON configuration: no circular references and ISO date format
  services.AddMvc(
   opt =>
   { // null soll nicht 204 liefern! https://weblog.west-wind.com/posts/2020/Feb/24/Null-API-Responses-and-HTTP-204-Results-in-ASPNET-Core
    opt.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
   }
    )
     //.SetCompatibilityVersion(CompatibilityVersion.Version_3_0) // war seit v2.2 notwendig
     .AddNewtonsoftJson(options =>
     {

      options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
      options.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
      options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
     });
  #endregion

  #region DI

  services.AddScoped(typeof(MiracleListEnvInfo));
  #endregion

  #region Sessions (used in Razor Pages)
  services.AddMemoryCache();
  services.AddSession(o =>
  {
   //o.CookieName = "MiracleListeBackend.Cookie"; // Name festlegen --> schon wieder veraltert _:-(, nicht mehr in 3.0
   o.Cookie.Name = "MiracleListeBackend.Cookie"; // Name festlegen --> neu in v2.0
   //o.CookieSecure = CookieSecurePolicy.SameAsRequest;
   o.IdleTimeout = TimeSpan.FromMinutes(10); // Timeout 10 Minuten
   o.Cookie.HttpOnly = false; // auch für Client-Skript verfügbar machen
  });
  #endregion

  #region Enable MVC (this includes WebAPI Controllers and ASP.NET Core MVC+WebPages)
  services.AddMvc(options =>
  {
   // Exception Filter
   options.Filters.Add(typeof(GlobalExceptionFilter));
   //options.Filters.Add(typeof(GlobalExceptionAsyncFilter)); 
   options.Filters.Add(typeof(LoggingActionFilter));
  });
  #endregion

  #region Enable CORS 
  services.AddCors();
  #endregion


  #region Swagger
  services.AddSwaggerGen(options =>
  {

   // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1269
   //c.DescribeAllEnumsAsStrings(); // Important for Enums!
   options.SchemaFilter<EnumSchemaFilter>();

   options.SwaggerDoc("v1", new OpenApiInfo
   {
    Version = "v1",
    Title = "MiracleList API",
    Description = "Backend for MiracleList.de with token in URL",

    Contact = new OpenApiContact { Name = "Holger Schwichtenberg", Email = "", Url = new Uri("http://it-visions.de/kontakt") }
   });

   options.SwaggerDoc("v2", new OpenApiInfo
   {
    Version = "v2",
    Title = "MiracleList API",
    Description = "Backend for MiracleList.de with token in HTTP header",

    Contact = new OpenApiContact { Name = "Holger Schwichtenberg", Email = "", Url = new Uri("http://it-visions.de/kontakt") }
   });

   // Adds tokens as header parameters
   options.OperationFilter<SwaggerTokenHeaderParameter>();

   // include XML comments in Swagger doc
   var basePath = PlatformServices.Default.Application.ApplicationBasePath;
   if (String.IsNullOrEmpty(basePath)) basePath = System.Environment.CurrentDirectory;
   var xmlPath = Path.Combine(basePath, "Miraclelist_WebAPI.xml");
   options.IncludeXmlComments(xmlPath);
  });
  #endregion

  #region SignalR
  services.AddSignalR(
   (options) =>
   {
    //options.ClientTimeoutInterval = new TimeSpan(0,0,90);
    //options.HandshakeTimeout = new TimeSpan(0, 0, 90);
    //options.KeepAliveInterval = new TimeSpan(0, 0, 90);
   }
   ).AddMessagePackProtocol();

  #endregion

  services.AddSingleton<EnvInfo>(new EnvInfo());
 }

 /// <summary>
 /// Called by ASP.NET Core during startup
 /// </summary>
 public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
 {
  #region Error handling

  app.UseExceptionHandler(errorApp =>
  {
   errorApp.Run(async context =>
   {
    context.Response.StatusCode = 500;
    context.Response.ContentType = "text/plain";

    var error = context.Features.Get<IExceptionHandlerFeature>();
    if (error != null)
    {
     var ex = error.Error;
     await context.Response.WriteAsync("ASP.NET Core Exception Middleware:" + ex.ToString());
    }
   });
  });

  // ---------------------------- letzte Fehlerbehandlung: Fehlerseite für HTTP-Statuscode
  app.UseStatusCodePages();

  #endregion

  #region ASP.NET Core services
  app.UseSession();  // Sessions aktivieren
  app.UseDefaultFiles();
  app.UseStaticFiles();
  app.UseDirectoryBrowser();
  app.UseRouting(); // seit 2.2
  #endregion

  #region Authentication+Authorization

  // seit 3.0: For most apps, calls to UseAuthentication, UseAuthorization, and UseCors must appear between the calls to UseRouting and UseEndpoints to be effective. 
  app.UseAuthentication();
  app.UseAuthorization();
  //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
  //loggerFactory.AddDebug();
  #endregion

  #region CORS
  // Namespace: using Microsoft.AspNet.Cors;
  app.UseCors(builder =>
  builder.SetIsOriginAllowed((x) => true) // statt .AllowAnyOrigin(), notwendig für SignalR-JavaScript-Client, da The CORS protocol does not allow specifying a wildcard (any) origin and credentials at the same time. Configure the CORS policy by listing individual origins if credentials needs to be supported
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials() // notwendig für SignalR-JavaScript-Client, siehe https://docs.microsoft.com/en-us/aspnet/core/signalr/javascript-client?view=aspnetcore-6.0
          );
  #endregion

  #region Websockets CORS 

  //// bei Bedarf: siehe https://docs.microsoft.com/de-de/aspnet/core/fundamentals/websockets?view=aspnetcore-3.1#websocket-origin-restriction

  var webSocketOptions = new WebSocketOptions()
  {
   KeepAliveInterval = TimeSpan.FromSeconds(120), // How frequently to send "ping" frames to the client to ensure proxies keep the connection open. The default is two minutes. https://docs.microsoft.com/en-us/aspnet/core/fundamentals/websockets?view=aspnetcore-6.0#websocket-origin-restriction
  };
  webSocketOptions.AllowedOrigins.Any();

  app.UseWebSockets(webSocketOptions);
  #endregion

  #region Swagger
  // NUGET: Install-Package Swashbuckle.AspNetCore
  // Namespace: using Swashbuckle.AspNetCore.Swagger;
  app.UseSwagger(c =>
  {
  });

  // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
  app.UseSwaggerUI(c =>
  {
   c.SwaggerEndpoint("/swagger/v1/swagger.json", "MiracleList v1");
   c.SwaggerEndpoint("/swagger/v2/swagger.json", "MiracleList v2");
  });
  #endregion

  #region  MVC with Routing
  app.UseEndpoints(endpoints =>
  {
   endpoints.MapRazorPages();
   //routes.MapAreaRoute("blog_route", "StandardPages",
   //"Standard/{controller}/{action}/{id?}");

   endpoints.MapControllerRoute(
               name: "default",
               pattern: "{controller}/{action}/{id?}",
               defaults: new { controller = "Home", action = "Index" });

   // iX tutorial 2017
   endpoints.MapControllerRoute(
             name: "iX",
             pattern: "iX",
             defaults: new { controller = "Client", action = "Index" });

   // Schulungsteilnehmer ab Jan 2017
   endpoints.MapControllerRoute(
            name: "Schulung",
            pattern: "Schulung",
            defaults: new { controller = "Client", action = "Index" });

   // für ASP.NET SignalR
   endpoints.MapHub<MLHub>("/MLHub");
   endpoints.MapHub<MLHubV2>("/MLHubV2");
  });
  #endregion
 }
}

public class GlobalExceptionFilter : IExceptionFilter
{
 public void OnException(ExceptionContext context)
 {
  if (context.Exception is UnauthorizedAccessException)
  {
   context.HttpContext.Response.StatusCode = 403;
  }
  else
  {
   context.HttpContext.Response.StatusCode = 500;
  }
  context.HttpContext.Response.ContentType = "text/plain";

  var s = "MiracleListBackend v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
  s += " on ASP.NET Core v" + typeof(WebHost).Assembly.GetName().Version.ToString() + " GlobalExceptionFilter: ";

  context.HttpContext.Response.WriteAsync(s + context.Exception.ToString());

  context.HttpContext.Response.WriteAsync("\n---System Info:\n");

  IEnumerable<String> e = (context.HttpContext.RequestServices.GetService(typeof(EnvInfo)) as EnvInfo).GetStringList();

  context.HttpContext.Response.WriteAsync(String.Join("\n", e));
 }
}

public class GlobalExceptionAsyncFilter : IAsyncExceptionFilter
{
 public Task OnExceptionAsync(ExceptionContext context)
 {
  context.HttpContext.Response.StatusCode = 500;
  context.HttpContext.Response.ContentType = "text/plain";
  return context.HttpContext.Response.WriteAsync("MVC GlobalExceptionAsyncFilter:" + context.Exception.ToString());
 }
}