using MiracleList_MAUI.ViewModels;

namespace MiracleList_MAUI.Views;

[QueryProperty(nameof(Task), nameof(Task))]
public partial class TaskDetailsPage : ContentPage
{
    private TaskDetailsPageViewModel viewModel;

    public TaskDetailsPage(TaskDetailsPageViewModel viewModel)
	{
		InitializeComponent();
		this.BindingContext = this.viewModel = viewModel;
	}

    public BO.Task Task { get; set; }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {

        viewModel.Task = Task;
        await viewModel.InitializeAsync();
        base.OnNavigatedTo(args);
    }
}