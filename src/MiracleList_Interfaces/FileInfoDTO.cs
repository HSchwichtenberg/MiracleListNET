#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. 
namespace MiracleList;

public partial class FileInfoDTO
{
 [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
 public string Name { get; set; }

 [Newtonsoft.Json.JsonProperty("relPath", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
 public string RelPath { get; set; }

 [Newtonsoft.Json.JsonProperty("length", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
 public long Length { get; set; }

 [Newtonsoft.Json.JsonProperty("lastWriteTime", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
 public System.DateTimeOffset LastWriteTime { get; set; }

}