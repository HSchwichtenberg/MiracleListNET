//NUGET: Selenium.Support 
using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiracleListClientSeleniumTestsCore;
using OpenQA.Selenium;              // contains: IWebDriver
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;   // contains: WebDriverWait, ExpectedConditions 
using SeleniumTests;

namespace MiracleListUITests
{
 [TestClass]
 public class MainScreenTests
 {
  string anmeldename = "testuser " + DateTime.Now.ToString();
  string kennwort = "geheim";


  [TestMethod]
  public void TaskCreateTestPO()
  {
   var dir = (Directory.GetCurrentDirectory());
   using (IWebDriver b = Util.GetDriver())
   {
    string url = Util.GetConfig("URL");
    Console.WriteLine("Teste URL " + url);

    b.GoToUrlWithCheck(url);

    // Anmeldedialog
    var po = new AnmeldedialogPO(b);
    po.Username.Clear();
    po.Username.SendKeys(anmeldename);
    po.Password.Clear();
    po.Password.SendKeys(kennwort);
    po.Login.Click();

    // Main Page
    var mpo = new MainPO(b);
    mpo.Wait();

    Assert.IsTrue(b.Url.EndsWith("/main"));
    Assert.IsTrue(mpo.LoggedInUserText.Contains(anmeldename));

    //((ITakesScreenshot)b).GetScreenshot().SaveAsFile(@"Screenshot3.png", ScreenshotImageFormat.Png);

    //WebDriverWait wait = new WebDriverWait(b, TimeSpan.FromSeconds(Util.GetTimeoutSec()));
    //wait.Until(d => mpo.NewCategory != null);

    // Kategorie anlegen
    mpo.NewCategory.SendKeys("Testkategorie");
    mpo.NewCategory.SendKeys(Keys.Return);

    WebDriverWait wait2 = new WebDriverWait(b, TimeSpan.FromSeconds(Util.GetTimeoutSec()));
    wait2.Until(d => mpo.TaskHeadline.Text == "0 Tasks in Testkategorie" || mpo.TaskHeadline.Text == "0 Aufgaben in Testkategorie");

    var headline1 = mpo.TaskHeadline.Text;

    Assert.IsTrue(headline1 == "0 Tasks in Testkategorie" || headline1 == "0 Aufgaben in Testkategorie");

    // Aufgabe anlegen
    var taskTitle = "Testaufgabe " + DateTime.Now;
    mpo.NewTask.SendKeys(taskTitle);
    mpo.NewTask.SendKeys(Keys.Return);

    WebDriverWait wait3 = new WebDriverWait(b, TimeSpan.FromSeconds(Util.GetTimeoutSec()));
    wait3.Until(d => mpo.TaskHeadline != null && mpo.TaskHeadline.Text == "1 Tasks in Testkategorie" || mpo.TaskHeadline.Text == "1 Aufgaben in Testkategorie"); // Chrome ist seit v74 zu schnell!

    ((ITakesScreenshot)b).GetScreenshot().SaveAsFile(@"Screenshot_TaskCreateTestPO.png", ScreenshotImageFormat.Png);

    var headline2 = mpo.TaskHeadline.Text;

    Assert.IsTrue(headline2 == "1 Tasks in Testkategorie" || headline2 == "1 Aufgaben in Testkategorie");

    var taskElements = mpo.TaskSet.FindElements(By.CssSelector("li"));
    IWebElement task = taskElements[0];

    Assert.AreEqual(1, taskElements.Count);
    Assert.IsTrue(task.Text.Contains(taskTitle));

    // ENDE
    b.Quit();
   }
  }


