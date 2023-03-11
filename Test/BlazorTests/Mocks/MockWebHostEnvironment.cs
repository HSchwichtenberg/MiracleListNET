using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace BlazorTests.Mocks
{
 public class MockWebHostEnvironment : IWebHostEnvironment
 {
  public IFileProvider WebRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
  public string WebRootPath { get => "webroot"; set => throw new NotImplementedException(); }
  public string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
  public IFileProvider ContentRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
  public string ContentRootPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
  public string EnvironmentName { get => "RazorUnitTest"; set { } }
 }
}