using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Reflection;
using BO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;

namespace DA
{
 /// <summary>
 /// Context class for Entity Framework Core
 /// Forms the DAL that is used the BL manager classes
 /// </summary>
 public class Context : DbContext
 {
  // Register the entity classes in the context
  public DbSet<Client> ClientSet { get; set; }
  public DbSet<User> UserSet { get; set; }
  public DbSet<BO.Task> TaskSet { get; set; }
  public DbSet<Category> CategorySet { get; set; }
  public DbSet<Log> LogSet { get; set; }

  #region Pseudo-entities for grouping results
  public DbSet<UserStatistics> UserStatistics { get; set; } // for grouping result
  #endregion

  private static int instanceCount = 0;

  public Context()
  {
   instanceCount++;
  }

  public Context(DbContextOptions<Context> options) : base(options)
  {
   instanceCount++;
  }

  private static List<string> additionalColumnSet = null;
  public static List<string> AdditionalColumnSet
  {
   get { return additionalColumnSet; }
   set
   {
    if (instanceCount > 0) throw new ApplicationException("Cannot set AdditionalColumnSet as context has been used before!");
    additionalColumnSet = value;
   }
  }

  // This connection string is just for testing. It is read at runtime from configuration file.
  public static string ConnectionString { get; set; } = "Data Source=D120;Initial Catalog = MiracleList_TEST; Integrated Security = True; Connect Timeout = 2; Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;Application Name=EntityFramework";

  public static DbConnection Connection { get; set; } = null;

  protected override void OnConfiguring(DbContextOptionsBuilder builder)
  {
   if (!builder.IsConfigured)
   {
    if (Connection != null)
    {
     builder.UseSqlite(Context.Connection);
    }
    else
    {

     if (!String.IsNullOrEmpty(Context.ConnectionString) || Context.ConnectionString.Contains("InMemory"))
     {
      if (Context.ConnectionString.Contains("Ora"))
      {
       builder.UseOracle(Context.ConnectionString);
      }
      else
      {
       if (Context.ConnectionString.Contains("mysql"))
       {
        builder.UseMySql(Context.ConnectionString, ServerVersion.AutoDetect(Context.ConnectionString)); // UseMySQL for Oracle MySql Driver
       }
       else
       {
        builder.UseSqlServer(Context.ConnectionString, providerOptions => providerOptions.EnableRetryOnFailure(5));
       }
      }
     }
     else
     {
      builder.UseInMemoryDatabase("MiracleList InMemoryDB");
     }
    }
   }
  }

  public static bool IsRuntime { get; set; } = false;

  protected override void OnModelCreating(ModelBuilder builder)
  {
   //var p = System.Diagnostics.Process.GetCurrentProcess();
   //Console.WriteLine(p.ProcessName + "/" + System.Diagnostics.Debugger.IsAttached);

   // necessary for MySQL. Both the Oracle and the Pomelo Driver have a problem with Guid columns: "You have an error in your SQL syntax; check the manual that corresponds to your MySQL server version for the right syntax to use near 'uniqueidentifie"
   if (Database.IsMySql())
   {
    builder.Entity<Client>().Property<Guid>("ClientID").HasColumnType("char(36)");
   }

   #region Trick for pseudo entities for grouping and Views



   if (!IsRuntime)
   {
    builder.Ignore<UserStatistics>();
   }


   #endregion

   // In this case, EFCore can derive the database schema from the entity classes by convention and annotation.
   // The following Fluent API configurations only change the default behavior!
   #region Shadow property
   if (AdditionalColumnSet != null)
   {
    foreach (string shadowProp in AdditionalColumnSet)
    {
     var splitted = shadowProp.Split(';');
     string entityclass = splitted[0];
     string columnname = splitted[1];
     string columntype = splitted[2];

     Type columntypeObj = Type.GetType(columntype);

     builder.Entity(entityclass).Property(columntypeObj, columnname);
    }
   }
   #endregion

   #region Mass configuration via model class (--> Custom Convention)
   foreach (IMutableEntityType entity in builder.Model.GetEntityTypes())
   {
    // EFC Standard is to use the DbSet property name as table name
    // e.g. class "Task" --> table "TaskSet"
    // this code will change this convention
    // all table names = class names (as with EF 6.x), 
    // except the classes that have [Table] annotation
    var annotation = entity.ClrType.GetCustomAttribute<TableAttribute>();
    if (annotation == null)
    {
     entity.SetTableName(entity.DisplayName());
    }
   }
   #endregion

   #region Computed Column
   if (Database.IsSqlServer())
   {
    builder.Entity<BO.Task>().Property(x => x.DueInDays)
          .HasComputedColumnSql("DATEDIFF(day, GETDATE(), [Due])");
   }

   builder.Entity<BO.Task>().Property(x => x.Title).HasDefaultValue(BO.Task.DefaultTitle);
   #endregion

   #region Custom Indices
   builder.Entity<Category>().HasIndex(x => x.Name);
   builder.Entity<BO.Task>().HasIndex(x => x.Title);
   builder.Entity<BO.Task>().HasIndex(x => x.Done);
   builder.Entity<BO.Task>().HasIndex(x => x.Due);
   builder.Entity<BO.Task>().HasIndex(x => new { x.Title, x.Due });
   #endregion
  }
 }
}
