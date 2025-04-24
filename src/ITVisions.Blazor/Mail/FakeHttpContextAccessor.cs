using System;
using Microsoft.AspNetCore.Http;

namespace ITVisions.Blazor.Mail;

public class FakeHttpContextAccessor : IHttpContextAccessor
{
 Microsoft.AspNetCore.Http.HttpContext IHttpContextAccessor.HttpContext { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
