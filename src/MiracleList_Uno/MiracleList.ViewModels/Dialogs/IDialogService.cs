namespace MiracleList_WinUI.Dialogs
{
    public interface IDialogService
    {
        Task<bool> ShowYesNoDialogAsync(string content, string title);
    }
}