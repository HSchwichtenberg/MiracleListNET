using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MiracleList;
using MiracleList_MAUI.Messages;
using MiracleList_MAUI.Services;
using System.Collections.ObjectModel;

namespace MiracleList_MAUI.ViewModels
{
    [INotifyPropertyChanged]
    public partial class LoginPageViewModel
    {
        private readonly IAppState appState;
        private readonly IMiracleListProxy proxy;
        private readonly IDialogService dialogService;
        private readonly IMessenger messenger;
        private readonly ISecureStorage secureStorage;
        private readonly IPreferences preferences;
        private readonly INavigationService navigationService;

        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        [ObservableProperty]
        private string username;
        
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string password;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string backendUrl;

        [ObservableProperty]
        private string currentState;

        [ObservableProperty]
        private string hinweisText;

        public ObservableCollection<string> Servers { get; } = new ObservableCollection<string>();

        public LoginPageViewModel(IAppState appState, IMiracleListProxy proxy,
            IDialogService dialogService, IMessenger messenger,
            ISecureStorage secureStorage, IPreferences preferences)
        {
            this.appState = appState;
            this.proxy = proxy;
            this.dialogService = dialogService;
            this.messenger = messenger;
            this.secureStorage = secureStorage;
            this.preferences = preferences;
            foreach (var url in appState.GetBackendSet())
            {
                Servers.Add(url.Value);
            }
            BackendUrl = Servers.FirstOrDefault();

            HinweisText = $"MiracleList ist eine praxisnahe Beispielanwendung für eine App zur " +
                $"Aufgabenverwaltung. Diese Variante des Frontends läuft mit .NET MAUI " +
                $"auf {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}." +
                $"{Environment.NewLine}{Environment.NewLine}" +
                $"Zur Benutzeranmeldung geben Sie die Kombination aus Ihrer E-Mail-Adresse und " +
                $"einem beliebigen Kennwort ein. Wenn es für diese E-Mail-Adresse noch kein " +
                $"Benutzerkonto gibt, wird automatisch ein neues Benutzerkonto mit einigen " +
                $"Beispielaufgaben angelegt. Merken Sie sich das Kennwort für zukünftige Anmeldungen. " +
                $"Das Kennwort kann nicht geändert werden. Das Benutzeranmeldeverfahren ist bewusst " +
                $"vereinfacht, um die Hürde zum Einstieg in die Anwendung gering zu halten.";
        }

        public async Task InitializeAsync()
        {
            // Achtung! Nur zu Demozwecken! Hier wäre es besser das Token zu speichern!
            Username = await secureStorage.GetAsync(nameof(Username));
            Password = await secureStorage.GetAsync(nameof(Password));
            BackendUrl = preferences.Get(nameof(BackendUrl), Servers.FirstOrDefault());
        }

        [RelayCommand(CanExecute = nameof(CanLogin))]
        private async void Login()
        {
            CurrentState = "Logging in ...";
            var loginInfo = new LoginInfo() 
            { 
                ClientID = appState.ClientID, 
                Username = Username, 
                Password = Password,
            };

            proxy.BaseUrl = BackendUrl;
            var loginResult = await proxy.LoginAsync(loginInfo);


            if (string.IsNullOrEmpty(loginResult.Message)) // OK
            {
                // Das merken wir uns im AppState
                appState.Token = loginResult.Token;
                appState.Username = loginResult.Username;
                appState.BackendURL = proxy.BaseUrl;

                // Und im SecureStorage (Nur zu Demozwecken!)
                await secureStorage.SetAsync(nameof(Username), Username);
                await secureStorage.SetAsync(nameof(Password), Password);
                preferences.Set(nameof(BackendUrl), BackendUrl);

                messenger.Send(new UserLoggedInMessage(Username));
            }
            else
            {
                CurrentState = loginResult.Message;
            }
        }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Username) 
                && !string.IsNullOrWhiteSpace(Password)
                && !string.IsNullOrWhiteSpace(BackendUrl);
        }
    }
}
