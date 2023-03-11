using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SeleniumTests
{
 public class Util
 {
  /// <summary>
  /// Lade Konfig aus Umgebunsgvariablen (Prio 1) oder AppSettings (Prio 2)
  /// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?tabs=basicconfiguration
  /// </summary>

  public static string GetConfig(string name)
  {

   //var dic = new Dictionary<string, string> { { "ConnectionStrings:MiracleListDB", "" } };

   var builder = new ConfigurationBuilder() // NUGET: Microsoft.Extensions.Configuration
   //.AddInMemoryCollection(dic)
   .AddJsonFile("appsettings.json") // NUGET: Microsoft.Extensions.Configuration.Json
   .AddEnvironmentVariables(); // NUGET: Microsoft.Extensions.Configuration.EnvironmentVariables

   IConfigurationRoot configuration = builder.Build();

   var e = configuration[name];
   return e;
  }

  public static int GetTimeoutSec()
  {
   return Int32.Parse(GetConfig("TimeoutSec"));
  }

  public static IWebDriver GetDriver()
  {
   IWebDriver driverObj;
   string driver = Util.GetConfig("Browser");
   switch (driver)
   {
    //https://automatetheplanet.com/webdriver-dotnetcore2/
    case "Chrome": driverObj = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)); break;
    case "Firefox":
     // notwendig für neuere Firefox-Versionen
     //System.Environment.SetEnvironmentVariable("webdriver.gecko.driver", @"C:\geckodriver.exe");
     driverObj = new FirefoxDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
     break;
    case "Edge": driverObj = new EdgeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)); break;
    default: driverObj = new FirefoxDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)); break;
   }
   // http://toolsqa.com/selenium-webdriver/implicit-explicit-n-fluent-wait/
   driverObj.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(GetTimeoutSec());
   driverObj.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(GetTimeoutSec());
   return driverObj;
  }
 }
}