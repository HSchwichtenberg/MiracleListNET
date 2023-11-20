namespace MiracleList_MAUI.Services
{
 public interface IExportService
 {
  Task ExportTasks(BO.Category category, IEnumerable<BO.Task> tasks);
 }
}
