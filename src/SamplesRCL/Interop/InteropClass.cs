using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Interop
{
 public class InteropClass
 {
  // Wenn das aktiv, dann Laufzeitfehler, weil doppelt!!!
  //[JSInvokable]
  //public static Task<string[]> CallbackFromJS()
  //{
  // return Task.FromResult(new string[] { "### Information from .NET:", ".NET Version: " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription, "App Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() });
  //}
 }
}
