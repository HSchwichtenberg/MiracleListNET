using System.Collections.Generic;

namespace ITVisions.Blazor.Controls;

public class NewFilesArrivedArgs
{
 public List<FileWithComment> UploadedFiles { get; set; } = new();
 public bool Silent { get; set; }
}
