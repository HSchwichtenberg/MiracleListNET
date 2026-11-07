using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ITVisions;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MiracleList;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MLBlazorRCL.Login;

/// <summary>
/// Diese Komponente merkt sich den Zustand der Backend-System beim Prerendering und fragt beim Hauptrendern nicht erneut ab!
/// </summary>
public partial class ServerState(PersistentComponentState ComponentState)
{
 [Inject] public BlazorUtil Util { get; set; } = null;
 [Inject] public NavigationManager NavigationManager { get; set; } = null;
 [Inject] public MiracleList.IMiracleListProxy Proxy { get; set; } = null;
 [Inject] public MiracleList.IAppState AppSettings { get; set; } = null;
 [Inject] public MiracleList.IMLAuthenticationStateProvider MLAuthenticationStateProvider { get; set; } = null;

 public List<BackendState> BackendStatusList = new();

 private PersistingComponentStateSubscription persistingSubscription;

 protected override async Task OnInitializedAsync()
 {
  persistingSubscription = ComponentState.RegisterOnPersisting(PersistState);

  Print("ServerState.OnInitializedAsync.Start");

  if (!ComponentState.TryTakeFromJson<List<BackendState>>(
        nameof(BackendStatusList), out var restoredCount))
  { // Zustände liegen noch nicht vor
   foreach (var item in AppSettings.GetBackendSet())
   {
    var be = new BackendState() { Display = item.Key, Address = item.Value, State = BackendStateStatus.Checking };
    this.BackendStatusList.Add(be);
   }
   Print("OnInitialized | Initialisiert");
  }
  else
  { // Zustände liegen bereits vor
   BackendStatusList = restoredCount!;
   Print("OnInitialized | Wiederhergestellt");
  }

  Print("ServerState.OnInitializedAsync.End");
 }

 protected override async Task OnAfterRenderAsync(bool firstRender)
 {
  if (firstRender)
  {
   // Asynchrones Abfragen der Zustände
   foreach (var system in BackendStatusList)
   {
    _ = Task.Run(
     async () =>
     {
      var result = await MLAuthenticationStateProvider.CheckBackend(system.Address);
      system.State = result.State;
      system.StateDetails = result.StateDetails;
      await InvokeAsync(StateHasChanged);
      await PersistState();
     }
     ); // end Task
   } // next foreach
  }
 }

 /// <summary>
 /// Merken der Zustände beim Prerendering
 /// </summary>
 private Task PersistState()
 {
  if (this.RendererInfo.Name != "WebView")
  {
   Print("PersistState");
   ComponentState.PersistAsJson(nameof(BackendStatusList), BackendStatusList);
  }
  return Task.CompletedTask;
 }

 private void Print(string s)
 {
  var o = ($"----> {DateTime.Now} | {this.RendererInfo.Name} | {s} | BackendStatusList={BackendStatusList.AsDump(ObjektTrenner: " / ")} ");
  System.Diagnostics.Trace.WriteLine(o);
  Console.WriteLine(o);
 }
}