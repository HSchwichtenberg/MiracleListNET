using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiracleList;
using MiracleList_MAUI.Services;
using System.Collections.ObjectModel;


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

        public CategoriesPageViewModel(IAppState appState, IMiracleListProxy proxy, IDialogService dialogService)
        {
            this.appState = appState;
            this.proxy = proxy;
            this.dialogService = dialogService;
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


        private bool CanCreateNewCategory()
        {
            return !string.IsNullOrEmpty(NewCategoryName);
        }

    }
}
