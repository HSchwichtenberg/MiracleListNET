
using MiracleList_MAUI.ViewModels;

namespace MiracleList_MAUI.Views;
[QueryProperty(nameof(Category), nameof(Category))]
public partial class TasksPage : ContentPage
{
    private readonly TasksPageViewModel viewModel;

    public BO.Category Category { get; set; }

    public TasksPage(TasksPageViewModel viewModel)
	{
		InitializeComponent();
        this.BindingContext = this.viewModel = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        viewModel.Category = Category;
        await viewModel.InitializeAsync();
        base.OnNavigatedTo(args);
    }
}