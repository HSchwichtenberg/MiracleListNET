using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
//using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BD.Web.Components
{
    public partial class FilesFromFilesystem
 {
        [Parameter]
        public string FolderName { get; set; } = "Demo";

        [Inject]
        private BlazorUtil Util { get; set; } = null;
        //[Inject]
        //private IWebHostEnvironment env { get; set; } = null;

        public const long MAXFILESIZE = (long)1073741824 * 4; // 4Gb 100 * 1024 * 1024; // 100 MB
        public const string FILEROOTFOLDER = "Files";

        public string relPathFilesDir => Path.Combine(FILEROOTFOLDER, FolderName);

        public string absolutePathFilesDir => Path.Combine(Environment.CurrentDirectory ?? "", relPathFilesDir);

        public string Info { get; set; }
        List<FileInfo> files;

        bool displayProgress;
        int progressPercent;

        CancellationTokenSource cancelation;
        IReadOnlyCollection<IBrowserFile> filesToUpload;
        //IBrowserFile fileToUpload; // falls man nur eine Datei pro Upload zulassen will

        #region Model wird nicht verwendet, ist aber notwendig für EditForm
        public object model { get; set; } = new object();
        #endregion

        #region Standard-Lebenszyklus-Ereignisse
        protected async override Task OnParametersSetAsync()
        {
            //if (String.IsNullOrEmpty(env.WebRootPath)) { throw new ApplicationException("WebRootPath ist leer. Gibt es kein wwwroot-Verzeichnis?"); }
            GetFiles();
            cancelation = new CancellationTokenSource();
        }
        #endregion

        void GetFiles()
        {
            var di = GetOrCreateDir(new DirectoryInfo(absolutePathFilesDir));
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

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            foreach (IBrowserFile currentFile in filesToUpload)
            {
                Info = "Hochladen der Datei <b>" + currentFile.Name + "</b>...";
                string newFilePath = Path.Combine(absolutePathFilesDir, currentFile.Name);

                // Sicherstellen, dass es Pfad gibt
                GetOrCreateDir(new FileInfo(newFilePath));
                // Wenn es die Datei schon gibt, dann löschen!
                if (File.Exists(newFilePath)) File.Delete(newFilePath);

                #region Upload ohne Fortschrittsanzeige
                //using var stream = f.OpenReadStream(MAXFILESIZE); // 50 MB
                //var outputStream = File.Create(path);
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
                    await newFile.WriteAsync(buffer, cancelation.Token);
                    progressPercent = (int)(totalRead / currentFile.Size * 100);
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
                filesToUpload = null;
            }
            progressPercent = 0;
            displayProgress = false;
            GetFiles();
            StateHasChanged();
        }

        #region Util
        public string GetMB(long Bytes)
        {
            return $"{(decimal)Bytes / 1024 / 1024:00.00} MB";
        }

        public DirectoryInfo GetOrCreateDir(FileInfo obj, bool NoRecurse = false)
        {
            DirectoryInfo d = obj.Directory;

            if (!d.Exists && !d.Parent.Exists && !NoRecurse) GetOrCreateDir(new DirectoryInfo(d.Parent.FullName), true);
            if (!d.Exists) d.Create();
            return d;
        }

        public DirectoryInfo GetOrCreateDir(DirectoryInfo obj, bool NoRecurse = false)
        {
            DirectoryInfo d = new DirectoryInfo(obj.FullName);
            if (!d.Exists && d.Parent == null) throw new ApplicationException("Cannot Create Directory - No Parent Directory");
            if (!d.Exists && !d.Parent.Exists && !NoRecurse) GetOrCreateDir(new DirectoryInfo(d.Parent.FullName), true);
            if (!d.Exists) d.Create();
            return d;
        }
        #endregion

    } // end class
} // end namespace