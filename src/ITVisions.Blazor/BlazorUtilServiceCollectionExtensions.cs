using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ITVisions.Blazor;

/// <summary>
/// Erweiterungen für DI
/// </summary>
public static class BlazorUtilServiceCollectionExtensions
{
 public static IServiceCollection AddBlazorUtilForBlazorServer(this IServiceCollection services)
 {
  services.AddScoped<BlazorUtil>();
  return services;
 }

 public static IServiceCollection AddBlazorUtil(this IServiceCollection services)
 {
  services.AddScoped<IHttpContextAccessor, HttpContextAccessorDummy>();
  services.AddBlazorUtilForBlazorServer();
  return services;
 }
}