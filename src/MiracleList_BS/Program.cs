using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Web;

/// <summary>
/// Die Program.cs ist bewusst mit klassischen OOP-Pattern aufgebaut
/// und verzichtet auf die meisten Kurzschreibweisen, die Microsoft in den Projektvorlagen zu .NET 6 eingeführt hat
/// </summary>
public class Program
{
 public static void Main(string[] args)
 {
  CreateHostBuilder(args).Build().Run();
 }

 public static IHostBuilder CreateHostBuilder(string[] args) =>
     Host.CreateDefaultBuilder(args)
         .ConfigureWebHostDefaults(webBuilder =>
         {
          webBuilder.UseStartup<Startup>(); //.UseUrls("http://localhost:6000", "https://localhost:6001");
         });
}