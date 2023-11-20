using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiracleList_MAUI.Services
{
 public class CsvExportService : IExportService
 {
  public async Task ExportTasks(BO.Category category, IEnumerable<BO.Task> tasks)
  {
   string fileName = $"MiracleList_Export_ {category.Name}_{ITVisions.DateTimeExtensions.ToDateString(DateTime.Now)}.csv";
   var exportFile = Path.Combine(FileSystem.AppDataDirectory, fileName);
   using (var writer = new StreamWriter(exportFile, false, Encoding.UTF8))
   {
    await writer.WriteLineAsync("Task ID;Title;Due Date");
    foreach (var task in tasks)
    {
     await writer.WriteLineAsync($"{task.TaskID}; {task.Title}; {task.Due.GetValueOrDefault().ToShortDateString()}");
    }
   }
   await Launcher.Default.OpenAsync(new OpenFileRequest("CSV Export öffnen", new ReadOnlyFile(exportFile)));
  }
 }
}
