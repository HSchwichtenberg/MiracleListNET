using MiracleList_MAUI.Views;

namespace MiracleList_MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(TasksPage), typeof(TasksPage));
        }
    }
}