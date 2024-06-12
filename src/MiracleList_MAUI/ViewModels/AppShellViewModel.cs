using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MiracleList;
using MiracleList_MAUI.Messages;
using System.Diagnostics;

namespace MiracleList_MAUI.ViewModels
{
 [INotifyPropertyChanged]
 public partial class AppShellViewModel
 {
  private readonly IAppState appState;
  private readonly IMessenger messenger;
  private readonly IBrowser browser;

  [ObservableProperty]
  private string username;

  [ObservableProperty]
  private string statustext;

  public AppShellViewModel(IAppState appState, IMessenger messenger, IBrowser browser)
  {
   this.appState = appState;
   this.messenger = messenger;
   this.browser = browser;
   Username = appState.Username;
   try
   {
    Statustext = $"{System.Runtime.InteropServices.RuntimeInformation.OSDescription} | {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription} | MAUI {FileVersionInfo.GetVersionInfo(typeof(Shell).Assembly.Location).FileVersion} | Process #{System.Environment.ProcessId} {System.IO.Path.GetFileName(System.Environment.ProcessPath)} | Thread #{System.Threading.Thread.CurrentThread.ManagedThreadId} | {System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024} MB | {DateTime.Now.ToLongTimeString()} | {(appState.IsLoggedIn ? appState.Username : "Kein Benutzer")}";
   }
   catch
   (PlatformNotSupportedException)
   {
    Statustext = $"{System.Runtime.InteropServices.RuntimeInformation.OSDescription} | {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription} | MAUI {FileVersionInfo.GetVersionInfo(typeof(Shell).Assembly.Location).FileVersion} | Process #{System.Environment.ProcessId} {System.IO.Path.GetFileName(System.Environment.ProcessPath)} | Thread #{System.Threading.Thread.CurrentThread.ManagedThreadId} | {DateTime.Now.ToLongTimeString()} | {(appState.IsLoggedIn ? appState.Username : "Kein Benutzer")}";
   }
  }

  [RelayCommand]
  private async Task Logout()
  {
   messenger.Send(new UserLoggedOutMessage(appState.Username));
  }

  [RelayCommand]
  private async Task NavigateToUrl(string url)
  {
   await browser.OpenAsync(url);
  }


 }
}
