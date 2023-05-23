using MiracleList;
using MiracleList_WinUI.Dialogs;
using MiracleList_WinUI.Events;
using MvvmGen;
using MvvmGen.Events;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace MiracleList_WinUI.ViewModels
{
    [Inject(typeof(IEventAggregator))]
    [Inject(typeof(IAppState))]
    [Inject(typeof(IDialogService))]
    [Inject(typeof(IMiracleListProxy))]
    [ViewModelGenerateFactory]
    [ViewModel(ModelType = typeof(BO.Task))]
    public partial class TaskItemViewModel
    {
        public void SetModel(BO.Task task) => Model = task;

        public string DueText => $"Fällig {(IsOverdue ? "seit" : "am")} {Due?.Date:d}";

        public bool IsOverdue => Due?.Date >= DateTime.Now.Date;

        [Command]
        private async Task DeleteTask()
        {
            var task = this.Model;
            var message = $"Löschen der Aufgabe #{task.TaskID}: {task.Title}?";
            if (await DialogService.ShowYesNoDialogAsync("Aufgabe löschen", message))
            {
                await MiracleListProxy.DeleteTaskAsync(task.TaskID, AppState.Token);
                EventAggregator.Publish(new TaskDeletedEvent());
            }
        }

        [Command]
        private async Task DoneChanged()
        {
            var done = this.Done;
            await MiracleListProxy.ChangeTaskDoneAsync(Model.TaskID, done, AppState.Token);
        }
    }
}
