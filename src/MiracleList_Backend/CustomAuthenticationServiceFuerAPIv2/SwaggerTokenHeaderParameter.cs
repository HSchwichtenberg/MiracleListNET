
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace MiracleList.CustomAuthenticationService
{

 /// <summary>
 /// Diese Klasse sorgt dafür, dass Swagger weiß, dass ML-AuthToken im Header erwartet wird
 /// </summary>
 public class SwaggerTokenHeaderParameter : IOperationFilter
 {
  public void Apply(OpenApiOperation operation, OperationFilterContext context)
  {
   bool needToken = false;
   var controllerAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true);
   var actionAttributes = context.MethodInfo.GetCustomAttributes(true).ToList();

   // prüfen, ob der Controller (==Klasse) ein [Autorize] besitzt
   if (controllerAttributes.Any(x=>x.GetType() == typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute))) {
    needToken = true;
   }

   // prüfen, ob die Operation (Action/Method) ein [Autorize] besitzt
   if (actionAttributes.Any(x => x.GetType() == typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute)))
   {
    needToken = true;
   }

   if (!needToken) return; // nichts tun, wenn kein [Autorize]

   if (operation.Parameters == null)
    operation.Parameters = new List<OpenApiParameter>();

   // Hinweis an Swagger, dass diese Methode einen Parameter via HTTP-Header erwartet
   operation.Parameters.Add(new OpenApiParameter()
   {   
    Name = MLTokenAuthenticationHandler.MLTOKENNAME, // "ML-AuthToken"
    In = ParameterLocation.Header,  
    Schema = new OpenApiSchema() {  Type = "string"},
    Required = true,
    Description = "Access Token",
   });

  }

 }

}
