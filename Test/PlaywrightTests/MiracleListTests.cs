using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using ITVisions;
using ITVisions.Reflection;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace PlaywrightTests;

[TestClass]
public class MiracleListTests : PageTest
{
 int anzahlAufgaben = 5;

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

  string URL = TestContext.Properties["URL"].ToString();
  Assert.IsTrue(URL.IsNotNullOrEmpty());
  await Page.GotoAsync(URL);

  // Expect a title "to contain" a substring.
  await Expect(Page).ToHaveTitleAsync(new Regex("MiracleList_BS"));

  // Login-Formular ausfüllen
  //await Page.GetByPlaceholder("Ihre E-Mail-Adresse").FillAsync(anmeldename);
  //await Page.GetByPlaceholder("Ihr Kennwort").FillAsync(kennwort);
  //await Page.GetByRole(AriaRole.Button, new() { Name = "Anmelden" }).ClickAsync();
  // oder
  await Page.FillAsync("#username", anmeldename);
  await Page.FillAsync("#password", kennwort);
  await Page.ClickAsync("#login");

  // Nun sollten wir auf der Hauptseite sein, der Titel sollte aber gleich sein
  await Expect(Page).ToHaveURLAsync(new Regex("main"));
  await Expect(Page).ToHaveTitleAsync(new Regex("MiracleList_BS"));

  // Der Anmeldename sollte dargestellt sein
  await Expect(Page.Locator("#LoggedInUser").Nth(0)).ToContainTextAsync(anmeldename);

 }

 [TestMethod]
 public async Task AufgabenAnlegenUndLoeschen()
 {
  await Login();

  // Kein direkter Zugang zu children() 
  #region Prüfe korrektes Rendern der Kategorieliste

  // Wähle das <ol>-Element aus
  var col1List = await Page.QuerySelectorAsync("#col1 ol");

  // Überprüfe, ob es ein <ol>-Element gibt
  Assert.IsNotNull(col1List);

  // // 1 bis n-1 sind <li> Elemente (Letztes Element ist <input>)
  // Das ist leider umständlich in Playwright, siehe auch https://github.com/microsoft/playwright/issues/17703 und https://github.com/microsoft/playwright/issues/4845
  await PlaywrightUtil.VerifyChilddren(col1List, "li", 1);
  Assert.AreEqual("input", await PlaywrightUtil.GetLastChildTag(col1List));
  #endregion

  #region Kategorien anlegen
  int anzKategorienVorher = (await Page.Locator("#categoryCount").InnerTextAsync()).ToInt32();
  await Page.GetByPlaceholder("Neue Kategorie...").ClickAsync();
  await Page.GetByPlaceholder("Neue Kategorie...").FillAsync("Kat1");
  await Page.GetByPlaceholder("Neue Kategorie...").PressAsync("Enter");
  await Page.GetByPlaceholder("Neue Kategorie...").FillAsync("Kat2");
  await Page.GetByPlaceholder("Neue Kategorie...").PressAsync("Enter");
  await Expect(Page.Locator("#categoryCount")).ToHaveTextAsync((anzKategorienVorher + 2).ToString());
  #endregion

  #region Aufgaben anlegen
  for (int i = 1; i <= anzahlAufgaben; i++)
  {
   await Expect(Page.Locator("#taskCount")).ToHaveTextAsync((i - 1).ToString());
   await Page.GetByPlaceholder("Neue Aufgabe...").ClickAsync();
   await Page.GetByPlaceholder("Neue Aufgabe...").FillAsync("Aufgabe #" + i);
   await Page.GetByPlaceholder("Neue Aufgabe...").PressAsync("Enter");
   await Expect(Page.Locator("#taskCount")).ToHaveTextAsync(i.ToString());
  }
  #endregion

  #region Erste drei Aufgaben abharken
  var list = Page.Locator("#col2 ol li input");
  await list.Nth(2).CheckAsync();
  await list.Nth(1).CheckAsync();
  await list.Nth(0).CheckAsync();

  for (int i = 0; i < 3; i++)
  {
   await Expect(Page.Locator("#col2 ol li input").Nth(i)).ToBeCheckedAsync();
  }
  #endregion

  #region Alle Aufgaben löschen: Immer die oberste löschen
  for (int i = 0; i < anzahlAufgaben; i++)
  {
   await Page.Locator("#col2 #Remove").Nth(0).ClickAsync();
   await Page.GetByRole(AriaRole.Button, new() { Name = "Yes" }).ClickAsync();
  }
  await Expect(Page.Locator("#taskCount")).ToHaveTextAsync("0");
  #endregion
 }

 [TestMethod]
 public async Task AufgabenBearbeiten()
 {
  string aufgabenTitel = "Testaufgabe " + Guid.NewGuid();

  await Context.Tracing.StartAsync(new()
  {
   Screenshots = true,
   Snapshots = true,
   Sources = true
  });

  await Login();
  await Page.ScreenshotAsync();

  var count = (await Page.Locator("#taskCount").InnerTextAsync()).ToInt32();

  // Neue Aufgabe anlegen
  await Page.GetByPlaceholder("Neue Aufgabe...").ClickAsync();
  await Page.GetByPlaceholder("Neue Aufgabe...").FillAsync(aufgabenTitel);
  await Page.GetByPlaceholder("Neue Aufgabe...").PressAsync("Enter");
  await Expect(Page.Locator("#taskCount")).ToHaveTextAsync((count + 1).ToString());

  // Neue Aufgabe zur aktiven Aufgabe machen
  await Page.GetByText(aufgabenTitel).ClickAsync();

  #region Testaufgabe ändern und speichern
  await Page.GetByLabel("Titel").ClickAsync();
  aufgabenTitel += " geändert";
  await Page.GetByLabel("Titel").FillAsync(aufgabenTitel);
  await Page.GetByLabel("Wichtigkeit").SelectOptionAsync(new[] { "A" });
  await Page.Locator("#taskeffort").FillAsync("2.5");
  await Page.Locator("#taskdue").FillAsync("2023-09-01");

  var inputNeueAufgabe = Page.GetByPlaceholder("Neue Unteraufgabe...");
  await inputNeueAufgabe.ClickAsync();
  await inputNeueAufgabe.FillAsync("Unteraufgabe 1");
  await inputNeueAufgabe.PressAsync("Enter");
  await inputNeueAufgabe.FillAsync("Unteraufgabe 2");
  await inputNeueAufgabe.PressAsync("Enter");
  await inputNeueAufgabe.FillAsync("Unteraufgabe 3");
  await inputNeueAufgabe.PressAsync("Enter");

  await Page.GetByLabel("Notizen").FillAsync("Eine Notiz zur Testaufgabe");

  await Page.GetByRole(AriaRole.Button, new() { Name = "Speichern" }).ClickAsync();
  #endregion

  await Expect(Page.GetByText(aufgabenTitel)).ToHaveTextAsync(aufgabenTitel);

  // Suche eine CSS-Klasse list-group-item, die einen Text beinhaltet. Suche in dem list-group-item die Klasse badge
  await Expect(Page.Locator(".list-group-item").Filter(new() { Has = Page.GetByText(aufgabenTitel) }).Locator(".badge")).ToHaveTextAsync("A");

  await Page.ScreenshotAsync();

  // Stop tracing and export it into a zip archive.
  await Context.Tracing.StopAsync(new()
  {
   Path = "trace.zip"
  });
 }
}