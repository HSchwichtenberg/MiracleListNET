//NUGET: Selenium.Support 
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;              // contains: IWebDriver
using OpenQA.Selenium.Support.UI;   // contains: WebDriverWait, ExpectedConditions 
using SeleniumTests;
using System;

namespace MiracleListUITests
{

 /// <summary>
 /// Diese Klasse testet nur die Test-Infrastruktur
 /// </summary>
 [TestClass]
 public class TestTest
 {
  [TestMethod]
  public void Run()
  {
   Console.WriteLine("Console.WriteLine");
   System.Diagnostics.Trace.WriteLine("System.Diagnostics.Trace.WriteLine");
   System.Diagnostics.Debug.WriteLine("System.Diagnostics.Debug.WriteLine");
   Assert.IsTrue(true);
  }

 }
}