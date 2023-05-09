using MiracleList_MAUI.ViewModels;
using MiracleList_MAUI.Views;

namespace MiracleList_MAUI
{
    public partial class AppShell : Shell
    {
        private AppShellViewModel viewModel;

        public AppShell(AppShellViewModel viewModel)
        {
            InitializeComponent();
            this.BindingContext = this.viewModel = viewModel;
            Routing.RegisterRoute(nameof(TasksPage), typeof(TasksPage));
            Routing.RegisterRoute(nameof(TaskDetailsPage), typeof(TaskDetailsPage));
        }
    }
}