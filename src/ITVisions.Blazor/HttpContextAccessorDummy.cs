using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITVisions.Blazor
{

 /// <summary>
 /// Für BW und BD, nur BS hat einen HttpContextAccessor
 /// </summary>
 public class HttpContextAccessorDummy : IHttpContextAccessor
 {
  public HttpContext HttpContext { get => null; set => throw new NotImplementedException(); }
 }
}
