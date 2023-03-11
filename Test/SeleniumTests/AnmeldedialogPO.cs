using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using SeleniumTests;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiracleListClientSeleniumTestsCore
{
 class AnmeldedialogPO
 {
  private IWebDriver driver;

  public AnmeldedialogPO(IWebDriver driver)
  {
   this.driver = driver;
   //PageFactory.InitElements(driver, this); // gibt es noch nicht in .NET Core :-(
   this.driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(Util.GetTimeoutSec());

   // Wäre unnötig mit Pagefactory
   this.Name = driver.FindElement(By.Id("name"));
   this.Password = driver.FindElement(By.Id("password"));
   this.Anmelden = driver.FindElement(By.Id("Anmelden"));
  }

  [FindsBy(How = How.Id, Using = "name")]
  public IWebElement Name;

  [FindsBy(How = How.Id, Using = "password")]
  public IWebElement Password;

  [FindsBy(How = How.Id, Using = "Anmelden")]
  public IWebElement Anmelden;

  public void SetName(string anmeldename)
  {
   this.Name.Clear();
   this.Name.SendKeys(anmeldename);
  }

  public void SetPassword(string kennwort)
  {
   this.Password.Clear();
   this.Password.SendKeys(kennwort);
  }

  public void ClickAnmelden()
  {
   this.Anmelden.Click();
  }

  public IWebElement LoggedInUser
  {
   get
   {
    return driver.FindElement(By.Id("LoggedInUser"));
   }
  }
 }
}
