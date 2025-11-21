using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace PlaywrightTests;

[TestClass]
public class MiracleListTests : PageTest
{
 int anzahlKategorien = 5;
 int anzahlAufgaben = 5;

 string anmeldename = "testuser " + DateTime.Now.ToString();
 string kennwort = "geheim";

 [TestInitialize()]
 public async Task Initialize()
 {
  #region Optional: Tracing starten
  await Context.Tracing.StartAsync(new()
  {
   Screenshots = true,
   Snapshots = true,
   Sources = true,
   Title = TestContext.TestName
  });
  #endregion
 }

 [TestCleanup()]
 public async Task Cleanup()
 {
  // Tracing beenden und speichern in ZIP-Datei pro Test
  await Context.Tracing.StopAsync(new()
  {
   Path = TestContext.TestName + ".zip"
  });
 }

 [TestMethod]
 public async Task Login()
 {
  #region Optionale TestRunner-Konfiguration per Code
  //await using var browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
  //{
  // Headless = true,
  // SlowMo = 1000,
  //});
  //var context = await browser.NewContextAsync();
  #endregion

  string URL = TestContext?.Properties["URL"]?.ToString() ?? "";
  Console.WriteLine("Öffne: " + URL);
  Assert.IsTrue(URL.IsNotNullOrEmpty());
  await Page.GotoAsync(URL);
  Console.WriteLine("Geöffnet: " + URL);
  await Page.PauseAsync(); // Öffnet Inspector und pausiert hier
  // Expect a title "to contain" a substring.
  await Expect(Page).ToHaveTitleAsync(new Regex("MiracleList_B"));

  // Login-Formular ausfüllen: Suche über Placeholder
  //await Page.GetByPlaceholder("Ihre E-Mail-Adresse").FillAsync(anmeldename);
  //await Page.GetByPlaceholder("Ihr Kennwort").FillAsync(kennwort);
  //await Page.GetByRole(AriaRole.Button, new() { Name = "Anmelden" }).ClickAsync();
  // oder Suche über ID
  await Page.FillAsync("#username", anmeldename);
  await Page.FillAsync("#password", kennwort);
  await Page.ClickAsync("#login");
  await Page.ScreenshotAsync(new() { Path = "VorLogin.png" });

  // Nun sollten wir auf der Hauptseite sein, der Titel sollte aber gleich sein
  await Expect(Page).ToHaveURLAsync(new Regex("main"));
  await Expect(Page).ToHaveTitleAsync(new Regex("MiracleList"));

  // Der Anmeldename sollte auf dem Bildschirm dargestellt sein
  await Expect(Page.Locator("#LoggedInUser").Nth(0)).ToContainTextAsync(anmeldename);
  await Page.ScreenshotAsync(new() { Path = "NachLogin.png" });
 }

 [TestMethod]
 public async Task AufgabenAnlegenUndLoeschen()
 {
  await Login();

  #region Prüfe korrektes Rendern der Kategorieliste
  // Wähle das <ol>-Element aus
  var col1List = await Page.QuerySelectorAsync("#col1 ol");
  // Überprüfe, ob es das <ol>-Element gibt
  Assert.IsNotNull(col1List);

  // // 1 bis n-1 sind <li> Elemente (Letztes Element ist <input>)
  // Zugriff auf die Children ist leider umständlich in Playwright, siehe auch https://github.com/microsoft/playwright/issues/17703 und https://github.com/microsoft/playwright/issues/4845
  await PlaywrightUtil.VerifyChildren(col1List, "li", 1);
  Assert.AreEqual("input", await PlaywrightUtil.GetLastChildTag(col1List));
  #endregion

  #region Kategorien anlegen
  int anzKategorienVorher = (await Page.Locator("#categoryCount").InnerTextAsync()).ToInt32();
  for (int i = 1; i <= anzahlKategorien; i++)
  {
   await Page.GetByPlaceholder("Neue Kategorie...").FillAsync($"Testkategorie {i}");
   await Page.GetByPlaceholder("Neue Kategorie...").PressAsync("Enter");
   await Expect(Page.Locator("#categoryCount")).ToHaveTextAsync((anzKategorienVorher + i).ToString());
   // In der neuen Kategorie gibt es erstmal keine Aufgaben
   await Expect(Page.Locator("#taskCount")).ToHaveTextAsync("0");
  }
  await Page.ScreenshotAsync(new() { Path = "NachDemAnlegenDerKategorien.png" });
  #endregion

  #region Aufgaben anlegen
  for (int i = 1; i <= anzahlAufgaben; i++)
  {
   await Expect(Page.Locator("#taskCount")).ToHaveTextAsync((i - 1).ToString());
   await Page.GetByPlaceholder("Neue Aufgabe...").FillAsync("Testaufgabe #" + i);
   await Page.GetByPlaceholder("Neue Aufgabe...").PressAsync("Enter");
   await Expect(Page.Locator("#taskCount")).ToHaveTextAsync(i.ToString());
  }
  await Page.ScreenshotAsync(new() { Path = "NachDemAnlegenDerAufgabe.png" });
  #endregion

  #region Erste drei Aufgaben abharken
  var list = Page.Locator("#col2 ol li input");
  await list.Nth(2).CheckAsync();
  await list.Nth(1).CheckAsync();
  await list.Nth(0).CheckAsync();

  await Expect(Page.Locator("#col2 ol li input:checked")).ToHaveCountAsync(3);

  var inputs = await list.ElementHandlesAsync();
  foreach (IElementHandle input in inputs.Take(3))
  {
   bool isChecked = await input.IsCheckedAsync();
   Assert.IsTrue(isChecked, "Input isChecked " + await input.GetAttributeAsync("ID"));

   // ODER per Expect().ToBeCheckedAsync():
   // Aber leider kann man bei Excpect kein IElementHandle angeben, daher Umweg über Locator
   var locator = Page.Locator($"#{(await input.GetAttributeAsync("id"))}");
   await Assertions.Expect(locator).ToBeCheckedAsync();
  }

  // oder:
  //for (int i = 0; i < 3; i++)
  //{
  // await Expect(Page.Locator("#col2 ol li input").Nth(i)).ToBeCheckedAsync();
  //}
  await Page.ScreenshotAsync(new() { Path = "NachDemAbharken.png" });
  #endregion

  #region Alle Aufgaben löschen: 5x jeweils die oberste Aufgabe löschen
  for (int i = 0; i < anzahlAufgaben; i++)
  {
   await Page.Locator("#col2 #Remove").Nth(0).ClickAsync();
   await Page.GetByRole(AriaRole.Button, new() { Name = "Yes" }).ClickAsync();
  }
  await Expect(Page.Locator("#taskCount")).ToHaveTextAsync("0");
  await Page.ScreenshotAsync(new() { Path = "NachDemLoeschen.png" });
  #endregion
 }

