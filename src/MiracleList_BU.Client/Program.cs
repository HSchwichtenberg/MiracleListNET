using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MiracleList;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

#region DI
// Dienste, die Server und Client gemeinsam nutzen
Web.Client.DI.AddServices(builder.Services);

// Spezielle Dienste nur f�r Server
builder.Services.AddSingleton<IAppState, Web.Client.AppState>();
#endregion

await builder.Build().RunAsync();
