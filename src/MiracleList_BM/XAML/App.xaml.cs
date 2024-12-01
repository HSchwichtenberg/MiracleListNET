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
  return new Window(new MainPage(Appstate, HybridSharedState));
 }
}