  [TestMethod]
  public void TaskCreateTestPOExtrem()
  {

   var dir = (Directory.GetCurrentDirectory());
   using (IWebDriver b = Util.GetDriver())
   {
    string url = Util.GetConfig("URL");
    b.GoToUrlWithCheck(url);


    // Anmeldedialog
    var po = new AnmeldedialogPO(b);

    po.SetName(anmeldename);
    po.SetPassword(kennwort);
    po.ClickLogin();

    // Main Page
    var mpo = new MainPO(b);
    mpo.Wait();

    Assert.IsTrue(b.Url.EndsWith("/main"));

    Assert.IsTrue(mpo.LoggedInUserText.Contains(anmeldename));

    // Kategorie anlegen
    mpo.NewCategory.SendKeys("Testkategorie");
    mpo.NewCategory.SendKeys(Keys.Return);

    WebDriverWait wait2 = new WebDriverWait(b, TimeSpan.FromSeconds(Util.GetTimeoutSec()));
    wait2.Until(d => mpo.TaskHeadline.Text == "0 Tasks in Testkategorie" || mpo.TaskHeadline.Text == "0 Aufgaben in Testkategorie");

    var headline1 = mpo.TaskHeadline.Text;

    Assert.IsTrue(headline1 == "0 Tasks in Testkategorie" || headline1 == "0 Aufgaben in Testkategorie");

    // Aufgabe anlegen
    var taskTitle = "Testaufgabe " + DateTime.Now;
    mpo.NewTask.SendKeys(taskTitle);
    mpo.NewTask.SendKeys(Keys.Return);

    System.Threading.Thread.Sleep(2000); // Chrome ist seit v74 zu schnell!

    WebDriverWait wait3 = new WebDriverWait(b, TimeSpan.FromSeconds(Util.GetTimeoutSec()));
    wait3.Until(d => mpo.TaskHeadline.Text == "1 Tasks in Testkategorie" || mpo.TaskHeadline.Text == "1 Aufgaben in Testkategorie");

    ((ITakesScreenshot)b).GetScreenshot().SaveAsFile(@"Screenshot4.png", ScreenshotImageFormat.Png);
    //  ((ITakesScreenshot)b).GetScreenshot().SaveAsFile(@"Screenshot3.png", ScreenshotImageFormat.Png);
    var headline2 = mpo.TaskHeadline.Text;

    Assert.IsTrue(headline2 == "1 Tasks in Testkategorie" || headline2 == "1 Aufgaben in Testkategorie");

    var taskElements = mpo.TaskSet.FindElements(By.CssSelector("li"));

    Assert.AreEqual(1, taskElements.Count);
    Assert.IsTrue(taskElements[0].Text.Contains(taskTitle));

    // ENDE
    b.Quit();
   }
  }

  //[TestMethod]
  //public void TaskCreateAndRemoveTest()
  //{
  // var dir = (Directory.GetCurrentDirectory());
  // using (IWebDriver b = Util.GetDriver())
  // {
  //  string url = Util.GetConfig("URL");
  //  Console.WriteLine("Teste URL " + url);

  //  b.GoToUrlWithCheck(url);

  //  // Anmeldedialog
  //  var po = new AnmeldedialogPO(b);
  //  po.Name.Clear();
  //  po.Name.SendKeys(anmeldename);
  //  po.Password.Clear();
  //  po.Password.SendKeys(kennwort);
  //  po.Anmelden.Click();

  //  // Main Page
  //  var mpo = new MainPO(b);
  //  mpo.Wait();

  //  Assert.IsTrue(b.Url.EndsWith("/main"));
  //  Assert.IsTrue(mpo.LoggedInUserText.Contains(anmeldename));

  //  //((ITakesScreenshot)b).GetScreenshot().SaveAsFile(@"Screenshot3.png", ScreenshotImageFormat.Png);

  //  //WebDriverWait wait = new WebDriverWait(b, TimeSpan.FromSeconds(Util.GetTimeoutSec()));
  //  //wait.Until(d => mpo.NewCategory != null);

  //  // Kategorie anlegen
  //  mpo.NewCategory.SendKeys("Testkategorie");
  //  mpo.NewCategory.SendKeys(Keys.Return);

  //  WebDriverWait wait2 = new WebDriverWait(b, TimeSpan.FromSeconds(Util.GetTimeoutSec()));
  //  wait2.Until(d => mpo.TaskHeadline.Text == "0 Tasks in Testkategorie" || mpo.TaskHeadline.Text == "0 Aufgaben in  Testkategorie");

  //  var headline1 = mpo.TaskHeadline.Text;

  //  Assert.AreEqual("0 Tasks in Testkategorie", headline1);

  //  // Aufgabe anlegen
  //  var taskTitle = "Testaufgabe " + DateTime.Now;
  //  mpo.NewTask.SendKeys(taskTitle);
  //  mpo.NewTask.SendKeys(Keys.Return);

  //  WebDriverWait wait3 = new WebDriverWait(b, TimeSpan.FromSeconds(Util.GetTimeoutSec()));
  //  wait3.Until(d => mpo.TaskHeadline != null && mpo.TaskHeadline.Text == "1 Tasks in Testkategorie" || mpo.TaskHeadline.Text == "1 Aufgaben in Testkategorie"); // Chrome ist seit v74 zu schnell!

  //  ((ITakesScreenshot)b).GetScreenshot().SaveAsFile(@"Screenshot_TaskCreateTestPO.png", ScreenshotImageFormat.Png);

  //  var headline2 = mpo.TaskHeadline.Text;

  //  Assert.AreEqual("1 Tasks in Testkategorie", headline2);

  //  var taskElements = mpo.TaskSet.FindElements(By.CssSelector("li"));
  //  IWebElement task = taskElements[0];

