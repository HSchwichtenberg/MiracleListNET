#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

using MiracleList;

namespace BM;

public partial class App : Application {
 public App(IAppState appstate, HybridSharedState hybridSharedState) {
  InitializeComponent();

  Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) => {

   // https://stackoverflow.com/questions/72399551/maui-net-set-window-size
#if WINDOWS
   var mauiWindow = handler.VirtualView;
   var nativeWindow = handler.PlatformView;
   nativeWindow.Activate();
   IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
   WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
   AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
   appWindow.Resize(new SizeInt32(3000, 2000));
#endif
  });

  MainPage = new MainPage(appstate, hybridSharedState);
 }
}