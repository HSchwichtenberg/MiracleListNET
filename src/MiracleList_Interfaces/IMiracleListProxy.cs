﻿namespace MiracleList;

/// <summary>
/// Diese ist eine aus dem generierten MiracleListProxy heraus erstellte Schnittstelle
/// zur Abstraktion zw MiracleListProxy (3-Tier) und MiracleListNoProxy (2-Tier).
/// </summary>
public interface IMiracleListProxy
{
 string BaseUrl { get; set; }
 bool ReadResponseAsString { get; set; }

 Task<List<string>> AboutAsync();
 Task<List<BO.Category>> CategorySetAsync(string mL_AuthToken);
 Task<BO.SubTask> ChangeSubTaskAsync(BO.SubTask st, string mL_AuthToken);
 Task<BO.Task> ChangeTaskAsync(BO.Task t, string mL_AuthToken);
 Task<BO.Task> ChangeTaskDoneAsync(int? id, bool? done, string mL_AuthToken);
 Task<BO.Category> CreateCategoryAsync(string name, string mL_AuthToken);
 Task<BO.Task> CreateTaskAsync(BO.Task t, string mL_AuthToken);
 System.Threading.Tasks.Task DeleteCategoryAsync(int id, string mL_AuthToken);
 System.Threading.Tasks.Task DeleteTaskAsync(int id, string mL_AuthToken);
 Task<List<BO.Category>> DueTaskSetAsync(string mL_AuthToken);
 Task<LoginInfo> LoginAsync(LoginInfo loginInfo);
 Task<bool> LogoffAsync(string token);
 Task<List<BO.Category>> SearchAsync(string text, string mL_AuthToken);
 Task<BO.Task> TaskAsync(int id, string mL_AuthToken);
 Task<List<BO.Task>> TaskSetAsync(int id, string mL_AuthToken);
 Task<string> VersionAsync();
 Task<bool> RemoveFileAsync(int id, string name, string mL_AuthToken);
 Task<IDictionary<string, FileInfoDTO>> FilelistAsync(int id, string mL_AuthToken);
 Task UploadAsync(int id, string mL_AuthToken, FileParameter file);
}