using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiracleList;
using MiracleList_MAUI.Services;
using System.Collections.ObjectModel;

namespace MiracleList_MAUI.ViewModels
{
    [INotifyPropertyChanged]
    public partial class TasksPageViewModel
    {

        [ObservableProperty]
        private int taskCount;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CreateNewTaskCommand))]
        private string newTaskTitle;

        [ObservableProperty]
        private BO.Category category;
        private readonly IAppState appState;
        private readonly IMiracleListProxy proxy;
        private readonly IDialogService dialogService;

        public ObservableCollection<BO.Task> Tasks { get; } = new ObservableCollection<BO.Task>();

        public TasksPageViewModel(IAppState appState, IMiracleListProxy proxy, IDialogService dialogService)
        {
            this.appState = appState;
            this.proxy = proxy;
            this.dialogService = dialogService;
        }

        public async Task InitializeAsync()
        {
            Tasks.Clear();

            var taskSet = await proxy.TaskSetAsync(Category.CategoryID, appState.Token);

            foreach (var task in taskSet)
            {
                Tasks.Add(task);
            }

            TaskCount = taskSet.Count;
        }

        [RelayCommand(CanExecute = nameof(CanCreateNewTask))]
        private async Task CreateNewTask()
        {

            var task = new BO.Task
            {
                TaskID = 0, // notwendig für Server, da der die ID vergibt
                Title = NewTaskTitle,
                CategoryID = Category.CategoryID,
                Importance = BO.Importance.B,
                Created = DateTime.Now,
                Due = null,
                Order = 0,
                Note = "",
                Done = false
            };

            await proxy.CreateTaskAsync(task, appState.Token);
            NewTaskTitle = string.Empty;
            await InitializeAsync();
        }

        [RelayCommand]
        private async Task DeleteTask(BO.Task task)
        {

            var message = $"Löschen der Aufgabe #{task.TaskID}: {task.Title}?";
            if (await dialogService.DisplayAlert("Aufgabe löschen", message, "Ja", "Nein"))
            {
                await proxy.DeleteTaskAsync(task.TaskID, appState.Token);
                await InitializeAsync();
            }
        }


        private bool CanCreateNewTask()
        {
            return !string.IsNullOrEmpty(NewTaskTitle);
        }

    }
}
