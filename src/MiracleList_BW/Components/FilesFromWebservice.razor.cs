using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MiracleList;

namespace Web.Components;

public partial class FilesFromWebservice
{
 /// <summary>Aktuelle Aufgabe</summary>
 [Parameter]
 public BO.Task Task { get; set; }

 [Inject]
 private BlazorUtil Util { get; set; } = null;
 [Inject]
 private IAppState AppState { get; set; } = null;
 [Inject]
 private HttpClient HttpClient { get; set; } = null;
 [Inject]
 private IMiracleListProxy Proxy { get; set; } = null;

 public const long MAXFILESIZE = (long)1073741824 * 4; // 4Gb 100 * 1024 * 1024; // 100 MB
 public const string FILEROOTFOLDER = "Files";

 public string Info { get; set; }

 /// <summary>Liste der Dateien auf dem Server</summary>
 IDictionary<string, FileInfoDTO> files;

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
  await GetFiles();
  cancelation = new CancellationTokenSource();
 }
 #endregion

 async Task GetFiles()
 {
  files = await Proxy.FilelistAsync(Task.TaskID, AppState.Token);

  // oder mit HttpClient:
  //var url = new Uri(new Uri(appstate.BackendURL), "/v2/Task/" + Task.TaskID + "/FileList");
  //var request = new HttpRequestMessage(HttpMethod.Get, url);
  //request.Headers.Add("ML-AuthToken", appstate.Token);
  //using var httpResponse = await HttpClient.SendAsync(request);
  //files = await httpResponse.Content.ReadFromJsonAsync<Dictionary<string, FileInfoDTO>>();
 }

 async Task RemoveFile(FileInfoDTO f)
 {
  if (!await Util.Confirm("Datei " + f.Name + " wirklich löschen?")) return;
  await Proxy.RemoveFileAsync(Task.TaskID, f.Name, AppState.Token);

  // oder mit HttpClient:
  // var url = new Uri(new Uri(appstate.BackendURL), "/v2/Task/" + Task.TaskID + "/File/" + f.Name);
  //var request = new HttpRequestMessage(HttpMethod.Delete, url);
  //request.Headers.Add("ML-AuthToken", appstate.Token);
  //await HttpClient.SendAsync(request);

  await GetFiles();
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
   if (currentFile.Size > MAXFILESIZE) { await Util.Alert("Datei " + currentFile.Name + " ist zu groß!"); break; }
   var sw = new System.Diagnostics.Stopwatch();
   sw.Start();
   Info = "Hochladen der Datei <b>" + currentFile.Name + "</b>...";

   #region Datei senden per MiracleListProxy
   var stream = currentFile.OpenReadStream(MAXFILESIZE);
   var fileParameter = new FileParameter(stream,currentFile.Name);
   await Proxy.UploadAsync(Task.TaskID, AppState.Token, fileParameter);
   #endregion

   //#region Datei senden direkt per HTTPClient
   //// Stream der Datei holen
   //var stream = currentFile.OpenReadStream(MAXFILESIZE);
   //// Stream festlegen als Form-Data im HTTP-Request
   //using var content = new MultipartFormDataContent();
   //content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
   //content.Add(new StreamContent(stream, Convert.ToInt32(stream.Length)), "image", currentFile.Name);
   //// Setzen des Token
   //content.Headers.Add("ML-AuthToken", AppState.Token);
   //// Senden per HTTP
   //var url = new Uri(new Uri(AppState.BackendURL), "/v2/task/" + Task.TaskID + "/upload");
   //var response = await HttpClient.PostAsync(url, content);
   //// Serverantwort
   //var responseString = await response.Content.ReadAsStringAsync();
   //stream.Close();
   //Util.Log($"{currentFile.Name}: {responseString}");
   //#endregion

   sw.Stop();
   Info = $"Datei <b>{currentFile.Name}</b> hochgeladen in {sw.ElapsedMilliseconds}ms!";
   Util.Log($"File Upload {currentFile.Name}: {sw.ElapsedMilliseconds}ms!");
   this.StateHasChanged();
  }

  if (filesToUpload.Count > 1)
  {
   Info = filesToUpload.Count + " Dateien hochgeladen!";
  }

  this.filesToUpload = null;
  progressPercent = 0;
  displayProgress = false;
  await GetFiles();
  StateHasChanged();
 }

 #region Util
 public string GetMB(long Bytes)
 {
  return $"{((decimal)Bytes / 1024 / 1024):00.00} MB";
 }

 #endregion

} // end class