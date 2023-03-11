using System;
using System.Collections.Generic;
using System.Linq;
using BO;
using DA;
using ITVisions.EFCore;
using Microsoft.EntityFrameworkCore;

namespace BL
{
 public class SubTaskManager : EntityManagerBase<Context, SubTask>
 {

  public SubTask CreateSubTask(int taskID, string title)
  {
   var t = new SubTask();
   t.TaskID = taskID;

   t.Title = title;

   this.New(t);
   return t;
  }


  /// <summary>
  /// Deletes all subtasks
  /// </summary>
  /// <param name="taskID"></param>
  public void DeleteSubTasks(int taskID)
  {

   // Workaround for Unit Test with InMemDB
   var task = ctx.TaskSet.Include(x => x.SubTaskSet).SingleOrDefault(x => x.TaskID == taskID);
   if (task.SubTaskSet != null)
   {
    foreach (var st in task.SubTaskSet)
    {
     ctx.Remove(st);
    }
   }
   ctx.SaveChanges();


   // This is not possible in Unit Test with InMemDB :-(
   // Message: System.InvalidOperationException : Relational-specific methods can only be used when the context is using a relational database provider.
   //var sql = "delete from Subtask where taskid = " + taskID;
   //ctx.Database.ExecuteSqlCommand(sql);
  }
 }
}
