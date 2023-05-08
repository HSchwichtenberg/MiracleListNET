
using CommunityToolkit.Mvvm.ComponentModel;

namespace MiracleList_MAUI.ViewModels
{
    [INotifyPropertyChanged]
    public partial class SubTaskViewModel
    {
        [ObservableProperty]
        private int taskID;

        [ObservableProperty]
        private int subTaskID;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private bool done;

        [ObservableProperty]
        private DateTime created;

        public SubTaskViewModel()
        {
            
        }

        public SubTaskViewModel(BO.SubTask subTask)
        {
            TaskID = subTask.SubTaskID;
            SubTaskID = subTask.SubTaskID;
            Title = subTask.Title;
            Done = subTask.Done;
            Created = subTask.Created;
        }
    }
}
