using MiracleList;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MiracleList_MAUI.ViewModels
{
    public class CategoriesPageViewModel : INotifyPropertyChanged
    {
        private int categoryCount;
        private string newCategoryName;
        private readonly IAppState appState;
        private readonly IMiracleListProxy proxy;

        public event PropertyChangedEventHandler PropertyChanged;

        

        public CategoriesPageViewModel(IAppState appState, IMiracleListProxy proxy)
        {
            this.appState = appState;
            this.proxy = proxy;
            CreateNewCategoryCommand = new Command(async () => { await CreateNewCategory(); }, CanCreateNewCategory);
        }

        public ObservableCollection<BO.Category> Categories { get; } = new ObservableCollection<BO.Category>();

        public Command CreateNewCategoryCommand { get; }

        public int CategoryCount
        {
            get { return categoryCount; }
            set
            {
                if (categoryCount != value)
                {
                    categoryCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public string NewCategoryName
        {
            get { return newCategoryName; }
            set
            {
                if (newCategoryName != value)
                {
                    newCategoryName = value;
                    OnPropertyChanged();
                    CreateNewCategoryCommand.ChangeCanExecute();
                }
            }
        }

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


        private async Task CreateNewCategory()
        {
            await proxy.CreateCategoryAsync(NewCategoryName, appState.Token);
            NewCategoryName = string.Empty;
            await InitializeAsync();
        }

        private bool CanCreateNewCategory()
        {
            return !string.IsNullOrEmpty(NewCategoryName);
        }


        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
