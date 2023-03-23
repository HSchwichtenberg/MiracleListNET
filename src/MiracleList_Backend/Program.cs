﻿using System;
using System.Reflection;
using ITVisions;
using ITVisions.Network;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MiracleList;

/// <summary>
/// Start with: dotnet run --hosturl http://192.168.1.66:5000
/// </summary>
public class Program
{

 /// <summary>
 /// Start code for ASP.NET Core >= 2.0, see https://docs.microsoft.com/en-us/aspnet/core/migration/1x-to-2x/
 /// </summary>
 /// <param name="args"></param>
 public static void Main(string[] args)
 {
  //BL.DataGenerator.Run();

  CUI.MainHeadline("MiracleList Backend v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

  Console.WriteLine();
  var versions = ITVisions.CLRInfo.GetClrVersion();

  CUI.Headline("Main");
  CUI.Print("OS: " + System.Environment.OSVersion);
  CUI.Print("Runtime: " + ITVisions.CLRInfo.GetClrVersion());
  CUI.Print("Webframework: ASP.NET Core v" + typeof(WebHost).Assembly.GetName().Version.ToString());

  var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
    .AddCommandLine(args)
    .Build();

  // set a hostURL to use if there is no hosturl in the config
  var hostUrl = configuration["hosturl"];
  if (string.IsNullOrEmpty(hostUrl))
   hostUrl = "http://localhost:8899";
  CUI.Print("HostURL: " + hostUrl);

  // Set Mailconfig
  MailUtil.SMTPServer = configuration["EMail:SMTPServer"];
  MailUtil.SMTPSender = configuration["EMail:SMTPSender"];
  MailUtil.SMTPPassword = configuration["EMail:SMTPPassword"];

  var builder = WebHost.CreateDefaultBuilder(args)
   .UseUrls(hostUrl)
   .UseSetting("detailedErrors", "true")
   .UseStartup<Startup>()
   .CaptureStartupErrors(true)
    .ConfigureLogging((hostingContext, logging) =>
    {

     // https://docs.microsoft.com/en-us/aspnet/core/signalr/diagnostics?view=aspnetcore-3.1

     logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
     logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);

     logging.AddConsole();
    })
   .Build();

  CUI.H3("Server Features:");
  foreach (var sf in builder.ServerFeatures)
  {
   Console.WriteLine(sf.Key + "=" + sf.Value);
   if (sf.Value is IServerAddressesFeature)
   {
    var saf = sf.Value as IServerAddressesFeature;
    foreach (var a in saf.Addresses)
    {
     Console.WriteLine(a);
    }
   }
  }

  builder.Run();
 }

 // old start code for ASP.NET Core 1.x
 //public static void Main(string[] args)
 //{
 // var host = new WebHostBuilder()
 //              .UseKestrel()
 //              .UseUrls(hostUrl)  
 //              .UseContentRoot(Directory.GetCurrentDirectory())
 //              .UseIISIntegration()
 //              .UseStartup<Startup>()
 //              .Build();

 // host.Run();
 //}

}
