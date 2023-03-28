using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ITVisions;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MiracleList;

namespace MLBlazorRCL.Files;

public partial class FilesFromFilesystem
{
 [Parameter]
 public BO.Task Task { get; set; }

 [Inject]
 private BlazorUtil Util { get; set; } = null;
 [Inject]
 private IAppState appstate { get; set; } = null;

 public const long MAXFILESIZE = (long)1073741824 * 4; // 4Gb 100 * 1024 * 1024; // 100 MB

 public string relPathFilesDir => Path.Combine(appstate.CurrentUserDirectoryRelativePath, Task.TaskID.ToString());
 public string absolutePathFilesDir => Path.Combine(appstate.CurrentUserDirectoryAbsolutePath, Task.TaskID.ToString());

 public string Info { get; set; }
 List<FileInfo> files;

 bool displayProgress;
 int progressPercent;

 CancellationTokenSource cancelation;
 IReadOnlyCollection<IBrowserFile> filesToUpload;
 //IBrowserFile fileToUpload; // falls man nur eine Datei pro Upload zulassen will

 #region Model wird nicht verwendet, ist aber notwendig für EditForm
 public object model { get; set; } = new Object();
 #endregion

 #region Standard-Lebenszyklus-Ereignisse
 protected override async Task OnParametersSetAsync()
 {

  if (String.IsNullOrEmpty(absolutePathFilesDir)) { throw new ApplicationException("CurrentUserDirectory nicht gefunden"); }
  GetFiles();
  cancelation = new CancellationTokenSource();
 }
 #endregion

 void GetFiles()
 {
  var di = new DirectoryInfo(absolutePathFilesDir).GetOrCreateDir();
  if (di != null) files = di.GetFiles().ToList();
 }

 async Task DeleteFile(FileInfo f)
 {
  if (!await Util.Confirm("Datei " + f.Name + " wirklich löschen?")) return;
  f.Delete();
  GetFiles();
 }

 async Task OnInputFileChange(InputFileChangeEventArgs e)
 {
  filesToUpload = e.GetMultipleFiles();
  //fileToUpload = e.File; // falls man nur eine Datei pro Upload zulassen will
 }

 async Task OnSubmit()
 {
  if (filesToUpload == null) return;

  foreach (IBrowserFile currentFile in filesToUpload)
  {
   var sw = new System.Diagnostics.Stopwatch();
   sw.Start();
   Info = "Hochladen der Datei <b>" + currentFile.Name + "</b>...";
   string newFilePath = Path.Combine(absolutePathFilesDir, currentFile.Name);

   // Sicherstellen, dass es Pfad gibt
   var d = new DirectoryInfo(absolutePathFilesDir).GetOrCreateDir();

   // Wenn es die Datei schon gibt, dann löschen!
   if (File.Exists(newFilePath)) File.Delete(newFilePath);

   #region Upload ohne Fortschrittsanzeige
   //using var stream = currentFile.OpenReadStream(MAXFILESIZE);
   //var outputStream = File.Create(newFilePath);
   ////stream.Seek(0, SeekOrigin.Begin); // Seek: Specified method is not supported. at Microsoft.AspNetCore.Components.Forms.BrowserFileStream.Seek
   //await stream.CopyToAsync(outputStream); // CopyTo(): Error: System.NotSupportedException: Synchronous reads are not supported.
   //outputStream.Close();
   #endregion

   #region Upload mit Fortschrittsanzeige
   using FileStream newFile = File.OpenWrite(newFilePath);
   using Stream uploadFileStream = currentFile.OpenReadStream(MAXFILESIZE);

   var buffer = new byte[50 * 1024]; // 50 KB
   int bytesRead;
   double totalRead = 0;
   progressPercent = 0;
   displayProgress = true;
   int count = 0;

   while ((bytesRead = await uploadFileStream.ReadAsync(buffer, cancelation.Token)) != 0)
   {
    count++;
    totalRead += bytesRead;
    await newFile.WriteAsync(buffer, 0, bytesRead, cancelation.Token);
    progressPercent = (int)((totalRead / currentFile.Size) * 100);
    Info = "Hochladen der Datei <b>" + currentFile.Name + "</b>: " + progressPercent.ToString() + "% / " + sw.ElapsedMilliseconds + "ms";
    this.StateHasChanged();
   }
   sw.Stop();
   Info = "Datei <b>" + currentFile.Name + "</b> hochgeladen in " + sw.ElapsedMilliseconds + "ms!";
   Util.Log(Info + " Anzahl der Schritte: " + count);
   #endregion
  }

  if (filesToUpload.Count > 1)
  {
   Info = filesToUpload.Count + " Dateien hochgeladen!";
  }
  else
  {
   this.filesToUpload = null;
  }
  progressPercent = 0;
  displayProgress = false;
  GetFiles();
  StateHasChanged();
 }


} // end class
  // end namespace