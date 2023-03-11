using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UnitTests
{
 public class LaunchSettingsFixture : IDisposable
 {
  public LaunchSettingsFixture()
  {
   using (var file = File.OpenText("Properties\\launchSettings.json"))
   {
    var reader = new JsonTextReader(file);
    var jObject = JObject.Load(reader);


   }
  }

  public void Dispose()
  {
   // ... clean up
  }
 }
}
