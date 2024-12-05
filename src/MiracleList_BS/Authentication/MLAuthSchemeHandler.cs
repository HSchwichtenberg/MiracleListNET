using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiracleList;

namespace Web.Authentication;

public class MLAuthSchemeHandler : AuthenticationHandler<MLAuthSchemeOptions>
{
 public MLAuthSchemeHandler(
     IOptionsMonitor<MLAuthSchemeOptions> options,
     ILoggerFactory logger,
     UrlEncoder encoder, IMLAuthenticationStateProvider ml, IHttpContextAccessor httpContextAccessor) : base(options, logger, encoder)
 {
  AuthenticationStateProvider = ml;
  HttpContextAccessor = httpContextAccessor;
 }

 public IMLAuthenticationStateProvider AuthenticationStateProvider { get; }
 public IHttpContextAccessor HttpContextAccessor { get; }
 const string TokenStorageKey = "MLToken";

 protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
 {
  //AuthenticationState state = await AuthenticationStateProvider.GetAuthenticationStateAsync();
  //if (!state.User.Identity.IsAuthenticated) return AuthenticateResult.Fail("Authentication failed");

  string token = HttpContextAccessor.HttpContext.Request.Cookies[TokenStorageKey];
  if (string.IsNullOrEmpty(token)) { return AuthenticateResult.NoResult(); }
  //var claims = new[] { new Claim(ClaimTypes.Name, state.User.Identity.Name) };
  var claims = new[] { new Claim(ClaimTypes.Name, token) };
  var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "ML"));
  var ticket = new AuthenticationTicket(principal, Scheme.Name);
  return AuthenticateResult.Success(ticket);

 }
}


public class MLAuthSchemeOptions : AuthenticationSchemeOptions
{
}
