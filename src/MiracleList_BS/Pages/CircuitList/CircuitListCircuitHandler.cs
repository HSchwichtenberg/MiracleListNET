using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ITVisions.Blazor.Services
{

 /// <summary>
 /// Eigener CircuitHandler, der eine Liste aller Circuits anbietet
 /// verwendet https://github.com/ua-parser/uap-csharp
 /// ACHTUNG: Fehler hier können zu "Error: The circuit failed to initialize." führen!
 /// </summary>
 public class CircuitListCircuitHandler : CircuitHandler
 {
  private readonly ILogger<CircuitHandler> logger;
  private readonly IJSRuntime jsRuntime;
  private readonly IHttpContextAccessor ca;
  /// <summary>
  /// Globale Liste aller Circuits
  /// </summary>
  public static ConcurrentBag<CircuitInfo> CircuitSet = new ConcurrentBag<CircuitInfo>();

  public CircuitListCircuitHandler(ILogger<CircuitHandler> logger, IJSRuntime jsRuntime) // TODO: , IHttpContextAccessor ca
  {
   this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
   this.jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
   //this.ca = ca;
   this.logger.LogInformation($"{nameof(Services.CircuitListCircuitHandler)}.ctor");
  }

  public override async Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
  {
   logger.LogInformation($"{nameof(CircuitListCircuitHandler)}.{nameof(OnCircuitOpenedAsync)}: {circuit.Id}");

   var myC = new CircuitInfo(circuit);
   myC.ClientIP = "n/a";
   try
   {
    myC.ClientIP = ca.HttpContext?.Connection?.RemoteIpAddress?.ToString();
   }
   catch (Exception)   {  }

   try
   {
    var uaParser = UAParser.Parser.GetDefault();
    var ua = uaParser.Parse(ca.HttpContext.Request.Headers["User-Agent"]);
    myC.ClientBrowser = ua.UA + " @ " + ua.OS;
   }
   catch (Exception)
   {
    myC.ClientBrowser = "n/a";
   }

   CircuitSet.Add(myC);

   await base.OnCircuitOpenedAsync(circuit, cancellationToken);
  }

  public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
  {
   logger.LogInformation($"{nameof(CircuitListCircuitHandler)}.{nameof(OnConnectionDownAsync)}: {circuit.Id}");

   var myC = CircuitSet.FirstOrDefault(x => x.Circuit.Id == circuit.Id);
   myC.CircuitState = CircuitState.ConnectionDown;
   myC.LastStateChanged = DateTime.Now;

   return base.OnConnectionDownAsync(circuit, cancellationToken);
  }

  public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
  {
   logger.LogInformation($"{nameof(CircuitListCircuitHandler)}.{nameof(OnConnectionUpAsync)}: {circuit.Id}");

   var myC = CircuitSet.FirstOrDefault(x => x.Circuit.Id == circuit.Id);
   myC.CircuitState = CircuitState.ConnectionUp;
   myC.LastStateChanged = DateTime.Now;

   return base.OnConnectionUpAsync(circuit, cancellationToken);
  }

  public override async Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
  {
   logger.LogInformation($"{nameof(CircuitListCircuitHandler)}.{nameof(OnCircuitClosedAsync)}: {circuit.Id}");

   var myC = CircuitSet.FirstOrDefault(x => x.Circuit.Id == circuit.Id);
   myC.CircuitState = CircuitState.Closed;
   myC.LastStateChanged = DateTime.Now;

   await base.OnCircuitClosedAsync(circuit, cancellationToken);
  }

 }
}