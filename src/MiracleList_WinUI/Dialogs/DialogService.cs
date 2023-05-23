using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace MiracleList_WinUI.Dialogs
{
    public interface IDialogService
    {
        Task<bool> ShowYesNoDialogAsync(string content, string title);
    }

    public class DialogService : IDialogService
    {
        private MainWindow? mainWindow;

        public DialogService(MainWindow? mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public async Task<bool> ShowYesNoDialogAsync(string title, string content)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content=content,
                PrimaryButtonText = "Ja",
                SecondaryButtonText="Nein",
                XamlRoot=mainWindow?.Content.XamlRoot
                
            };
            var result = await dialog.ShowAsync();

            return result == ContentDialogResult.Primary;
        }
    }
}
