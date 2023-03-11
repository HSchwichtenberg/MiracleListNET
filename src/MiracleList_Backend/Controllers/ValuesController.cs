using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using BO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace MiracleList.Controllers
{

 [Route("api/[controller]")]
 //[Authorize(Policy = "ML-AuthToken")]
 [ApiExplorerSettings(IgnoreApi = true)]
 public class ValuesController : Controller
 {
  private TelemetryClient telemetry;

  // GET api/values

  public ValuesController(TelemetryConfiguration tc)
  {
   this.telemetry = new TelemetryClient(tc);
  }

  [HttpGet]
  public IEnumerable<string> Get()
  {
   telemetry.TrackEvent("ValuesController:Get");


   var c = new Category();
   c.CategoryID = 1;
   c.Name = "test";
   return new string[] { "value1", "value2", c.Name };
  }


  // GET api/values/5
  [HttpGet("{id}")]
  //[Authorize(Policy = "ML-AuthToken")]
  public string Get(int id)
  {
   var p = new Dictionary<string, string>();
   p.Add("id", id.ToString());
   telemetry.TrackEvent("ValuesController:Get(ID)", p);
   return "value" + id;
  }

  // GET api/values/bsp1/5
  [HttpGet("bsp1/{id}")]
  //[Authorize(Policy = "ML-AuthToken")]
  public string GetBsp1(int id)
  {
   var p = new Dictionary<string, string>();
   p.Add("id", id.ToString());
   telemetry.TrackEvent("ValuesController:Get(ID)", p);
   return "Bsp1:" + id;
  }


  // GET api/values/bsp2/5
  [HttpGet]
  [Route("bsp2/{id}")]
  public string GetBsp2(int id)
  {
   var p = new Dictionary<string, string>();
   p.Add("id", id.ToString());
   telemetry.TrackEvent("ValuesController:Get(ID)", p);
   return "Bsp2:" + id;
  }

  // GET api/values/bsp2/5
  [HttpGet]
  [Route("bsp3/{id:int}")]
  public string GetBsp3(int id)
  {
   var p = new Dictionary<string, string>();
   p.Add("id", id.ToString());
   telemetry.TrackEvent("ValuesController:Get(ID)", p);
   return "Bsp3:" + id;
  }

  // GET api/values/bsp2/5
  [HttpGet]
  [Route("bsp4/{id}")]
  public string GetBsp4(int id)
  {
   if (id <= 0) throw new Exception("Ungueltige ID: " + id);
   var p = new Dictionary<string, string>();
   p.Add("id", id.ToString());
   telemetry.TrackEvent("ValuesController:Get(ID)", p);
   return "Bsp4:" + id;
  }


  [HttpGet]
  [Route("bsp5/{id}")]
  public JsonResult GetBsp5(int id)
  {
   var e = "Bsp5:" + id;
   var r = new JsonResult(e);
   this.Response.Headers.Add("X-Metadaten", "Input=" + id);
   r.StatusCode = 202;
   return r;
  }


  [HttpGet]
  [Route("bsp6/{id}")]
  public JsonResult GetBsp6(int id)
  {
   if (id <= 0)
   {
    var e = new { Status = "UngueltigeID", ID = id };
    var r = new JsonResult(e);
    this.Response.Headers.Add("X-Error", "UngueltigeID:" + id);
    r.StatusCode = 500;
    return r;
   }
   else
   {
    var e = "Bsp6:" + id;
    var r = new JsonResult(e);
    return r;

   }

  }


  // POST api/values
  [HttpPost]
  public void Post([FromBody]string value)
  {
  }

  // PUT api/values/5
  [HttpPut("{id}")]
  public void Put(int id, [FromBody]string value)
  {
  }

  // DELETE api/values/5
  [HttpDelete("{id}")]
 
  public void Delete(int id)
  {
  }
 }
}
