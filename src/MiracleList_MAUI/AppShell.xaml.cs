using MiracleList_MAUI.ViewModels;
using MiracleList_MAUI.Views;
using MiracleList_MAUI.Views.GeneralSamples;

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
            Routing.RegisterRoute(nameof(PointerDemosPage), typeof(PointerDemosPage));
            Routing.RegisterRoute(nameof(DragDropDemoPage), typeof(DragDropDemoPage));
  }
    }
}