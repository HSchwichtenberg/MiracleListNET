using System.Net;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Services;
using MiracleList;
using Web.Components;

namespace Web;

public class Program
{
 public static void Main(string[] args)
 {
  var builder = WebApplication.CreateBuilder(args);

  // Add services to the container.
  builder.Services
      .AddRazorComponents()
      .AddInteractiveServerComponents()
      .AddInteractiveWebAssemblyComponents();

  // f�r Lazy Loading
  builder.Services.AddScoped<LazyAssemblyLoader>();

  #region Demo-Ausgabe
  //System.Diagnostics.Debug.WriteLine("Liste der vorregistrierten Dienste in Blazor United:");
  //foreach (var s in builder.Services)
  //{
  // System.Diagnostics.Debug.WriteLine(s.ServiceType.FullName + ": " + s.Lifetime);
  //}
  #endregion

  #region DI: Spezielle Dienste nur f�r Server
  builder.Services.AddSingleton<IAppState, AppState>();
  builder.Services.AddHttpContextAccessor();
  builder.Services.AddAuthentication().AddScheme<MLAuthSchemeOptions, MLAuthSchemeHandler>("ML", opts => { }); // notwendig, damit bei Static SSR Pre-Rendering Zugriffe auf Unterseiten zum Fehler 401 f�hren, der dann auf /login umgeleitet wird

  #endregion

  #region DI: Dienste, die Server und Client gemeinsam nutzen
  Web.Client.DI.AddServices(builder.Services);
  #endregion

  var app = builder.Build();

  //---------------------------------------------------------------------
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

  #region Bei Static SSR: Umleiten von 401-Fehler auf die /Login-Seite
  app.UseStatusCodePages(async context =>
  {
   var request = context.HttpContext.Request;
   var response = context.HttpContext.Response;
   if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
   {
    response.Redirect("/Login");  //redirect to the login page.
   }
  });
  #endregion

  #region Authentifizierung und Autorisierung nutzen
  app.UseAuthentication();
  app.UseAuthorization();
  #endregion

  app.UseHttpsRedirection();
  app.UseAntiforgery();
  app.UseStaticFiles();

  app.MapRazorComponents<App>()
     .AddInteractiveServerRenderMode()
     .AddInteractiveWebAssemblyRenderMode()
     .AddAdditionalAssemblies( // Suche nach Routen in diesen Assemblies:
      typeof(Web.Client.Components.Routes).Assembly,
      typeof(MLBlazorRCL.Login.Login).Assembly,
      typeof(Samples.SamplesList).Assembly
      );

  app.Run();
 }
}