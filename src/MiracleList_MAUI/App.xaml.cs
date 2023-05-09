using CommunityToolkit.Mvvm.Messaging;
using MiracleList_MAUI.Messages;
using MiracleList_MAUI.Views;

namespace MiracleList_MAUI
{
    public partial class App : Application
    {
        private readonly IServiceProvider services;

        public App(IServiceProvider services, IMessenger messenger)
        {
            InitializeComponent();
            this.services = services;
            messenger.Register<UserLoggedInMessage>(this, (r,m) => {
                ShowAppShell();
            });

            messenger.Register<UserLoggedOutMessage>(this, (r, m) => {
                ShowLogin();
            });

            ShowLogin();
        }

        private void ShowLogin()
        {
            MainPage = new NavigationPage(services.GetService<LoginPage>());
        }

        private void ShowAppShell()
        {
            MainPage = new AppShell();
        }
    }
}