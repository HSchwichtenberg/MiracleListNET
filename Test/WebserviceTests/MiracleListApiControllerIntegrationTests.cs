using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using MiracleList;
using MiracleList.Util;

namespace WebserviceTests.Integration.WebAPI.Backend
{

 public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
 {
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
   builder.ConfigureServices(services =>
   {
    services.AddScoped(typeof(MiracleListEnvInfo));
   });
  }
 }

 public class MiracleListApiControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
 {
  private readonly HttpClient _client;

  public MiracleListApiControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory)
  {
   _client = factory.CreateClient();
  }

  [Theory]
  [InlineData("/")]
  [InlineData("/Index")]
  public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
  {
   // Act
   var response = await _client.GetAsync(url);

   // Assert
   response.EnsureSuccessStatusCode(); // Status Code 200-299
   Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
  }

  [Fact]
  public async Task About_ReturnsApiVersion()
  {
   // Act
   var response = await _client.GetAsync("/About");
   response.EnsureSuccessStatusCode();
   var content = await response.Content.ReadAsStringAsync();

   // Assert
   Assert.Contains("API-Version: v1", content);
  }

  [Fact]
  public async Task Version_ReturnsVersionString()
  {
   // Act
   var response = await _client.GetAsync("/Version");
   response.EnsureSuccessStatusCode();
   var content = await response.Content.ReadAsStringAsync();

   // Assert
   Assert.False(string.IsNullOrWhiteSpace(content));
  }
 }
}