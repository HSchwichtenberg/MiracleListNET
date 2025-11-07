//NUGET: Selenium.Support 
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;              // contains: IWebDriver
using OpenQA.Selenium.Support.UI;   // contains: WebDriverWait, ExpectedConditions 
using SeleniumTests;

namespace MiracleListUITests
{
 [TestClass]
 public class LoginTest
 {
  // DEMO: 72. UI Tests mit Selenium
  [TestMethod]
  public void LoginSuccessTest()
  {
   string anmeldename = "testuser " + DateTime.Now.ToString();
   string kennwort = "geheim";

   string url = Util.GetConfig("URL");

   using (IWebDriver b = Util.GetDriver())
   {

    Console.WriteLine("Teste URL " + url + " mit " + b.GetType().FullName);

    //b.Manage().Window.Maximize();

    b.Navigate().GoToUrl(url);

    WebDriverWait wait = new WebDriverWait(b, TimeSpan.FromSeconds(5));
    wait.Until(d => b.FindElement(By.CssSelector("h2")));

    IWebElement headline = b.FindElement(By.CssSelector("h2")); // start
    Assert.IsTrue(headline.Text == "Benutzeranmeldung" || headline.Text == "User Login");

    // 2x Textbox
    b.FindElement(By.Id("username")).Clear();
    b.FindElement(By.Id("username")).SendKeys(anmeldename);
    b.FindElement(By.Id("password")).Clear();
    b.FindElement(By.Id("password")).SendKeys(kennwort);

    // Screenshot!
    ((ITakesScreenshot)b).GetScreenshot().SaveAsFile(@"Screenshot1.png");

    // Button
    b.FindElement(By.Id("login")).Click();

    //Thread.Sleep(1000); :-(

    wait.Until(d => b.Url.EndsWith("/main"));

    Assert.IsTrue(b.Url.EndsWith("/main"));

    ((ITakesScreenshot)b).GetScreenshot().SaveAsFile(@"Screenshot2.png");

    Assert.IsTrue(b.FindElement(By.Id("LoggedInUser")).Text.Contains(anmeldename));

    //// Wait for results to show
    //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    //wait.Until(d => ExpectedConditions.ElementIsVisible(By.Id("resultStats")));

    //// Extract results from webpage and print to log file
    //IWebElement lbl_Results = driver.FindElement(By.Id("resultStats"));
    //Console.WriteLine("Results: " + lbl_Results.Text);

    b.Quit();
   }
  }
 }
}