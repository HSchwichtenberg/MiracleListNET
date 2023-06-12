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

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            // Fensterposition setzen
            window.X = 300;
            window.Y = 100;

            // Größe des Fensters definieren
            window.Width = 400;
            window.Height = 800;
            return window;
        }

        private void ShowLogin()
        {
            MainPage = new NavigationPage(services.GetService<LoginPage>());
        }

        private void ShowAppShell()
        {
            MainPage = services.GetService<AppShell>();
        }
    }
}