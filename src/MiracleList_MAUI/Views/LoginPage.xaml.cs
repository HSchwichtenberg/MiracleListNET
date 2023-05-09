using MiracleList_MAUI.ViewModels;

namespace MiracleList_MAUI.Views;

public partial class LoginPage : ContentPage
{
    private readonly LoginPageViewModel viewModel;

    public LoginPage(LoginPageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = this.viewModel = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        await viewModel.InitializeAsync();
        base.OnNavigatedTo(args);
    }
}