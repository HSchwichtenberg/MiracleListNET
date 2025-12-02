using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace MiracleList_Backend.SwaggerExtensions;

/// <summary>
/// Für "x-enumNames" 
/// siehe https://github.com/RicoSuter/NJsonSchema/wiki/Enums
/// und
/// https://github.com/domaindrivendev/Swashbuckle.WebApi/issues/1287
/// </summary>
public class EnumSchemaFilter : ISchemaFilter
{
 public void Apply(OpenApiSchema schema, SchemaFilterContext context)
 {
  if (context.Type.IsEnum)
  {
   var array = new OpenApiArray();
   array.AddRange(Enum.GetNames(context.Type).Select(n => new OpenApiString(n)));
   schema.Extensions.Add("x-enumNames", array);
  }
 }
}


