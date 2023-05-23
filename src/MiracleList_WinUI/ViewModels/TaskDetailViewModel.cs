using MiracleList;
using MiracleList_WinUI.Dialogs;
using MiracleList_WinUI.Events;
using MvvmGen;
using MvvmGen.Events;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Threading.Tasks;

namespace MiracleList_WinUI.ViewModels
{
    [Inject(typeof(IEventAggregator))]
    [Inject(typeof(IAppState))]
    [Inject(typeof(IDialogService))]
    [Inject(typeof(IMiracleListProxy))]
    [ViewModelGenerateFactory]
    [ViewModel(ModelType = typeof(BO.Task))]
    public partial class TaskDetailViewModel
    {
        public void SetModel(BO.Task task)
        {
            Model = task;
            SubTasks.Clear();
            foreach (var subTask in task.SubTaskSet)
            {
                var vm = new SubTaskViewModel(OnDeleteSubTask);
                vm.SetModel(subTask);
                SubTasks.Add(vm);
            }
        }

        [Property] string _newSubTaskTitle;

        partial void OnInitialize()
        {
            ImportanceList.Clear();
            var importanceValues = Enum.GetValues(typeof(BO.Importance));
            foreach (BO.Importance importance in importanceValues)
            {
                ImportanceList.Add(importance);
            }
        }

        [Command]
        private async Task CreateSubTask()
        {
            var newSubTask = new BO.SubTask()
            {
                SubTaskID = 0,
                Title = NewSubTaskTitle,
                Created = DateTime.Now,
                Done = false,
                TaskID = Model.TaskID
            };

            Model.SubTaskSet.Add(newSubTask);

            var subTaskVm = new SubTaskViewModel(OnDeleteSubTask);
            subTaskVm.SetModel(newSubTask);
            SubTasks.Add(subTaskVm);

            NewSubTaskTitle = "";
        }

        private void OnDeleteSubTask(SubTaskViewModel vm)
        {
            Model.SubTaskSet.Remove(vm.GetModel());
            SubTasks.Remove(vm);
        }

        public async Task LoadAsync()
        {
            await GetFilesAsync();
        }

        private async Task GetFilesAsync()
        {
            Files.Clear();

            var fileDictionary = await MiracleListProxy
                .FilelistAsync(Model.TaskID, AppState.Token);

            foreach (var file in fileDictionary)
            {
                Files.Add(file.Value);
            }
        }

        public ObservableCollection<BO.Importance> ImportanceList { get; } = new ();
        public ObservableCollection<SubTaskViewModel> SubTasks { get; } = new ();
        public ObservableCollection<FileInfoDTO> Files { get; } = new();

        [Command]
        private void Cancel()
        {
            EventAggregator.Publish(new TaskEditCancelEvent());
        }

        [Command]
        private async Task Save()
        {
            await MiracleListProxy.ChangeTaskAsync(Model, AppState.Token);
            EventAggregator.Publish(new TaskSavedEvent());
        }
    }
}
