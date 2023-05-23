using Microsoft.UI.Xaml.Controls;
using MiracleList_WinUI.ViewModels;
using System.Drawing;

namespace MiracleList_WinUI.Views
{
    public sealed partial class TaskManagementView : UserControl
    {
        public TaskManagementView(TaskManagementViewModel viewModel)
        {
            this.InitializeComponent();
            ViewModel = viewModel;
            DataContext = this;
            this.Loaded += TaskManagementView_Loaded;
        }

        private async void TaskManagementView_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            await ViewModel.LoadAsync(false);
        }

        public TaskManagementViewModel ViewModel { get; }
    }
}
