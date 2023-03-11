using Microsoft.AspNetCore.Http;

namespace BlazorTests.Mocks
{
 public class MockHttpContextAccessor : IHttpContextAccessor
 {
  HttpContext context = new DefaultHttpContext();
  public HttpContext HttpContext { get => context; set { } }
 }
}