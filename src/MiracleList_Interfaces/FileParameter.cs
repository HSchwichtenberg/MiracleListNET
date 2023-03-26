#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. 
namespace MiracleList;

[System.CodeDom.Compiler.GeneratedCode("NSwag", "13.18.2.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v11.0.0.0))")]
public partial class FileParameter
{
 public FileParameter(System.IO.Stream data)
     : this(data, null, null)
 {
 }

 public FileParameter(System.IO.Stream data, string fileName)
     : this(data, fileName, null)
 {
 }

 public FileParameter(System.IO.Stream data, string fileName, string contentType)
 {
  Data = data;
  FileName = fileName;
  ContentType = contentType;
 }

 public System.IO.Stream Data { get; private set; }

 public string FileName { get; private set; }

 public string ContentType { get; private set; }
}