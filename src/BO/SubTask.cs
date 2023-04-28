using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace BO;

/// <summary>
/// Geschäftsobjektklasse für eine Unteraufgabe
/// Wird auf dem Server auf allen Schichten von EFCore bis hin zum WebAPI und auch in .NET-Clients verwendet
/// Auf dem JavaScript-basierten Clients wird eine hieraus generiert TypeScript-Proxy-Klasse  verwendet
/// </summary>
[DataContract(IsReference = true)] // nur für XML-Export in Blazor Desktop-App
[JsonObject(IsReference = false)] // das aber dann fpr JSON.NET notwendig, siehe https://stackoverflow.com/questions/19308992/how-do-i-disable-object-reference-creation-in-the-newtonsoft-json-serializer?rq=1 ("As well as using the built-in Json.NET attributes, Json.NET also looks for the SerializableAttribute (if IgnoreSerializableAttribute on DefaultContractResolver is set to false) DataContractAttribute, DataMemberAttribute and NonSerializedAttribute ... when determining how JSON is to be serialized and deserialized.")
public class SubTask
{
 public int SubTaskID { get; set; } // PK
 [MaxLength(250)]
 public string Title { get; set; }
 public bool Done { get; set; }
 public DateTime Created { get; set; } = DateTime.Now;
 // -------------- Navigation Properties
 public Task Task { get; set; }
 public int TaskID { get; set; }
}