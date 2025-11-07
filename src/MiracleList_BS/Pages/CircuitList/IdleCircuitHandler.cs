using System;
using System.Threading.Tasks;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Web.Pages.CircuitList;

/// <summary>
/// Stellt fest, ob Benutzer seit einer bestimmten Zeit inaktiv ist und zeigt eine Warnung an
/// ab Blazor 8.0
/// </summary>
public sealed class IdleCircuitHandler : CircuitHandler, IDisposable
{
 readonly System.Timers.Timer timer;
 readonly ILogger logger;
 BlazorUtil util;

 public IdleCircuitHandler(IOptions<IdleCircuitOptions> options, ILogger<IdleCircuitHandler> logger, BlazorUtil util)
 {
  this.util = util;
  timer = new System.Timers.Timer();
  timer.Interval = options.Value.IdleTimeout.TotalMilliseconds;
  timer.AutoReset = false;
  timer.Elapsed += CircuitIdle;
  this.logger = logger;
 }

 private void CircuitIdle(object sender, System.Timers.ElapsedEventArgs e)
 {
  showNextActivity = false;
  var timer = sender as System.Timers.Timer;
  util.Log(DateTime.Now + ": Sie sind inaktiv! Tun Sie etwas, sonst wird die Sitzung beendet!");
  // Das ist aber eine leere Drohung, weil das bisher nicht möglich ist! ;-(
 }

 bool showNextActivity = true;

 public override Func<CircuitInboundActivityContext, Task> CreateInboundActivityHandler(
  Func<CircuitInboundActivityContext, Task> next)
 {
  return context =>
  {
   timer.Stop();
   timer.Start();
   if (showNextActivity) { util.Log(DateTime.Now.ToString() + ": Benutzer war aktiv :-) " + context.Circuit.Id); showNextActivity = false; }
   else showNextActivity = true;
   return next(context);
  };
 }

 public void Dispose()
 {
  timer.Dispose();
 }
}

public class IdleCircuitOptions
{
 public TimeSpan IdleTimeout { get; set; } = TimeSpan.FromSeconds(60); // nur als Beispiel
}

public static class IdleCircuitHandlerServiceCollectionExtensions
{
 public static IServiceCollection AddIdleCircuitHandler(this IServiceCollection services, Action<IdleCircuitOptions> configureOptions)
 {
  services.Configure(configureOptions);
  services.AddIdleCircuitHandler();
  return services;
 }

 public static IServiceCollection AddIdleCircuitHandler(this IServiceCollection services)
 {
  services.AddScoped<CircuitHandler, IdleCircuitHandler>();
  return services;
 }
}