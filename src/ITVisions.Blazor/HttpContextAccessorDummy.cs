using System;
using Microsoft.AspNetCore.Http;

namespace ITVisions.Blazor;

/// <summary>
///  Für BW und BD, nur BS hat einen HttpContextAccessor
/// </summary>
public class HttpContextAccessorDummy : IHttpContextAccessor
{
 public Microsoft.AspNetCore.Http.HttpContext HttpContext
 {
  get => null;
  set => throw new NotImplementedException();
 }
}