  //  Assert.AreEqual(1, taskElements.Count);
  //  Assert.IsTrue(task.Text.Contains(taskTitle));


  //  // Fallunterscheidung Remove-ICON mit ID "Remove" gibt es noch nicht in Angular-Client, denn hier erfolgt das über Kontextmenü
  //  if (b.HasSubElement(task, "Remove"))
  //  {
  //   var removeIcon = task.FindElement(By.Id("Remove"));
  //   removeIcon.Click();
  //   // Standarddialog akzeptieren
  //   var message = b.SwitchTo().Alert().Text;
  //   b.SwitchTo().Alert().Accept();
  //  }
  //  else
  //  { // Kontextmenü öffnen und zweiten Eintrag wählen
  //   Actions actions = new Actions(b);
  //   actions.ContextClick(task).SendKeys(Keys.ArrowDown).SendKeys(Keys.ArrowDown).SendKeys(Keys.Return).Perform();
  //   System.Threading.Thread.Sleep(100);
  //   // Daten im eigenen Dialog auf "Löschen" klicken 
  //   //<button class="btn btn-primary">Löschen</button>
  //   b.FindElement(By.XPath("//button[text()='Löschen']")).Click();
  //  }

  //  // nun sollte die Liste wieder leer sein

  //  WebDriverWait wait4 = new WebDriverWait(b, TimeSpan.FromSeconds(Util.GetTimeoutSec()));
  //  wait3.Until(d => mpo.TaskHeadline != null && mpo.TaskHeadline.Text == "0 Tasks in Testkategorie" || mpo.TaskHeadline.Text == "0 Aufgaben in Testkategorie"); // Chrome ist seit v74 zu schnell!

  //  taskElements = mpo.TaskSet.FindElements(By.CssSelector("li"));
  //  Assert.AreEqual(0, taskElements.Count);

  //  b.Quit();
  // }
  //}


  [TestMethod]
  public void TaskCreateMany()
  {

   var dir = (Directory.GetCurrentDirectory());
   using (IWebDriver b = Util.GetDriver())
   {
    string url = Util.GetConfig("URL");
    b.GoToUrlWithCheck(url);

    // Anmeldedialog
    var po = new AnmeldedialogPO(b);

    po.SetName(anmeldename);
    po.SetPassword(kennwort);
    po.ClickLogin();

    // Main Page
    var mpo = new MainPO(b);
    mpo.Wait();

    Assert.IsTrue(b.Url.EndsWith("/main"));

    Assert.IsTrue(mpo.LoggedInUserText.Contains(anmeldename));
    Console.WriteLine(mpo.LoggedInUserText);
    // Kategorie anlegen
    mpo.NewCategory.SendKeys("Testkategorie");
    mpo.NewCategory.SendKeys(Keys.Return);

    WebDriverWait wait2 = new WebDriverWait(b, TimeSpan.FromSeconds(Util.GetTimeoutSec()));
    wait2.Until(d => mpo.TaskHeadline.Text == "0 Tasks in Testkategorie" || mpo.TaskHeadline.Text == "0 Aufgaben in Testkategorie");

    var headline1 = mpo.TaskHeadline.Text;
    Assert.IsTrue(headline1 == "0 Tasks in Testkategorie" || headline1 == "0 Aufgaben in Testkategorie");

    // 10 Aufgaben anlegen
    for (int i = 1; i <= 10; i++)
    {
     var taskTitle = "Testaufgabe " + i;
     mpo.NewTask.SendKeys(taskTitle);
     mpo.NewTask.SendKeys(Keys.Return);

     WebDriverWait wait3 = new WebDriverWait(b, TimeSpan.FromSeconds(Util.GetTimeoutSec()));
     wait3.Until(d => mpo.TaskHeadline != null && (mpo.TaskHeadline.Text == i + " Tasks in Testkategorie" || mpo.TaskHeadline.Text == i + " Aufgaben in Testkategorie"));

     ((ITakesScreenshot)b).GetScreenshot().SaveAsFile($@"Screenshot_Task#{i}.png", ScreenshotImageFormat.Png);
     //  ((ITakesScreenshot)b).GetScreenshot().SaveAsFile(@"Screenshot3.png", ScreenshotImageFormat.Png);
     var headline2 = mpo.TaskHeadline.Text;

     Assert.IsTrue(headline2 == i + " Tasks in Testkategorie" || headline2 == i + " Aufgaben in Testkategorie");

     var taskElements = mpo.TaskSet.FindElements(By.CssSelector("li"));

     Assert.AreEqual(i, taskElements.Count);

     Assert.IsTrue(taskElements[i - 1].Text.Contains(taskTitle));

     Console.WriteLine(taskTitle + ": OK!");
    }

    b.Quit();
   }
  }
 }
}