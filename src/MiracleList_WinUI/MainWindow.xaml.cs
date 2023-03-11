// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.Net.Http;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using MiracleList;

namespace MiracleList_WinUI;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window {

 private ServiceCollection services { get; set; }
 private ServiceProvider ServiceProvider { get; set; }
 private IConfiguration Configuration { get; set; }
 private IAppState appState { get; set; }
 private IMiracleListProxy proxy { get; set; }

 const string DebugUser = "T.Huber@IT-Visions.de";
 const string DebugPassword = "Sehr+Geheim"; // :-)

 public MainWindow() {

  this.InitializeComponent();

  #region DI für ML
  services = new ServiceCollection();
  var builder = new ConfigurationBuilder()
     .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
     .AddUserSecrets(Assembly.GetExecutingAssembly(), true);
  Configuration = builder.Build();
  services.AddSingleton<IConfiguration>(Configuration);
  services.AddSingleton<IAppState, AppState>();
  services.AddSingleton(new HttpClient());
  services.AddScoped<MiracleList.IMiracleListProxy, MiracleList.MiracleListProxy>();
  ServiceProvider = services.BuildServiceProvider();
  #endregion

 }

 private async void LoginButton_Click(object sender, RoutedEventArgs e) {

  var output = "";

  appState = ServiceProvider.GetService<IAppState>();
  proxy = ServiceProvider.GetService<IMiracleListProxy>();

  var loginInfo = new LoginInfo() { ClientID = appState.ClientID, Username = DebugUser, Password = DebugPassword };

  var loginResult = await proxy.LoginAsync(loginInfo);

  if (String.IsNullOrEmpty(loginResult.Message)) // OK
   {

   // Das merken wir uns im AppState
   appState.Token = loginResult.Token;
   appState.Username = loginResult.Username;

   output += $"User: {appState.Username}\n";
   output += $"Token: {appState.Token}\n";

   var categorySet = await proxy.CategorySetAsync(appState.Token);

   foreach (var item in categorySet) {
    output = $"{output}{item.Name}: {item.TaskSet.Count} Tasks\n";
   }
  }
  else {
   output = "Login Error: " + loginResult.Message;
  }

  this.Output.Text = output;
 }
}
