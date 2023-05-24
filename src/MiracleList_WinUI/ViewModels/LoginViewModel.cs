using Microsoft.UI.Xaml;
using MiracleList;
using MiracleList_WinUI.Events;
using MiracleList_WinUI.Views;
using MvvmGen;
using MvvmGen.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.AppBroadcasting;

namespace MiracleList_WinUI.ViewModels
{
    [Inject(typeof(IEventAggregator))]
    [Inject(typeof(IAppState))]
    [Inject(typeof(IMiracleListProxy))]
    [ViewModel]
    public partial class LoginViewModel
    {
        [Property] private string _username;
        [Property] private string _password;
        [Property] private string _backendUrl;
        [Property] private string _currentState;

        public ObservableCollection<string> Servers { get; } = new();

        public string FrameworkDescription { get; } = RuntimeInformation.FrameworkDescription;

        partial void OnInitialize()
        {
            var servers = this.AppState.GetBackendSet();
            foreach (var server in servers)
            {
                Servers.Add(server.Value);
            }

            BackendUrl = Servers.FirstOrDefault();
        }

        [Command(CanExecuteMethod =nameof(CanLogin))]
        private async Task Login()
        {
            CurrentState = "Anmeldung läuft...";

            var loginInfo = new LoginInfo
            {
                ClientID = AppState.ClientID,
                Username = Username,
                Password = Password
            };
            
            MiracleListProxy.BaseUrl = BackendUrl;
            var loginResult = await MiracleListProxy.LoginAsync(loginInfo);

            if (string.IsNullOrEmpty(loginResult.Message)) // OK
            {
                // Das merken wir uns im AppState
                AppState.Token = loginResult.Token;
                AppState.Username = loginResult.Username;
                AppState.BackendURL = BackendUrl;

                CurrentState = string.Empty;

                EventAggregator.Publish(new UserLoggedInEvent());
            }
            else
            {
                CurrentState= "Anmeldefehler: " + loginResult.Message;
            }
        }

        [CommandInvalidate(nameof(Username),nameof(Password),nameof(BackendUrl))]
        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Username)
                && !string.IsNullOrWhiteSpace(Password)
                && !string.IsNullOrWhiteSpace(BackendUrl);
        }
    }
}
