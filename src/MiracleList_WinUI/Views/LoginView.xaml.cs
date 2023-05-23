using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using MiracleList_WinUI.ViewModels;

namespace MiracleList_WinUI.Views
{
    public sealed partial class LoginView : UserControl
    {
        public LoginView(LoginViewModel viewModel)
        {
            this.InitializeComponent();
            ViewModel = viewModel;
            this.AddHandler(KeyDownEvent, new KeyEventHandler(LoginView_KeyDown), true);
        }

        public LoginViewModel ViewModel { get; }

        private void LoginView_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (ViewModel.LoginCommand.CanExecute(null))
                {
                    ViewModel.LoginCommand.Execute(null);
                }
            }
        }
    }
}
