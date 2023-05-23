using Microsoft.UI.Xaml.Controls;
using MiracleList;
using MiracleList_WinUI.Dialogs;
using MiracleList_WinUI.Events;
using MvvmGen;
using MvvmGen.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MiracleList_WinUI.ViewModels
{
    [Inject(typeof(IEventAggregator))]
    [Inject(typeof(IDialogService))]
    [Inject(typeof(IMiracleListProxy))]
    [Inject(typeof(IAppState))]
    [Inject(typeof(ITaskDetailViewModelFactory))]
    [Inject(typeof(ITaskItemViewModelFactory))]
    [Inject(typeof(ICategoryItemViewModelFactory))]
    [ViewModel]
    public partial class TaskManagementViewModel :
        IEventSubscriber<TaskDeletedEvent, CategoryDeletedEvent,
            TaskEditCancelEvent, TaskSavedEvent>
    {
        [Property] private string _newCategoryName;
        [Property] private string _newTaskTitle;

        [Property] private int _categoryCount;
        [Property] private int _taskCount;
        [Property] private CategoryItemViewModel? _selectedCategory;
        [Property] private TaskItemViewModel? _selectedTask;
        [Property] private TaskDetailViewModel? _taskDetailViewModel;

        [PropertyInvalidate(nameof(SelectedCategory))]
        public bool IsCategorySelected => SelectedCategory is not null;

        [PropertyInvalidate(nameof(TaskDetailViewModel))]
        public bool HasTaskDetailViewModel => TaskDetailViewModel is not null;

        public ObservableCollection<CategoryItemViewModel> Categories { get; } = new();
        public ObservableCollection<TaskItemViewModel> Tasks { get; } = new();

        internal async Task LoadAsync(bool forceReload = true)
        {
            if (Categories.Count > 0 && !forceReload)
            {
                return;
            }

            Categories.Clear();

            var categorySet = await MiracleListProxy.CategorySetAsync(AppState.Token);

            foreach (var category in categorySet)
            {
                var categoryItemVm = CategoryItemViewModelFactory.Create();
                categoryItemVm.SetModel(category);
                Categories.Add(categoryItemVm);
            }

            CategoryCount = categorySet.Count;
        }

        [Command(CanExecuteMethod = nameof(CanCreateCategory))]
        private async Task CreateCategory()
        {
            await MiracleListProxy.CreateCategoryAsync(NewCategoryName, AppState.Token);
            NewCategoryName = string.Empty;
            await LoadAsync();
        }

        [CommandInvalidate(nameof(NewCategoryName))]
        private bool CanCreateCategory() => !string.IsNullOrWhiteSpace(NewCategoryName);

        [Command(CanExecuteMethod = nameof(CanCreateTask))]
        private async Task CreateTask()
        {
            if (SelectedCategory is null)
            {
                return;
            }

            var task = new BO.Task
            {
                TaskID = 0, // notwendig für Server, da der die ID vergibt
                Title = NewTaskTitle,
                CategoryID = SelectedCategory.CategoryID,
                Importance = BO.Importance.B,
                Created = DateTime.Now,
                Due = null,
                Order = 0,
                Note = "",
                Done = false
            };

            await MiracleListProxy.CreateTaskAsync(task, AppState.Token);
            NewTaskTitle = string.Empty;
            await LoadTasksAsync();
        }

        [CommandInvalidate(nameof(NewTaskTitle))]
        private bool CanCreateTask() => !string.IsNullOrWhiteSpace(NewTaskTitle);

        public async Task LoadTasksAsync()
        {
            SelectedTask = null;
            TaskDetailViewModel = null;
            Tasks.Clear();

            if (SelectedCategory is null)
            {
                return;
            }

            var taskSet = await MiracleListProxy.TaskSetAsync(
                SelectedCategory.CategoryID, AppState.Token);

            foreach (var task in taskSet)
            {
                var taskItemVm = TaskItemViewModelFactory.Create();
                taskItemVm.SetModel(task);
                Tasks.Add(taskItemVm);
            }

            TaskCount = taskSet.Count;
        }

        public async Task ShowTaskDetailsAsync()
        {
            if (SelectedTask is null)
            {
                return;
            }
            var taskDetailViewModel = TaskDetailViewModelFactory.Create();

            var task = await MiracleListProxy.TaskAsync(SelectedTask.TaskID, AppState.Token);
            taskDetailViewModel.SetModel(task);

            TaskDetailViewModel = taskDetailViewModel;
        }

        public async void OnEvent(TaskDeletedEvent eventData)
        {
            await LoadTasksAsync();
        }

        public async void OnEvent(CategoryDeletedEvent eventData)
        {
            await LoadAsync();
        }

        public async void OnEvent(TaskSavedEvent eventData)
        {
            SelectedTask = null;
            TaskDetailViewModel = null;
            await LoadTasksAsync();
        }

        public void OnEvent(TaskEditCancelEvent eventData)
        {
            SelectedTask = null;
            TaskDetailViewModel = null;
        }
    }
}
