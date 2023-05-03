using MiracleList;
using System.Collections.ObjectModel;

namespace MiracleList_MAUI.Views;

public partial class CategoriesPage : ContentPage
{
    private readonly IAppState appState;
    private readonly IMiracleListProxy proxy;

    public CategoriesPage(IAppState appState, IMiracleListProxy proxy)
    {
		InitializeComponent();
        this.appState = appState;
        this.proxy = proxy;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        var debugUser = "a.kraemer-demo-ml@it-visions.de";
        var debugPassword = "Sehr+Geheim"; // :-)

        var loginInfo = new LoginInfo() { ClientID = appState.ClientID, Username = debugUser, Password = debugPassword };

        var loginResult = await proxy.LoginAsync(loginInfo);

        if (string.IsNullOrEmpty(loginResult.Message)) // OK
        {

            // Das merken wir uns im AppState
            appState.Token = loginResult.Token;
            appState.Username = loginResult.Username;

            var categorySet = await proxy.CategorySetAsync(appState.Token);

            CategoriesCollectionView.ItemsSource = categorySet;
            HeaderLabel.Text = $"{categorySet.Count} Kategorien";
        }

        base.OnNavigatedTo(args);
    }
}