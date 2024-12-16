using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Samples.DI
{
 class DIbeiNormalerKlasse
 {

  // DI
  private BlazorUtil blazorUtil { get; set; }
  private Blazored.LocalStorage.ILocalStorageService localStorage { get; set; }
  private NavigationManager navigationManager { get; set; }

  // Name of local Storage Key
  const string LocalStorageKey = "MLToken";

  public DIbeiNormalerKlasse(BlazorUtil blazorUtil, Blazored.LocalStorage.ILocalStorageService localStorage, NavigationManager navigationManager)
  {
   // DI
   this.navigationManager = navigationManager;
   this.blazorUtil = blazorUtil;
   this.localStorage = localStorage;
  }
 }
}
