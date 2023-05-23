using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MiracleList_WinUI.ViewModels;

namespace MiracleList_WinUI.Views
{
    public sealed partial class TaskDetailView : UserControl
    {
        public TaskDetailView()
        {
            this.InitializeComponent();
        }

        public TaskDetailViewModel ViewModel
        {
            get { return (TaskDetailViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(TaskDetailViewModel),
                typeof(TaskDetailView), new PropertyMetadata(null,OnViewModelChanged));

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TaskDetailView;

            if (control is not null)
            {
                control.comboBox.ItemsSource = control.ViewModel?.ImportanceList;
                control.comboBox.SelectedItem = control.ViewModel?.Importance;
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Workaround für einen Bug, der beim Data Binding existiert:
            // Werden ItemsSource und SelectedItem gebunden,
            // setzt das DataBinding SelectedItem beim weg navigieren auf null,
            // und bei der Auswahl des nächsten Tasks wird es nicht richtig gesetzt.
            // D.h. wenn hier SelectedItem null ist, dann wurde die ViewModel auf null gesetzt
            // und in der App irgendwo anders hin navigiert.
            if (comboBox.SelectedItem is not null)
            {
                this.ViewModel.Importance = (BO.Importance?)comboBox.SelectedItem;
            }
        }
    }
}
