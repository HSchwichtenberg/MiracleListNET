using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumTests;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiracleListClientSeleniumTestsCore
{
 class MainPO
 {

  private IWebDriver b;

  public MainPO(IWebDriver driver)
  {
   this.b = driver;
  }

  public IWebElement LoggedInUser
  {
   get
   {
    return b.FindElement(By.Id("LoggedInUser"));
   }
  }

  public IWebElement NewTask
  {
   get
   {
    return b.FindElement(By.Name("newTaskTitle"));

   }
  }

  public IWebElement NewCategory
  {
   get
   {
    return b.FindElement(By.Name("newCategoryName"));
   }
  }

  public IWebElement TaskHeadline
  {
   get
   {
    return b.FindElement(By.Id("TaskHeadline"));
   }
  }

  public IWebElement TaskSet
  {
   get
   {
    return b.FindElement(By.Id("TaskSet"));
   }
  }

  public string LoggedInUserText
  {
   get
   {
    var t = LoggedInUser.Text.Replace("Current User: ", "").Replace("User: ", "").Replace("Angemeldeter Benutzer: ", "");
    return t;
   }
  }

  internal void Wait()
  {
   WebDriverWait wait = new WebDriverWait(b, TimeSpan.FromSeconds(5));
   wait.Until(d => b.Url.EndsWith("/main"));
  }
 }
}