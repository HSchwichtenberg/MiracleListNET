using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace PlaywrightTests.HelloWorld;

[TestClass]
public class MiracleListLogin : PageTest
{
 [TestMethod]
 public async Task Login()
 {
  string anmeldename = "testuser " + DateTime.Now.ToString();
  string kennwort = "geheim";

  //await using var browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
  //{
  // Headless = true,
  //});
  //var context = await browser.NewContextAsync();

  var page = Page;

  await page.GotoAsync("https://localhost:44387/"); //  https://miraclelist-bs.azurewebsites.net");

  // Expect a title "to contain" a substring.
  await Expect(page).ToHaveTitleAsync(new Regex("MiracleList_BS"));


  await page.GetByPlaceholder("Ihre E-Mail-Adresse").FillAsync(anmeldename);

  await page.GetByPlaceholder("Ihr Kennwort").FillAsync(kennwort);

  await page.GetByRole(AriaRole.Button, new() { Name = "Anmelden" }).ClickAsync();

  #region Kategorien und Aufgaben anlegen
  await page.GetByPlaceholder("Neue Kategorie...").ClickAsync();
  await page.GetByPlaceholder("Neue Kategorie...").FillAsync("Kat1");
  await page.GetByPlaceholder("Neue Kategorie...").PressAsync("Enter");
  await page.GetByPlaceholder("Neue Kategorie...").FillAsync("Kat2");
  await page.GetByPlaceholder("Neue Kategorie...").PressAsync("Enter");
  await page.GetByPlaceholder("Neue Aufgabe...").ClickAsync();
  await page.GetByPlaceholder("Neue Aufgabe...").FillAsync("Kat2A1");
  await page.GetByPlaceholder("Neue Aufgabe...").PressAsync("Enter");
  await page.GetByPlaceholder("Neue Aufgabe...").FillAsync("Kat2A2");
  await page.GetByPlaceholder("Neue Aufgabe...").PressAsync("Enter");
  await page.GetByPlaceholder("Neue Aufgabe...").ClickAsync();
  await page.GetByPlaceholder("Neue Aufgabe...").FillAsync("Kat2A4");
  await page.GetByPlaceholder("Neue Aufgabe...").PressAsync("Enter");
  await Expect(page.Locator("#taskCount")).ToHaveTextAsync("3");
  #endregion

  #region Alle Aufgaben abharken
  await page.Locator("#col2 ol li input").Nth(2).CheckAsync();
  await page.Locator("#col2 ol li input").Nth(1).CheckAsync();
  await page.Locator("#col2 ol li input").Nth(0).CheckAsync();

  for (int i = 0; i < 3; i++)
  {
   await Expect(page.Locator("#col2 ol li input").Nth(i)).ToBeCheckedAsync();
  }
  #endregion

  #region Alle Aufgaben löschen
  for (int i = 0; i < 3; i++)
  {
   await page.Locator("#col2 #Remove").Nth(0).ClickAsync();
   await page.GetByRole(AriaRole.Button, new() { Name = "Yes" }).ClickAsync();
  }
  await Expect(page.Locator("#taskCount")).ToHaveTextAsync("0");
  #endregion



 }
}
