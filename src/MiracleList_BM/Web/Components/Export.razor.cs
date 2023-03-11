using System.Runtime.Serialization;
using Microsoft.AspNetCore.Components;

namespace BM.Web.Components;
public partial class Export {

 [Parameter]
 public BO.Category category { get; set; }
 [Parameter]
 public List<BO.Task> taskSet { get; set; }

 public string ExportPath {
  get {
   if (this.category == null) return "";
   string fileName = "MiracleList_Export_" + category.Name + "_" + ITVisions.DateTimeExtensions.ToDateString(DateTime.Now) + ".xml";
   string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
   return path;
  }
 }

 /// <summary>
 /// Nur in BD und BM: Direkter Export ins Dateisystem
 /// </summary>
 public async Task ExportXMLFile() {
  if (this.category == null) return;

  if (System.IO.File.Exists(ExportPath)) {
   if (!await Util.Confirm($"Datei {ExportPath} existiert bereits. Überschreiben?")) return;
  }
  else {
   if (!await Util.Confirm($"Datei {ExportPath} anlegen?")) return;
  }

  try {
   DataContractSerializer xs = new(typeof(List<BO.Task>));
   System.IO.FileStream file = System.IO.File.Create(ExportPath);
   xs.WriteObject(file, this.taskSet);
   file.Close();
   await Util.Alert($"XML-Datei {ExportPath} wurde erzeugt!");
  }
  catch (Exception ex) {
   await Util.Alert($"XML-Datei {ExportPath} kann nicht erzeugt werden: {ex.Message}");
   return;
  }
 }

#if WINDOWS
 public string ExportPathWord
 {
 get
 {
  if (this.category == null) return "";
  string fileName = "MiracleList_Export_ " + category.Name + "_" + ITVisions.DateTimeExtensions.ToDateString(DateTime.Now) + ".docx";
  string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
  return path;
 }
 }

 /// <summary>
 /// Nur in BD und BM auf Windows: Word-Dokumentengenerierung
 /// </summary>
 public async Task ExportWord()
 {
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
 findObject.Replacement.Text = "All Tasks in Category " + this.category.Name;
 object replaceAll = Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll;
 findObject.Execute(Replace: replaceAll);

 // Seiteninhalt
 word.ActiveWindow.ActivePane.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekMainDocument;
 word.Selection.WholeStory();
 word.Selection.Delete();

 foreach (var t in this.taskSet)
 {
  var line = "Task #" + t.TaskID + ": " + t.Title + " Due: " + t.Due.GetValueOrDefault().ToShortDateString() + "\n";
  word.Selection.TypeText(line);
 }

 // Dokument speichern
 doc.SaveAs2(ExportPathWord);
 }

 async Task DoEvents()
 {
 this.StateHasChanged();
 await Task.Delay(1); // notwendig in Blazor WebAssembly und Blazor Desktop
 }
#else
 public string ExportPathWord => "";
 async Task ExportWord() {
  // hier muss nix stehen, wird nie aufgerufen für andere OS, aber muss existieren!
 }

#endif
}
