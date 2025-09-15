using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using MiracleList;
using MiracleList_Backend.Hubs;
using Xunit.Abstractions;

namespace WebserviceTests.Integration.SignalR;

public class SignalRTests
{
 private readonly ITestOutputHelper _testOutputHelper;

 MiracleListProxy Proxy;

 static string BackendUrl = "https://miraclelistbackend.azurewebsites.net"; //"http://localhost:8889"; //"https://miraclelistbackend.azurewebsites.net"; 
 static string hubURL = $"{BackendUrl}/MLHubV2";

 public SignalRTests(ITestOutputHelper testOutputHelper)
 {
  _testOutputHelper = testOutputHelper;
  Proxy = new MiracleListProxy(new HttpClient());
  Proxy.BackendUrl = BackendUrl;
 }

 [Fact]
 private async Task ChangeTask()
 {
  // Benutzer erstellen und Token holen
  string username = Guid.NewGuid().ToString();
  string password = username;
  string clientID = "11111111-1111-1111-1111-111111111111";
  var loginInfo = new LoginInfo() { ClientID = clientID, Username = username, Password = password };
  var loginResult = await Proxy.LoginAsync(loginInfo);

  // Zwei SignalR-Verbindungen erstellen
  HubConnection hc1;
  HubConnection hc2;
  int categoryIDForTest = 123;
  int? result1 = null, result2 = null;

  hc1 = new HubConnectionBuilder()
      .WithUrl(hubURL)
      .AddMessagePackProtocol()
      .WithAutomaticReconnect()
      .Build();

  hc2 = new HubConnectionBuilder()
    .WithUrl(hubURL)
    .AddMessagePackProtocol()
    .WithAutomaticReconnect()
    .Build();

  // Reaktionen auf eingehende Nachricht
  hc1.On<string, int>(nameof(IMLHub.TaskListUpdate), async (sender, categoryID) =>
  {
   _testOutputHelper.WriteLine("TaskListUpdate..." + categoryID);
   result1 = categoryID;
  });

  hc2.On<string, int>(nameof(IMLHub.TaskListUpdate), async (sender, categoryID) =>
  {
   _testOutputHelper.WriteLine("TaskListUpdate..." + categoryID);
   result2 = categoryID;
  });

  // Verbindungen starten
  await hc1.StartAsync();
  await hc2.StartAsync();
  Assert.True(hc1.State == HubConnectionState.Connected);
  Assert.True(hc2.State == HubConnectionState.Connected);

  // Registrieren
  await hc1.SendAsync(nameof(IMLHub.Register), loginResult.Token);
  await hc2.SendAsync(nameof(IMLHub.Register), loginResult.Token);

  // Nachricht senden
  await hc2.SendAsync(nameof(IMLHub.TaskListUpdate), loginResult.Token, categoryIDForTest);

  // Warten auf Ergebnis, max 5 Sekunden
  var start = DateTime.Now;
  while (result1 == null && ((DateTime.Now - start).TotalSeconds < 5))
  {
   System.Threading.Thread.Sleep(50);
  }
  _testOutputHelper.WriteLine("Signal eingegangen nach: " + (DateTime.Now - start).TotalSeconds + " ms");

  // Prüfen
  Assert.True(categoryIDForTest == result1, "Bei der ersten HubConnection ist kein Signal eingegangen!");
  Assert.True(null == result2, "Bei der zweiten HubConnection ist ein Signal eingegangen, was nicht hätte sein dürfen!");

  await hc1.StopAsync();
  await hc2.StopAsync();
  Assert.True(hc1.State == HubConnectionState.Disconnected);
  Assert.True(hc2.State == HubConnectionState.Disconnected);
 }
}
