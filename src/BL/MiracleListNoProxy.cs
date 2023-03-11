using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BO;
using MiracleList;

namespace BL;

public class MiracleListNoProxy : MiracleList.IMiracleListProxy
{
 public string BaseUrl { get => "http://localhost"; set => throw new NotImplementedException(); }
 public bool ReadResponseAsString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

 public Task<List<string>> AboutAsync()
 {
  throw new NotImplementedException();
 }

 public Task<List<Category>> CategorySetAsync(string mL_AuthToken)
 {
  var bl = new CategoryManager(Int32.Parse(mL_AuthToken));
  var r = bl.GetCategorySet();
  return System.Threading.Tasks.Task.FromResult(r);
 }

 public Task<SubTask> ChangeSubTaskAsync(SubTask st, string mL_AuthToken)
 {
  throw new NotImplementedException();
 }

 public Task<BO.Task> ChangeTaskAsync(BO.Task t, string mL_AuthToken)
 {
  var bl = new TaskManager(Int32.Parse(mL_AuthToken));
  var r = bl.ChangeTask(t);
  return System.Threading.Tasks.Task.FromResult(r);
 }

 public Task<BO.Task> ChangeTaskDoneAsync(int? id, bool? done, string mL_AuthToken)
 {
  var bl = new TaskManager(Int32.Parse(mL_AuthToken));
  var r = bl.ChangeTaskDone(id.GetValueOrDefault(), done.GetValueOrDefault());
  return System.Threading.Tasks.Task.FromResult(r);
 }

 public Task<Category> CreateCategoryAsync(string name, string mL_AuthToken)
 {
  var bl = new CategoryManager(Int32.Parse(mL_AuthToken));
  var r = bl.CreateCategory(name);
  return System.Threading.Tasks.Task.FromResult(r);
 }

 public Task<BO.Task> CreateTaskAsync(BO.Task t, string mL_AuthToken)
 {
  var bl = new TaskManager(Int32.Parse(mL_AuthToken));
  var r = bl.CreateTask(t);
  return System.Threading.Tasks.Task.FromResult(r);
 }

 public System.Threading.Tasks.Task DeleteCategoryAsync(int id, string mL_AuthToken)
 {
  var bl = new CategoryManager(Int32.Parse(mL_AuthToken));
  bl.RemoveCategory(id);
  return System.Threading.Tasks.Task.FromResult(0);
 }

 public System.Threading.Tasks.Task DeleteTaskAsync(int id, string mL_AuthToken)
 {
  var bl = new TaskManager(Int32.Parse(mL_AuthToken));
  bl.RemoveTask(id);
  return System.Threading.Tasks.Task.FromResult(0);
 }

 public Task<List<Category>> DueTaskSetAsync(string mL_AuthToken)
 {
  throw new NotImplementedException();
 }

 public Task<LoginInfo> LoginAsync(LoginInfo loginInfo)
 {
  // wird bei 2-Tier nicht benötigt, Login/Logoff erledigen die Authentication State Provider direkt mit der BL!
  throw new NotImplementedException();
 }

 public Task<bool> LogoffAsync(string token)
 {
  // wird bei 2-Tier nicht benötigt, Login/Logoff erledigen die Authentication State Provider direkt mit der BL!
  throw new NotImplementedException();
 }

 public Task<List<Category>> SearchAsync(string text, string mL_AuthToken)
 {
  var bl = new TaskManager(Int32.Parse(mL_AuthToken));
  return System.Threading.Tasks.Task.FromResult(bl.Search(text));
 }

 public Task<BO.Task> TaskAsync(int id, string mL_AuthToken)
 {
  var bl = new TaskManager(Int32.Parse(mL_AuthToken));
  var r = bl.GetTask(id);
  return System.Threading.Tasks.Task.FromResult(r);
 }

 public Task<List<BO.Task>> TaskSetAsync(int id, string mL_AuthToken)
 {
  return System.Threading.Tasks.Task.FromResult(new TaskManager(Int32.Parse(mL_AuthToken)).GetTaskSet(id));
 }

 public Task<string> VersionAsync()
 {
  throw new NotImplementedException();
 }
}