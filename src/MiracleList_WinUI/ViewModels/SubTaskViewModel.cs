using BO;
using MvvmGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiracleList_WinUI.ViewModels
{
    [Inject(typeof(Action<SubTaskViewModel>),PropertyName ="OnDelete")]
    [ViewModel(ModelType = typeof(BO.SubTask))]
    public partial class SubTaskViewModel
    {
        private readonly Action<SubTaskViewModel> onDelete;

        public void SetModel(SubTask subTask) => Model = subTask;

        public SubTask GetModel() => Model;

        [Command]
        private void Delete()
        {
            OnDelete(this);
        }
    }
}
