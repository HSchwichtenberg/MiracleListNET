using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ITVisions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace ITVisions.Blazor.Controls;

public partial class Upload
{
 /// <summary>
 /// Standard 100 MB
 /// </summary>
 [Parameter]
 public int MaxFileSize { get; set; } = 100 * 1024 * 1024; // 100 MB
 /// <summary>
 /// Standard 200 Zeichen
 /// </summary>
 [Parameter]
 public int MaxCommentLength { get; set; } = 200; // 200 Zeichen
 [Parameter]
 public string Path { get; set; } = "";
 [Parameter]
 public string Username { get; set; } = "";
 [Parameter]
 public int CleanupDays { get; set; } = 180;
 [Parameter]
 public string CleanupPath { get; set; } = "";
 [Parameter]
 public bool UseComment { get; set; } = false;
 [Parameter]
 public bool UseSilent { get; set; } = false;
 [Parameter]
 public string SilentMessage { get; set; } = "";
 [Parameter]
 public string AcceptableFileTypes { get; set; } = "";
 [Parameter]
 public string Headline { get; set; } = "Upload";
 [Parameter]
 public string Label { get; set; } = "Bitte Datei(en) per Drag&Drop hier fallen lassen oder mit Dialog wählen";
 [Parameter]
 public Dictionary<string, string> Tags { get; set; } = null;

 public List<string> AcceptableFileTypesList
 {
  get
  {
   if (String.IsNullOrEmpty(AcceptableFileTypes)) return new List<string>();
   return AcceptableFileTypes.Split(',').ToList().Select(x => x.ToLower().Trim()).ToList();
  }
 }

 [Parameter]
 public EventCallback<NewFilesArrivedArgs> NewFilesArrived { get; set; }
 [Parameter]
 public EventCallback<string> FilesCleaned { get; set; }

 #region wird nicht verwendet, ist aber notwendig für EditForm
 public class PageModel { }
 public PageModel model { get; set; } = new PageModel();
 #endregion

 List<IBrowserFile> filesToUpload = new List<IBrowserFile>();

 string Info { get; set; } = "";
 public string comment { get; set; }
 public bool silent { get; set; }

 bool displayProgress;
 int progressPercent;
 int notAllowedFiles = 0;

 CancellationTokenSource cancelation = new CancellationTokenSource();

 /// <summary>
 /// ------------------------------------------------------ Dateiauswahl ------------------------------------------------------
 /// </summary>
 async Task OnInputFileChange(InputFileChangeEventArgs e)
 {
  filesToUpload = new List<IBrowserFile>();

  var selectedFiles = e.GetMultipleFiles();
  if (String.IsNullOrEmpty(this.AcceptableFileTypes))
  {
   filesToUpload = selectedFiles.ToList();
  }
  else
  { // Aussortieren der Dateien nach Typ
   notAllowedFiles = 0;
   foreach (var f in selectedFiles)
   {
    if (this.AcceptableFileTypesList.Contains(System.IO.Path.GetExtension(f.Name).ToLower()) && !filesToUpload.Contains(f))
    {
     filesToUpload.Add(f);
    }
    else
    {
     notAllowedFiles++;
    }
   }
  }

  // Wenn es nur eine Datei ist, gibt es die schon? Dann verwende den Kommentar!
  if (filesToUpload.Count == 1)
  {
   string commentPath = System.IO.Path.Combine(Path, filesToUpload[0].Name + ".comment");
   if (File.Exists(commentPath)) { 
    this.comment = File.ReadAllText(commentPath);
    // Wenn this.comment "Hochgeladen von " enthält, entferne das und alles, was danach kommt

   
   }
  }

  //fileToUpload = e.File;
  Info = "";
 }

