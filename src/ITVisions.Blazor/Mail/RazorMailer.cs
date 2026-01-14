using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace ITVisions.Blazor.Mail;

/// <summary>
/// 
/// </summary>
public class RazorMailer
{

 private static void PrepareRender(out IServiceProvider serviceProvider, out ILoggerFactory loggerFactory)
 {
  #region Preparation
  IServiceCollection services = new ServiceCollection();
  services.AddLogging((loggingBuilder) => loggingBuilder
        .SetMinimumLevel(LogLevel.Trace)
        //.AddConsole()
        );

  services.AddScoped<BlazorUtil>(); // nur nötig, weil einige Komponenten auch im UI verwendet werden
  services.AddScoped<IJSRuntime, FakeJSRuntime>(); // nur nötig, weil einige Komponenten auch im UI verwendet werden
  services.AddScoped<NavigationManager, FakeNavigationManager>(); // nur nötig, weil einige Komponenten auch im UI verwendet werden
  services.AddScoped<IHttpContextAccessor, FakeHttpContextAccessor>(); // nur nötig, weil einige Komponenten auch im UI verwendet werden

  serviceProvider = services.BuildServiceProvider();
  loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
  #endregion
 }

 public async Task<string> Render<T>(Dictionary<string, object> parameter)
where T : Microsoft.AspNetCore.Components.IComponent
 {
  #region Preparation
  IServiceProvider serviceProvider;
  ILoggerFactory loggerFactory;
  PrepareRender(out serviceProvider, out loggerFactory);
  #endregion

  #region HTML Rendering using a Razor Component
  // Klasse HtmlRenderer neu in .NET 8.0: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.components.web.htmlrenderer?view=aspnetcore-8.0
  await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);

  var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
  {
   //var name = "Dr. Holger Schwichtenberg";
   //CUI.Print("Parameter 'Name' = " + name);
   //Dictionary<string, object> pdic = new Dictionary<string, object>() { { "Name", name } };
   var pv = ParameterView.FromDictionary(parameter);
   //or var pv = ParameterView.Empty;

   var output = await htmlRenderer.RenderComponentAsync<T>(pv);

   return output.ToHtmlString();
  });

  return html;
  #endregion
 }

 public async Task<string> Render(Type componentType, Dictionary<string, object> parameter)

 {
  #region Preparation
  IServiceProvider serviceProvider;
  ILoggerFactory loggerFactory;
  PrepareRender(out serviceProvider, out loggerFactory);
  #endregion

  #region HTML Rendering using a Razor Component
  // Klasse HtmlRenderer neu in .NET 8.0: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.components.web.htmlrenderer?view=aspnetcore-8.0
  await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);

  var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
  {
   //var name = "Dr. Holger Schwichtenberg";
   //CUI.Print("Parameter 'Name' = " + name);
   //Dictionary<string, object> pdic = new Dictionary<string, object>() { { "Name", name } };
   var pv = ParameterView.FromDictionary(parameter);
   //or var pv = ParameterView.Empty;

   var output = await htmlRenderer.RenderComponentAsync(componentType, pv);

   return output.ToHtmlString();
  });

  return html;
  #endregion
 }

 public async Task<string> Send<T>(Dictionary<string, object> parameter, string from, string to, string cc, string subject, List<string> Attachments = null, string bcc = "")
where T : Microsoft.AspNetCore.Components.IComponent
 {
  var html = await Render<T>(parameter);
  var e = await Send(from, to, cc, subject, html, Attachments, bcc);
  return e;
 }

 public async Task<string> Send(string from, string to, string cc, string subject, string html, List<string> Attachments = null, string bcc = "")
 {

  // https://www.litmus.com/blog/a-guide-to-bulletproof-buttons-in-email-design
  var linkStyle = """
   style='font-size: 18px; font-family: Helvetica, Arial, sans-serif; color: #ffffff; font-weight: bold; text-decoration: none; border-radius: 5px; background-color: #12B4FF; border-top: 12px solid #12B4FF; border-bottom: 12px solid #12B4FF; border-right: 18px solid #12B4FF; border-left: 18px solid #12B4FF; display: inline-block;'  
   """;

  html = html.Replace("class=\"mailbutton\"", linkStyle);
  //html = html.Replace("<h3>", "<h3 color:white;font-family: Futura Md Bt, Futura Bold, Arial Black;font-weight:bold;border-top:2px solid #12B4FF;border-bottom:2px solid #12B4FF;background-color:#12B4FF;border-left: 16px solid #12B4FF;margin-top:15px;'>");
  //html = html.Replace("<hr/>", "<hr style='border: none; height: 3px; color: #12B4FF; background-color: #12B4FF;' />");

  // Outlook is only taking the first class listed in the class attribute, ignoring everything else !!!

  html = "<html><head><style>" +
   " hr { border: none; height: 3px; color: #12B4FF; background-color: #12B4FF;} " +
   " h3 { color:white;font-family: Futura Md Bt, Futura Bold, Arial Black;font-weight:bold;border-top:2px solid #12B4FF;border-bottom:2px solid #12B4FF;background-color:#12B4FF;border-left: 16px solid #12B4FF;margin-top:15px; }" +

   " .badge-success { display: inline-block; font-size: 75%; font-weight: 700; line-height: 12px;  text-align: center; white-space: nowrap; vertical-align: baseline; border-radius: 0.25rem; color: #fff; background-color: #28a745; border: 3px solid #28a745; } " +
   " .badge-info { display: inline-block; font-size: 75%; font-weight: 700; line-height: 12px; text-align: center; white-space: nowrap; vertical-align: baseline; border-radius: 0.25rem; color: #fff; background-color: #17a2b8; border: 3px solid #17a2b8} " +
   " .badge-danger { display: inline-block; font-size: 75%; font-weight: 700; line-height: 12px; text-align: center; white-space: nowrap; vertical-align: baseline; border-radius: 0.25rem; color: #fff; background-color: #dc3545; border: 3px solid #dc3545} " +
   " .badge-secondary { display: inline-block; font-size: 75%; font-weight: 700; line-height: 12px; text-align: center; white-space: nowrap; vertical-align: baseline; border-radius: 0.25rem; color: #fff; background-color: #6c757d; border: 3px solid #6c757d} " +
   "</style><body>" +
   html +
   "</body></html>";

  #region Send HTML via E-Mail
  new ITVisions.Network.MailUtil().SendMail(from, to, subject, html, HTML: true, Attachments: Attachments, CC: cc, BCC: bcc);
  #endregion
  return html;
 }

}
