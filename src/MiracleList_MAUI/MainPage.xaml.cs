using Microsoft.Extensions.DependencyInjection;
using MiracleList;
using System.Diagnostics;

namespace MiracleList_MAUI
{
    public partial class MainPage : ContentPage
    {
        private readonly IAppState appState;
        private readonly IMiracleListProxy proxy;

        public MainPage(IAppState appState, IMiracleListProxy proxy )
        {
            InitializeComponent();
            this.appState = appState;
            this.proxy = proxy;
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            var debugUser = "a.kraemer-demo-ml@it-visions.de";
            var debugPassword = "Sehr+Geheim"; // :-)

            var output = "";

            var loginInfo = new LoginInfo() { ClientID = appState.ClientID, Username = debugUser, Password = debugPassword };

            var loginResult = await proxy.LoginAsync(loginInfo);

            if (String.IsNullOrEmpty(loginResult.Message)) // OK
            {

                // Das merken wir uns im AppState
                appState.Token = loginResult.Token;
                appState.Username = loginResult.Username;

                output += $"User: {appState.Username}\n";
                output += $"Token: {appState.Token}\n";

                var categorySet = await proxy.CategorySetAsync(appState.Token);

                foreach (var item in categorySet)
                {
                    output = $"{output}{item.Name}: {item.TaskSet.Count} Tasks\n";
                }
            }
            else
            {
                output = "Login Error: " + loginResult.Message;
            }

            this.OutputLabel.Text = output;
        }
    }
}