using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using System.Runtime.InteropServices;

namespace MiracleList_WinUI.Views
{
    public sealed partial class AboutView : UserControl
    {
        public AboutView()
        {
            this.InitializeComponent();
            dotnetVersion.Text = RuntimeInformation.FrameworkDescription;
        }
    }
}
