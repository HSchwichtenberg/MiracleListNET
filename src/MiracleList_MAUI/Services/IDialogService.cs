

namespace MiracleList_MAUI.Services
{
 public interface IDialogService
 {
  Task<bool> DisplayAlert(string title, string message, string accept, string cancel);

  Task DisplayAlert(string title, string message, string cancel);
 }
}
