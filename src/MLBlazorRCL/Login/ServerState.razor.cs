using System.Collections.Generic;
using System.Threading.Tasks;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using MiracleList;

namespace MLBlazorRCL.Login;

public partial class ServerState {
 [Inject] public BlazorUtil Util { get; set; } = null;
 [Inject] public NavigationManager NavigationManager { get; set; } = null;
 [Inject] public MiracleList.IMiracleListProxy Proxy { get; set; } = null;
 [Inject] public MiracleList.IAppState AppSettings { get; set; } = null;
 [Inject] public MiracleList.IMLAuthenticationStateProvider MLAuthenticationStateProvider { get; set; } = null;

 public List<BackendState> BackendStatusList = new();

 protected override async Task OnInitializedAsync() {

  foreach (var item in AppSettings.GetBackendSet()) {
   var be = new BackendState() { Display = item.Key, Address = item.Value, State = BackendStateStatus.Checking };
   this.BackendStatusList.Add(be);
  }
 }

 protected override async Task OnAfterRenderAsync(bool firstRender) {
  if (firstRender) {
   foreach (var system in BackendStatusList) {
    _ = Task.Run(
     async () => {
      var result = await MLAuthenticationStateProvider.CheckBackend(system.Address);
      system.State = result.State;
      system.StateDetails = result.StateDetails;
      await InvokeAsync(StateHasChanged);
      }
     ); // end Task
   } // next foreach
  }
 }
}