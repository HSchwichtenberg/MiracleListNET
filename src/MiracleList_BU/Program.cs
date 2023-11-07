using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MiracleList;
using Web.Components;

namespace Web;

public class Program
{
 public static void Main(string[] args)
 {
  var builder = WebApplication.CreateBuilder(args);

  // Add services to the container.
  builder.Services.AddRazorComponents()
      .AddInteractiveServerComponents()
      .AddInteractiveWebAssemblyComponents();

  #region DI
  // Spezielle Dienste nur für Server
  builder.Services.AddSingleton<IAppState, AppState>();
  builder.Services.AddAuthentication("ML");

  // Dienste, die Server und Client gemeinsam nutzen
  Web.Client.DI.AddServices(builder.Services);
  #endregion

  var app = builder.Build();

  // Configure the HTTP request pipeline.
  if (app.Environment.IsDevelopment())
  {
   app.UseWebAssemblyDebugging();
  }
  else
  {
   app.UseExceptionHandler("/Error");
   // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
   app.UseHsts();
  }

  app.UseHttpsRedirection();
  app.UseAntiforgery();
  app.UseStaticFiles();

  app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
     typeof(Web.Client.Components.Routes).Assembly,
     typeof(MLBlazorRCL.Login.Login).Assembly,
     typeof(Samples.SamplesList).Assembly

     );

  app.Run();
 }

}