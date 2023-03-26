using System;
using System.Linq;
using FluentValidation;
using ITVisions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace MLBlazorRCL.MainView
{

 public partial class TaskEdit
 {
  [Parameter]
  [EditorRequired] // Pflichtparameter (nur VS!)
  public BO.Task Task { get; set; } // zu bearbeitende Aufgabe

  [Parameter] // Ereignis, wenn Aufgabe sich geändert hat
  public EventCallback<bool> TaskHasChanged { get; set; }

  private EditContext editContext { get; set; } = null;

  /// <summary>
  /// Liefert die Information, ob ein Property im EditContext gültige Inhalte hat
  /// </summary>
  /// <param name="fieldname">Name des Properties</param>
  public bool IsValid(string fieldname) {
   return editContext.GetValidationMessages(this.editContext.Field(fieldname)).Any();
  }

  protected override async System.Threading.Tasks.Task OnInitializedAsync()
  {
   Util.Log(nameof(TaskEdit) + "." + nameof(OnInitialized) + ": " + Task.TaskID);
  }

  // wenn Parameter gesetzt wird
  protected async override void OnParametersSet()
  {
   Util.Log((nameof(OnParametersSet) + ": " + Task.TaskID));
   editContext = new(Task);
  }

  protected async void Save()
  {
   if (this.editContext != null)
   {
    var isValid = editContext.Validate();
    if (!isValid) { return; }
   }

   Util.Log(nameof(Save) + ": " + Task.TaskID);
   Util.Log("Task: " + this.Task);
   await TaskHasChanged.InvokeAsync(true);
  }

  protected async void Cancel()
  {
   Util.Log(nameof(Cancel) + ": " + Task.TaskID);
   await TaskHasChanged.InvokeAsync(false);
  }

 } // end class TaskEdit
}