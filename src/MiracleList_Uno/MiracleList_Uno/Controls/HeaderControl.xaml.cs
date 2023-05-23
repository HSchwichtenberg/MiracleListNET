using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MvvmGen.Events;
using Microsoft.Extensions.DependencyInjection;
using MiracleList_WinUI.Events;
using MiracleList_WinUI.Views;
using MiracleList;
using MiracleList_Uno;

namespace MiracleList_WinUI.Controls
{
    public sealed partial class HeaderControl : UserControl, 
        IEventSubscriber<UserLoggedInEvent>
    {
        private readonly IAppState? _appState;

        public HeaderControl()
        {
            this.InitializeComponent();
            EventAggregator = ((App)App.Current)
                .ServiceProvider
                .GetService<IEventAggregator>();

            EventAggregator?.RegisterSubscriber(this);

            _appState = ((App)App.Current)
               .ServiceProvider
               .GetService<IAppState>();

            SetState(loggedIn: false);
        }

        public IEventAggregator? EventAggregator { get; }

        public void OnEvent(UserLoggedInEvent eventData)
        {
            loggedInUserTextBlock.Text = $"Logged in: {_appState?.Username}";
            SetState(loggedIn: true);
        }

        private void SetState(bool loggedIn)
        {
            Visibility boolToVisibility(bool val) => val
                ? Visibility.Visible
                : Visibility.Collapsed;

            loginItem.Visibility = boolToVisibility(!loggedIn);
            logoutItem.Visibility = boolToVisibility(loggedIn);
            taskManagementItem.Visibility = boolToVisibility(loggedIn);
        }

        private void LogoutItemClick(object sender, RoutedEventArgs e)
        {
            loggedInUserTextBlock.Text = "";
            SetState(loggedIn: false);
            EventAggregator?.Publish(new UserLoggedOutEvent());
        }

        private void LoginItemClick(object sender, RoutedEventArgs e)
        {
            EventAggregator?.Publish(new ShowViewEvent(typeof(LoginView)));
        }

        private void AboutItemClick(object sender, RoutedEventArgs e)
        {
            EventAggregator?.Publish(new ShowViewEvent(typeof(AboutView)));
        }

        private void TaskManagementItemClick(object sender, RoutedEventArgs e)
        {
            EventAggregator?.Publish(new ShowViewEvent(typeof(TaskManagementView)));
        }
    }
}
