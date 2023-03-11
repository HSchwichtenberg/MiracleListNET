using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BD.Web.Authentication
{
 public class DebuggingAuthenticationStateProvider : AuthenticationStateProvider
 {
  public override Task<AuthenticationState> GetAuthenticationStateAsync()
  {
   var identity = new ClaimsIdentity(new[]
   {
     new Claim(ClaimTypes.Name, "Max Mustermann"),
        }, "Debugging authentication type");

   var user = new ClaimsPrincipal(identity);
   return Task.FromResult(new AuthenticationState(user));
  }
 }
}
