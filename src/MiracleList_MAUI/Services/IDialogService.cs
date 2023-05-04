

namespace MiracleList_MAUI.Services
{
    public interface IDialogService
    {
        Task<bool> DisplayAlert(string title, string message, string accept, string cancel);
    }
}
