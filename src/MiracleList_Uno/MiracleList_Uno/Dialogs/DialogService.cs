using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace MiracleList_WinUI.Dialogs
{
    public class DialogService : IDialogService
    {
        public async Task<bool> ShowYesNoDialogAsync(string title, string content)
        {
            return true;
        }
    }
}