 /// <summary>
 /// ------------------------------------------------------ Dateiupload ------------------------------------------------------
 /// </summary>
 async Task OnSubmit()
 {
  if (filesToUpload == null) return;

  #region Cleanup
  if (!String.IsNullOrEmpty(CleanupPath))
  {
   try
   {
    var d = new System.IO.DirectoryInfo(Path);

    List<System.IO.FileInfo> files1 = d.GetOldFiles(CleanupDays);
    List<System.IO.FileInfo> files2 = d.GetOldFiles(CleanupDays + 30);

    var count = d.DeleteOldFiles(CleanupDays);
    if (count > 0)
    {
     var body = "Files to be deleted now:<br><ol><li>" + String.Join("<li>", files1.Select(x => x.FullName + " (" + x.LastWriteTime + ")")) + "</ol><hr>";
     body += "Files within 30 days:<br><ol><li>" + String.Join("<li>", files2.Select(x => x.FullName + " (" + x.LastWriteTime + ")")) + "</ol><hr>";
     await FilesCleaned.InvokeAsync(body);
     body = $"File Cleanup in {Path}: Removed old files >{CleanupDays} days removed: {count}<hr>{body}";
     await FilesCleaned.InvokeAsync(body);
    }

    d.DeleteEmptyDirectories(tolerant: true);
   }
   catch (Exception ex)
   {
    await FilesCleaned.InvokeAsync($"File Cleanup in {Path} ERROR: {ex.ToString()}");
   }
  }
  #endregion

  // TODO: https://github.com/pranavkm/FileUpload/blob/master/Pages/Index.razor
  var commentComplete = comment;

  commentComplete = commentComplete.NotNullOrEmpty("");

  if (commentComplete.StartsWith("Kundenteam: "))
  {
   commentComplete = commentComplete.Replace("Kundenteam: ", "");
   commentComplete = commentComplete.AddIfNotContains("\n", "Hochgeladen von Kundenteam");
  }
  else
  {
   if (!String.IsNullOrEmpty(Username)) commentComplete = commentComplete.AddIfNotContains("\n", "Hochgeladen von " + Username, "Hochgeladen von");
  }

  var args = new NewFilesArrivedArgs() { Silent = silent };

  foreach (IBrowserFile f in filesToUpload)
  {
   bool Updated = false;
   Info = "Hochladen der Datei <b>" + f.Name + "</b>...";
   string newName = f.Name.Replace(" ", "_");
   string newFilePath = System.IO.Path.Combine(Path, newName);
   FileUtil.GetOrCreateDir(new System.IO.FileInfo(newFilePath));
   if (File.Exists(newFilePath)) { File.Delete(newFilePath); Updated = true; }

   #region Upload mit Fortschrittsanzeige
   await UploadFile(f, newFilePath);
   #endregion

   #region Kommentar speichern
   if (UseComment && !string.IsNullOrEmpty(commentComplete))
   {
    string commentPath = newFilePath + ".comment";
    File.WriteAllText(commentPath, commentComplete);
   }
   #endregion

   #region Benachrichtigungen über Upload
   Util.Log("Upload OK: " + f.Name);
   #endregion

   args.UploadedFiles.Add(new FileWithComment() { Name = newName, Comment = commentComplete, Size = f.Size, Updated = Updated });

   if (args.UploadedFiles.Count == 1)
    Info = "Datei <b>" + f.Name + "</b> hochgeladen!";
   else
    Info = "<b>" + args.UploadedFiles.Count + " Dateien</b> hochgeladen: " + string.Join(',', args.UploadedFiles.Select(x => x.Name).ToList());
  }

  await NewFilesArrived.InvokeAsync(args);

  comment = "";
  filesToUpload = null;
  progressPercent = 0;
  displayProgress = false;
  await InvokeAsync(StateHasChanged);
 }

 async Task UploadFile(IBrowserFile f, string newFilePath)
 {

  try
  {
   using var newFile = File.OpenWrite(newFilePath);
   using var newFileStream = f.OpenReadStream(MaxFileSize);
   var buffer = new byte[10 * 1096];
   int bytesRead;
   double totalRead = 0;
   progressPercent = 0;
   displayProgress = true;

   while ((bytesRead = await newFileStream.ReadAsync(buffer)) != 0)
   {
    totalRead += bytesRead;
    await newFile.WriteAsync(buffer, 0, bytesRead);

    progressPercent = (int)((totalRead / f.Size) * 100);
    Info = "Hochladen der Datei <b>" + f.Name + "</b> (" + FileUtil.GetMB(f.Size) + "): " + progressPercent.ToString() + "%";
    //Util.Log(progressPercent);
    await InvokeAsync(StateHasChanged);
   }
   newFileStream.Close();
   newFile.Close();
  }
  catch (Exception ex)
  {
   await Util.Alert("Fehler beim Upload:\n" + ex.Message);
  }

 }

 void InsertTag(string tagText)
 {
  comment = comment.Add(" ", tagText);
 }
}