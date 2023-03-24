using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MiracleList;
using Radzen;

namespace Web.Components;

public record FileInfoDTO(string Name, string RelPath, long Length, DateTime LastWriteTime);

public partial class FilesFromWebservice
{

 [Parameter]
 public BO.Task Task { get; set; }


 [Inject]
 private BlazorUtil Util { get; set; } = null;
 [Inject]
 private IAppState appstate { get; set; } = null;
 [Inject]
 private HttpClient HttpClient { get; set; } = null;
 [Inject]
 private IMiracleListProxy Proxy { get; set; } = null;

 public const long MAXFILESIZE = (long)1073741824 * 4; // 4Gb 100 * 1024 * 1024; // 100 MB
 public const string FILEROOTFOLDER = "Files";

 public string Info { get; set; }
 
Dictionary<string, FileInfoDTO> files;

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
  var url = new Uri(new Uri(appstate.BackendURL), "/v2/Task/" + Task.TaskID + "/FileList");

  var request = new HttpRequestMessage(HttpMethod.Get, url);
  request.Headers.Add("ML-AuthToken", appstate.Token);
  using var httpResponse = await HttpClient.SendAsync(request);
  files = await httpResponse.Content.ReadFromJsonAsync<Dictionary<string, FileInfoDTO>>();
 }

 async Task DeleteFile(FileInfoDTO f)
 {
  if (!await Util.Confirm("Datei " + f.Name + " wirklich löschen?")) return;

  await Proxy.RemoveFileAsync(Task.TaskID, f.Name, appstate.Token);

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

   var stream = currentFile.OpenReadStream(MAXFILESIZE);

   using var content = new MultipartFormDataContent();
   content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
   content.Add(new StreamContent(stream, Convert.ToInt32(stream.Length)), "image", currentFile.Name);

   // Setzen des Content-Type-Headers auf multipart/form-data
   //long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
   //string boundary = timestamp.ToString("x");
   //content.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data; boundary=" + boundary);
   // Setzen des Token
   content.Headers.Add("ML-AuthToken", appstate.Token);

   // Senden der HTTP-Anforderung mit dem MultipartFormDataContent-Objekt als Inhalt
   var url = new Uri(new Uri(appstate.BackendURL), "/v2/task/" + Task.TaskID + "/upload");
   var response = await HttpClient.PostAsync(url, content);

   // Ausgabe der Serverantwort
   var responseString = await response.Content.ReadAsStringAsync();

   stream.Close();

   sw.Stop();
   Info = "Datei <b>" + currentFile.Name + "</b> hochgeladen in " + sw.ElapsedMilliseconds + "ms!";
   this.StateHasChanged();
  
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
  // end namespace