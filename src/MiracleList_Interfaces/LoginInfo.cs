#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. 
using System.Diagnostics.CodeAnalysis;

namespace MiracleList;
/// <summary>DTO für Login-Operation</summary>
[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.0.21.0 (Newtonsoft.Json v11.0.0.0)")]
public partial class LoginInfo {
 [Newtonsoft.Json.JsonProperty("clientID", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
 public string ClientID { get; set; }

 [Newtonsoft.Json.JsonProperty("username", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
 public string Username { get; set; }

 [Newtonsoft.Json.JsonProperty("password", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
 public string Password { get; set; }

 [Newtonsoft.Json.JsonProperty("token", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
 [AllowNull]
 public string Token { get; set; }

 [Newtonsoft.Json.JsonProperty("message", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
 public string Message { get; set; }
}
