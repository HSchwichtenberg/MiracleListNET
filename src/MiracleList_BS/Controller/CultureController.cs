using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{

 /// <summary>
 /// MVC Controller zum Setzen des Culture-Cookie, vgl.
 /// https://docs.microsoft.com/de-de/aspnet/core/blazor/globalization-localization
 /// </summary>
 [Route("[controller]/[action]")]
 public class CultureController : Controller
 {
  public IActionResult SetCulture(string culture, string redirectUri)
  {
   if (culture != null)
   {
    HttpContext.Response.Cookies.Append(
        CookieRequestCultureProvider.DefaultCookieName,
        CookieRequestCultureProvider.MakeCookieValue(
            new RequestCulture(culture)));
   }

   return LocalRedirect(redirectUri);
  }
 }
}