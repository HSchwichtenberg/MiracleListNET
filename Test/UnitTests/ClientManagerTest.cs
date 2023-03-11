using BL;
using BO;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace UnitTests
{
 public class ClientManagerTest
 {



   public ClientManagerTest()
   {
    Util.Init();
   }

   [Fact]

   public void CheckClient()
   {
    var id = System.Guid.NewGuid();
    using (var cm = new ClientManager())
    {
     var c = new Client();
     c.ClientID = id;
     cm.New(c);
     var c2 = cm.CheckClient(id.ToString());
     Assert.Equal(id, c2.client.ClientID);
    }
   }
  }
 }

