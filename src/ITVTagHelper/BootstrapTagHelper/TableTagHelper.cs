using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ITVisions
{
 public class TableTagHelper : TagHelper
 {
  public bool Custom { get; set; }
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   if (!Custom) output.Attributes.Add("class", "table table-striped table-hover");
  }
 }
}