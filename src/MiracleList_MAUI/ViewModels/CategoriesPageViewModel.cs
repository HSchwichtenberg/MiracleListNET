using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiracleList;
using MiracleList_MAUI.Services;
using System.Collections.ObjectModel;
using System.Drawing.Text;

namespace MiracleList_MAUI.ViewModels
{
    [INotifyPropertyChanged]
    public partial class CategoriesPageViewModel
    {
        [ObservableProperty]
        private int categoryCount;
        
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CreateNewCategoryCommand))]
        private string newCategoryName;
        
        private readonly IAppState appState;
        private readonly IMiracleListProxy proxy;
        private readonly IDialogService dialogService;
        private readonly INavigationService navigationService;

        public CategoriesPageViewModel(IAppState appState, IMiracleListProxy proxy,
            IDialogService dialogService, INavigationService navigationService)
        {
            this.appState = appState;
            this.proxy = proxy;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
        }

        public ObservableCollection<BO.Category> Categories { get; } = new ObservableCollection<BO.Category>();


        public async Task InitializeAsync()
        {
            Categories.Clear();

            var categorySet = await proxy.CategorySetAsync(appState.Token);

            foreach (var category in categorySet)
            {
                Categories.Add(category);
            }

            CategoryCount = categorySet.Count;
        }

        [RelayCommand(CanExecute = nameof(CanCreateNewCategory))]
        private async Task CreateNewCategory()
        {
            await proxy.CreateCategoryAsync(NewCategoryName, appState.Token);
            NewCategoryName = string.Empty;
            await InitializeAsync();
        }

        [RelayCommand]
        private async Task DeleteCategory(BO.Category category)
        {
            var message = $"Löschen der Kategorie #{category.CategoryID} mit {category.TaskSet.Count} Aufgaben?";
            if (await dialogService.DisplayAlert("Kategorie löschen", message, "Ja", "Nein"))
            {
                await proxy.DeleteCategoryAsync(category.CategoryID, appState.Token);
                await InitializeAsync();
            }
        }

        [RelayCommand]
        private async Task NavigateToTaskList(BO.Category category)
        {
            var navigationParameters = new Dictionary<string, object>
            {
                { "Category", category }
            };

            await navigationService.GoToAsync("TasksPage", navigationParameters);
        }


        private bool CanCreateNewCategory()
        {
            return !string.IsNullOrEmpty(NewCategoryName);
        }

    }
}
