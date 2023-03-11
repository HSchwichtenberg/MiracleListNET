using System;
using System.Collections.Generic;
using System.Linq;
using BO;
using DA;
using ITVisions;
using ITVisions.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace EFTools
{
	public class Program
	{
		private static string GetConnectionString()
		{

			// Wenn es einen Eintrag in mehr als einer Datei gibt, gewinnt der zuletzt hinzugefügte Eintrag

			var dic = new Dictionary<string, string> { { "ConnectionStrings:MiracleListDB", "Data Source=.;Initial Catalog=MiracleList_Test;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" } };

			var builder = new ConfigurationBuilder(); // NUGET: Microsoft.Extensions.Configuration

			// this check is necessary, because AddEnvironmentVariables() only loads process env variables https://github.com/aspnet/Configuration/issues/721
			var env_user = Environment.GetEnvironmentVariable("ConnectionStrings:MiracleListDB", EnvironmentVariableTarget.User);

			if (!String.IsNullOrEmpty(env_user))
			{ // user-Einstellung kann nur durch prozess-Einstellung überschrieben werden!
				dic["ConnectionStrings:MiracleListDB"] = env_user;
				builder.AddInMemoryCollection(dic)
					.AddEnvironmentVariables(); // lädt nur Prozessvariablen! // NUGET: Microsoft.Extensions.Configuration.EnvironmentVariables
			}
			else
			{
				builder
				.AddInMemoryCollection(dic)
				.AddJsonFile("appsettings.json") // NUGET: Microsoft.Extensions.Configuration.Json
				.AddEnvironmentVariables(); // lädt nur Prozessvariablen! // NUGET: Microsoft.Extensions.Configuration.EnvironmentVariables
			}

			IConfigurationRoot configuration = builder.Build();

			var cs = configuration["ConnectionStrings:MiracleListDB"];
			return cs;
		}

		public static void Main(string[] args)
		{
			PrintHeadline("MiracleList Backend EFC Tools " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

			Context.ConnectionString = GetConnectionString();
			var ctx = new Context();
			PrintInfo("Args: " + String.Join(" ", args));
			PrintInfo("Database: " + ctx.Database.ProviderName);
			PrintInfo("Connection String: " + Context.ConnectionString);

			if (String.IsNullOrEmpty(Context.ConnectionString))
			{
				PrintError("No Connection String!");
				System.Environment.Exit(1);
			}

			if (args.Count() == 0)
			{
				CUI.PrintError("Missing Parameter: migrate | createtestuser | recreate");
				System.Environment.Exit(1);
			}

			for (int i = 0; i < args.Length; i++)
			{
				args[i] = args[i].ToLower().Replace("-", "").Replace("/", "");
			}

			try
			{
				if (args.Contains("recreate")) Recreate(args.Contains("whatif"));
				if (args.Contains("migrate")) Migrate(args.Contains("whatif"));
				if (args.Contains("createtestuser")) CreateTestUser(args.Contains("whatif"));
			}
			catch (Exception ex)
			{
				CUI.PrintError(ex.ToString());
				Environment.Exit(-1);
			}

			//var ctx = new Context();
			//var sts = ctx.Set<SubTask>().Where(st=>st.Done==true).ToList();
			//Console.WriteLine(sts.Count);

			CUI.H1("End of EFCTools.exe");
			CUI.PrintSuccess("DONE!");
			System.Environment.Exit(0);
			//Console.ReadLine();
		}

		private static void Recreate(bool whatif = false)
		{
			CUI.H1("(Re-)Creating Database...");
			try
			{
				using (var ctx = new Context())
				{

					if ((ctx.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
					{
						Console.WriteLine("Deleting database...");
						if (!whatif) ctx.Database.EnsureDeleted();
						Console.WriteLine("OK!");
					}
				}

				using (var ctx = new Context())
				{
					Console.WriteLine("Creating database...");
					if (!whatif) ctx.Database.EnsureCreated();
					Console.WriteLine("OK!");
				}
			}
			catch (Exception ex)
			{
				PrintError("Recreate Error", ex);
				System.Environment.Exit(2);
			}
		}

		private static void Migrate(bool whatif = false)
		{
			CUI.H1("Migrate Database...");
			try
			{
				var ctx = new Context();
				PrintMigrationStatus(ctx);
				if (!whatif) ctx.Database.Migrate();
			}
			catch (Exception ex)
			{
				PrintError("Migration Error", ex);
				System.Environment.Exit(2);
			}
		}

		private static void PrintMigrationStatus(DbContext ctx)
		{
			CUI.H2("Getting Migration Status");
			try
			{
				Dictionary<string, string> migrationsStatus = new Dictionary<string, string>();
				var migrations = ctx.Database.GetMigrationStatus();

				foreach (var item in migrations)
				{
					if (item.Value) CUI.Print(item.Key + ":" + " Applied", ConsoleColor.Green);
					else CUI.Print(item.Key + ":" + " TODO", ConsoleColor.Red);
				}
			}
			catch (Exception)
			{
				CUI.PrintError("Database not available!");
			}

		}
		private static void CreateTestUser(bool whatif = false)
		{
			CUI.H1("Creating test users...");

			try
			{
				var zeit = DateTime.Now.ToString();
				var guid = new Guid("11111111-1111-1111-1111-111111111111");

				//var ctx = new Context();
				//ctx.Log();

				var clm = new BL.ClientManager();
				if (clm.GetByID(guid) == null)
				{
					var c = new Client();
					c.Name = "test";
					c.Company = "test";
					c.EMail = "test";
					c.Created = DateTime.Now;
					c.ClientID = guid;
					c.Type = "Test";
					if (!whatif) clm.New(c);
					CUI.PrintSuccess($"Client {guid} angelegt!");
				}
				else
				{
					CUI.PrintSuccess($"Client {guid} vorhanden!");
				}

				var um = new BL.UserManager("test", "test", "test");
				if (!whatif) um.InitDefaultTasks();

				var cm = new BL.CategoryManager(um.CurrentUser.UserID);
				var cs = cm.GetCategorySet();

				PrintInfo(cs.Count + " Tasks for User ID=" + um.CurrentUser.UserID + " (" + um.CurrentUser.UserName + ") Token=" + um.CurrentUser.Token);
				if (cs.Count != 4)
				{
					PrintError("CreateTestUser/Data Test Error: Count=" + cs.Count);
					System.Environment.Exit(3);
				}

				var um2 = new BL.UserManager("unittest", "unittest");
				if (!whatif) um2.InitDefaultTasks();
			}
			catch (Exception ex)
			{
				PrintError("CreateTestUser Error", ex);
				System.Environment.Exit(4);
				throw;
			}
		}
		public static void PrintInfo(string s)
		{
			// VSO Logging Commands https://github.com/Microsoft/vsts-tasks/blob/master/docs/authoring/commands.md
			//s = s += "##vso[task.logdetail]" + s;
			CUI.Print(s);
		}
		public static void PrintHeadline(string s)
		{
			// VSO Logging Commands https://github.com/Microsoft/vsts-tasks/blob/master/docs/authoring/commands.md
			//s = s += "##vso[task.logdetail]" + s;
			CUI.Headline(s);
		}

		public static void PrintError(string s, Exception ex = null)
		{
			// VSO Logging Commands https://github.com/Microsoft/vsts-tasks/blob/master/docs/authoring/commands.md
			s = s += "##vso[task.logissue type=error;]" + s + (ex != null ? ": " + ex.ToString() : "");
			CUI.PrintError(s);
		}

	}
}