 [TestMethod]
 public async Task AufgabenBearbeitenInklusiveUnteraufgaben()
 {
  string aufgabenTitel = "Testaufgabe " + Guid.NewGuid();

  await Login();

  // Counter vorher merken
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
  await Page.Locator("#taskdue").FillAsync(DateTime.Now.ToString("yyyy-MM-dd"));

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
 }

 /// <summary>
 /// Test wurde mit Testgenerator
 /// bin/Debug/net10.0/playwright.ps1 codegen miraclelist-bu.azurewebsites.net
 /// erzeugt und dann angepasst
 /// </summary>
 /// <returns></returns>
 [TestMethod]
 public async Task AufgabeBearbeitenUndLoeschen()
 {
  string TaskName = Guid.NewGuid().ToString();

  // statt statischer URL von Testgenerator
  string URL = TestContext?.Properties["URL"]?.ToString() ?? "";
  Assert.IsTrue(URL.IsNotNullOrEmpty());
  await Page.GotoAsync(URL);

  await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Benutzeranmeldung" })).ToBeVisibleAsync();

  await Page.GetByRole(AriaRole.Textbox, new() { Name = "Ihre E-Mail-Adresse" }).ClickAsync();
  await Page.GetByRole(AriaRole.Textbox, new() { Name = "Ihre E-Mail-Adresse" }).FillAsync("pwtest");
  await Page.GetByRole(AriaRole.Textbox, new() { Name = "Ihre E-Mail-Adresse" }).PressAsync("Tab");
  await Page.GetByRole(AriaRole.Textbox, new() { Name = "Kennwort Server" }).FillAsync("pwtest");
  await Page.GetByRole(AriaRole.Button, new() { Name = "Anmelden" }).ClickAsync();
  await Expect(Page.Locator("body")).ToContainTextAsync("pwtest");
  await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Logout" })).ToBeVisibleAsync();
  await Page.GetByRole(AriaRole.Textbox, new() { Name = "Neue Aufgabe..." }).ClickAsync();
  await Page.GetByRole(AriaRole.Textbox, new() { Name = "Neue Aufgabe..." }).FillAsync(TaskName);
  await Page.GetByRole(AriaRole.Textbox, new() { Name = "Neue Aufgabe..." }).PressAsync("Enter");
  await Expect(Page.GetByText(TaskName)).ToBeVisibleAsync();

  await Page.GetByText(TaskName).ClickAsync();
  await Expect(Page.Locator("body")).ToContainTextAsync("Bearbeiten der Aufgabe");
  await Page.GetByLabel("Wichtigkeit").SelectOptionAsync(new[] { "A" });
  await Page.GetByRole(AriaRole.Button, new() { Name = " Speichern" }).ClickAsync();
  await Expect(Page.Locator("#TaskSet")).ToContainTextAsync("A");
  await Page.GetByText(TaskName).ClickAsync(new()
  {
   Button = MouseButton.Right,
  });
  await Page.GetByText("Aufgabe löschen").ClickAsync();
  await Expect(Page.GetByText("Remove this Task?")).ToBeVisibleAsync();
  await Page.GetByRole(AriaRole.Button, new() { Name = "Yes" }).ClickAsync();

  // Manuell, da Test von Testgenerator nicht funktioniert hat
  // Warten bis es unterhalb von <ol id="TaskSet" kein Element mehr mit dem Text gibt
  var taskSet = Page.Locator("#TaskSet");
  await Expect(taskSet.GetByText(TaskName)).ToHaveCountAsync(0);
 }
}