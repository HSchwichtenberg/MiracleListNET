using MiracleList_MAUI.ViewModels;

namespace MiracleList_MAUI.Views;

public partial class CategoriesPage : ContentPage
{
    private readonly CategoriesPageViewModel viewModel;
    
    public CategoriesPage(CategoriesPageViewModel viewModel)
    {
		InitializeComponent();
        this.viewModel = viewModel;
        this.BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        await viewModel.InitializeAsync();
        base.OnNavigatedTo(args);
    }
}