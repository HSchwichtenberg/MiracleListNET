using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiracleList;

namespace Web.Authentication;

public class MLAuthSchemeHandler : AuthenticationHandler<MLAuthSchemeOptions>
{
 public MLAuthSchemeHandler(
     IOptionsMonitor<MLAuthSchemeOptions> options,
     ILoggerFactory logger,
     UrlEncoder encoder, IMLAuthenticationStateProvider ml) : base(options, logger, encoder)
 {
  AuthenticationStateProvider = ml;
 }

 public IMLAuthenticationStateProvider AuthenticationStateProvider { get; }

 protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
 {
  // Read the token from request headers/cookies
  // Check that it's a valid session, depending on your implementation

  AuthenticationState state = await AuthenticationStateProvider.GetAuthenticationStateAsync();
  if (state.User.Identity.IsAuthenticated) return AuthenticateResult.Fail("Authentication failed");
  // If the session is valid, return success:
  var claims = new[] { new Claim(ClaimTypes.Name, "Test") };
  var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Tokens"));
  var ticket = new AuthenticationTicket(principal, Scheme.Name);
  return AuthenticateResult.Success(ticket);

  // If the token is missing or the session is invalid, return failure:
  // return AuthenticateResult.Fail("Authentication failed");
 }
}


public class MLAuthSchemeOptions : AuthenticationSchemeOptions
{
}
