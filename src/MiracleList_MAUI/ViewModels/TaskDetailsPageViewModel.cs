using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Graphics.Text;
using MiracleList;
using MiracleList_MAUI.Services;
using System.Collections.ObjectModel;
using System.Web;

namespace MiracleList_MAUI.ViewModels
{
    [INotifyPropertyChanged]
    public partial class TaskDetailsPageViewModel
    {
        [ObservableProperty]
        private long maxFileSize = (long)1073741824 * 4;

        public BO.Task Task { get; set; }

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private DateTime due;

        [ObservableProperty]
        private bool done;

        [ObservableProperty]
        private decimal effort;

        [ObservableProperty]
        private string importance;

        [ObservableProperty]
        private string note;

        [ObservableProperty]
        private string newSubTaskTitle;

        [ObservableProperty]
        private FileInfoDTO fileToUpload;

        private IAppState appState;
        private IMiracleListProxy proxy;
        private IDialogService dialogService;
        private readonly INavigationService navigationService;

        public ObservableCollection<string> ImportanceList { get; } = new ObservableCollection<string>();
        public ObservableCollection<SubTaskViewModel> SubTasks { get; } = new ObservableCollection<SubTaskViewModel>();
        public ObservableCollection<FileInfoDTO> Files { get; } = new ObservableCollection<FileInfoDTO>();

        public TaskDetailsPageViewModel(IAppState appState, IMiracleListProxy proxy, IDialogService dialogService, INavigationService navigationService)
        {
            this.appState = appState;
            this.proxy = proxy;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
            PopulateImportanceList();

        }

        private void PopulateImportanceList()
        {
            ImportanceList.Clear();
            var importanceValues = Enum.GetValues(typeof(BO.Importance));
            foreach (var importance in importanceValues)
            {
                ImportanceList.Add(importance.ToString());
            }
        }

        public async Task InitializeAsync()
        {
            Title = Task.Title;
            Importance = Task.ImportanceNN.ToString();
            Effort = Task.Effort ?? 0;
            Due = Task.DueNN;
            Note = Task.Note;
            SubTasks.Clear();
            foreach (var subTask in Task.SubTaskSet)
            {
                SubTasks.Add(new SubTaskViewModel(subTask));
            }

            await GetFiles();
        }

        private async Task GetFiles()
        {
            var fileDictionary = await proxy.FilelistAsync(Task.TaskID, appState.Token);
            Files.Clear();
            foreach (var file in fileDictionary)
            {
                Files.Add(file.Value);
            }
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await navigationService.GoToAsync("..");
        }

        [RelayCommand]
        private async Task Save()
        {
            Task.Title = Title;
            Task.Importance = Enum.Parse<BO.Importance>(Importance);
            Task.Effort = Effort;
            Task.Due = Due;
            Task.Note = Note;
            Task.SubTaskSet.Clear();
            foreach (var subTask in SubTasks)
            {
                Task.SubTaskSet.Add(new BO.SubTask
                {
                    SubTaskID = subTask.SubTaskID,
                    TaskID = Task.TaskID,
                    Done = subTask.Done,
                    Title = subTask.Title,
                    Created = subTask.Created
                });
            }

            await proxy.ChangeTaskAsync(Task, appState.Token);
            await navigationService.GoToAsync("..");
        }

        [RelayCommand]
        private async Task CreateSubTask()
        {
            var newSubTask = new BO.SubTask()
            {
                SubTaskID = 0,
                Title = NewSubTaskTitle,
                Created = DateTime.Now,
                Done = false,
                TaskID = Task.TaskID
            };
            SubTasks.Add(new SubTaskViewModel(newSubTask));
            NewSubTaskTitle = "";
        }

        [RelayCommand]
        private async Task OpenFile(FileInfoDTO file) 
        {
            var uri = new Uri(new Uri(appState.BackendURL), file.RelPath);
            await Browser.OpenAsync(uri);
        }

        [RelayCommand]
        private async Task DeleteFile(FileInfoDTO file)
        {
            var message = $"Löschen der Datei {file.Name}?";
            if (await dialogService.DisplayAlert("Datei löschen", message, "Ja", "Nein"))
            {
                await proxy.RemoveFileAsync(Task.TaskID, file.Name, appState.Token);
                await GetFiles();
            }
        }

        [RelayCommand]
        private async Task PickFile()
        {
            var file = await FilePicker.PickAsync();
            if (file != null)
            {
                // Use the file 
                FileToUpload = new FileInfoDTO
                {
                    Name = file.FileName,
                    RelPath = file.FullPath,
                    Length = new FileInfo(file.FullPath).Length
                };
            }
            else
            {
                FileToUpload = null;
            }

        }

        [RelayCommand]
        private async Task UploadFile()
        {
            var stream = File.OpenRead(FileToUpload.RelPath);
            var fileParameter = new FileParameter(stream, FileToUpload.Name);
            await proxy.UploadAsync(Task.TaskID, appState.Token, fileParameter);
            FileToUpload = null;
            await GetFiles();
        }

    }
}
