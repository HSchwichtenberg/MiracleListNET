using MiracleList;

namespace BM;

public partial class App : Application {
 
 public App(IAppState appstate, HybridSharedState hybridSharedState) 
 {
  InitializeComponent();
  Appstate = appstate;
  HybridSharedState = hybridSharedState;
 }

 public IAppState Appstate { get; }
 public HybridSharedState HybridSharedState { get; }

 protected override Window CreateWindow(IActivationState? activationState)
 {
  var w = new Window(new MainPage(Appstate, HybridSharedState));
  // Setzt den Fenstertitel (nur auf Desktop-OS)
  w.Title = "MiracleList Blazor MAUI (BM) v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
  return w;
 }
}