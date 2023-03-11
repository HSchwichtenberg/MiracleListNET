namespace MiracleList.Controllers
{
 /// <summary>
 /// DTO wird verwendet als Ein- + Ausgabe für Login-Operation
 /// Achtung: Hier waren früher Fields statt Properties. Das mag aber Swagger nicht mehr :-(
 /// </summary>
 public class LoginInfo
 {
  public string ClientID { get; set; }
  public string Username { get; set; }
  public string Password { get; set; }
  public string Token { get; set; }
  public string Message { get; set; }
 }
}
