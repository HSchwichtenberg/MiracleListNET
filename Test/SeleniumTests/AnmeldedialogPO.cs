using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using SeleniumTests;

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
   this.Username = driver.FindElement(By.Id("username"));
   this.Password = driver.FindElement(By.Id("password"));
   this.Login = driver.FindElement(By.Id("login"));
  }

  [FindsBy(How = How.Id, Using = "username")]
  public IWebElement Username;

  [FindsBy(How = How.Id, Using = "password")]
  public IWebElement Password;

  [FindsBy(How = How.Id, Using = "login")]
  public IWebElement Login;

  public void SetName(string anmeldename)
  {
   this.Username.Clear();
   this.Username.SendKeys(anmeldename);
  }

  public void SetPassword(string kennwort)
  {
   this.Password.Clear();
   this.Password.SendKeys(kennwort);
  }

  public void ClickLogin()
  {
   this.Login.Click();
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
