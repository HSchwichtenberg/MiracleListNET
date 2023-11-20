using MiracleList_MAUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiracleList_MAUI.Platforms.Windows
{
 public class WordExportService(IDialogService dialogService) : IExportService
 {
  public async Task ExportTasks(BO.Category category, IEnumerable<BO.Task> tasks)
  {
   try
   {
    string fileName = $"MiracleList_Export_ {category.Name}_{ITVisions.DateTimeExtensions.ToDateString(DateTime.Now)}.docx";
    var exportFile = Path.Combine(FileSystem.AppDataDirectory, fileName);

    // Neues Word-Dokument   
    var word = new Microsoft.Office.Interop.Word.Application();
    word.Visible = true;
    var doc = word.Documents.Add();

    // Anzeige aktualisieren   
    await DoEvents();

    // Kopfzeile
    word.ActiveWindow.ActivePane.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageHeader;
    Microsoft.Office.Interop.Word.Find findObject = word.Selection.Find;
    findObject.ClearFormatting();
    findObject.Text = "Dokumententitel [ITV DokHead]";
    findObject.Replacement.ClearFormatting();
    findObject.Replacement.Text = "Alle Aufgaben in der Kategorie " + category.Name;
    object replaceAll = Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll;
    findObject.Execute(Replace: replaceAll);

    // Seiteninhalt
    word.ActiveWindow.ActivePane.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekMainDocument;
    word.Selection.WholeStory();
    word.Selection.Delete();

    foreach (var t in tasks)
    {
     var line = "Aufgabe #" + t.TaskID + ": " + t.Title + " Fällig: " + t.Due.GetValueOrDefault().ToShortDateString() + "\n";
     word.Selection.TypeText(line);
    }

    doc.SaveAs2(exportFile);
   }
   catch (Exception ex)
   {
    await dialogService.DisplayAlert("Word-Dokument kann nicht erzeugt werden", ex.Message, "OK");
   }
   // Dokument speichern

  }

  async Task DoEvents()
  {
   await Task.Delay(1); // notwendig in Blazor WebAssembly und Blazor Desktop
  }
 }
}

