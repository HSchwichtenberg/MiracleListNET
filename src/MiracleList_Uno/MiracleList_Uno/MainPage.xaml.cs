using MiracleList;
using MiracleList_WinUI.Controls;
using MiracleList_WinUI.Events;
using MiracleList_WinUI.Views;
using MvvmGen.Events;

namespace MiracleList_Uno
{
    public sealed partial class MainPage : Page, IEventSubscriber<UserLoggedInEvent,
       UserLoggedOutEvent,
       ShowViewEvent>
    {
        public MainPage()
        {
            this.InitializeComponent();
            var eventAggregator = ((App)App.Current).ServiceProvider.GetService<IEventAggregator>();
            var appState = ((App)App.Current).ServiceProvider.GetService<IAppState>();

            ShowView(typeof(LoginView));
            eventAggregator.RegisterSubscriber(this);
            this.appState = appState;
        }

        private readonly IAppState appState;
        private Dictionary<Type, object> _viewCache = new();


        public void OnEvent(UserLoggedInEvent eventData)
        {
            footerControl.UpdateStatusText();
            ShowView(typeof(TaskManagementView));
        }

        public void OnEvent(UserLoggedOutEvent eventData)
        {
            appState.Username = "";
            _viewCache.Clear();
            footerControl.UpdateStatusText();
            ShowView(typeof(LoginView));
        }

        public void OnEvent(ShowViewEvent eventData)
        {
            ShowView(eventData.ViewType);
        }

        private void ShowView(Type viewType)
        {
            if (!_viewCache.ContainsKey(viewType))
            {
                var view = ((App)App.Current).ServiceProvider.GetService(viewType)
                    ?? throw new Exception($"View not registered {viewType}");
                _viewCache.Add(viewType, view);
            }

            contentArea.Content = _viewCache[viewType];
        }
    }
}