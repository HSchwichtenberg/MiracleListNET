using MiracleList;
using MiracleList_WinUI.Dialogs;
using MiracleList_WinUI.Events;
using MvvmGen;
using MvvmGen.Events;
using System.Threading.Tasks;

namespace MiracleList_WinUI.ViewModels
{
    [Inject(typeof(IEventAggregator))]
    [Inject(typeof(IMiracleListProxy))]
    [Inject(typeof(IAppState))]
    [Inject(typeof(IDialogService))]
    [ViewModelGenerateFactory]
    [ViewModel(ModelType=typeof(BO.Category))]
    public partial class CategoryItemViewModel
    {
        public void SetModel(BO.Category task) => Model = task;

        [Command]
        private async Task DeleteCategory()
        {
            var category = this.Model;

            var message = $"Löschen der Kategorie {category.Name} mit {category.TaskSet.Count} Aufgaben?";
            if (await DialogService.ShowYesNoDialogAsync("Kategorie löschen", message))
            {
                await MiracleListProxy.DeleteCategoryAsync(category.CategoryID, AppState.Token);
                EventAggregator.Publish(new CategoryDeletedEvent());
            }
        }

    }
}
