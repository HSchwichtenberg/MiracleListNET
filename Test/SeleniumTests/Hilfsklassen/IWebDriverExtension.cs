//NUGET: Selenium.Support 
using OpenQA.Selenium;              // contains: IWebDriver
using OpenQA.Selenium.Support.UI;   // contains: WebDriverWait, ExpectedConditions 
using System;

namespace MiracleListUITests
{
 public static class IWebDriverExtension
 {

  public static void GoToUrlWithCheck(this IWebDriver driver, string url, byte timeout = 5)
  {
   Console.WriteLine("Teste URL " + url);
   driver.Navigate().GoToUrl(url);
   WebDriverWait wait1 = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
   wait1.Until(d => driver.Url.StartsWith(url)); // StartsWith, da Browser ggf "/" anhängt!
  }


  public static bool HasSubElement(this IWebDriver driver, IWebElement element, string id)
  {
   var oldWait = driver.Manage().Timeouts().ImplicitWait;
   driver.Manage().Timeouts().ImplicitWait = new TimeSpan(0);

   try
   {
    var subelement = element.FindElement(By.Id(id));
    if (subelement != null) return true;
    return false;
   }
   catch (Exception)
   {
    return false;
   }
   finally
   {
    driver.Manage().Timeouts().ImplicitWait = oldWait;
   }

  }

 }
}