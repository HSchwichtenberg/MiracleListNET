

namespace MiracleList_MAUI.Services
{
 public class DialogService : IDialogService
 {
  public Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
  {
   return Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
  }

  public Task DisplayAlert(string title, string message, string cancel)
  {
   return Application.Current.MainPage.DisplayAlert(title, message, cancel);
  }
 }
}
