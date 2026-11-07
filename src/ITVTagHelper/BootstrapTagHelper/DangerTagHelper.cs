using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ITVisions
{
 public class DangerTagHelper : TagHelper
 {
  public int? Value { get; set; } = null;

  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "p";
   output.Attributes.Add("class", "bg-danger");
  }

  public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
  {
   TagHelperContent inhalt = await output.GetChildContentAsync();
   decimal zahl;
   if (decimal.TryParse(inhalt.GetContent(), out zahl))
   {
    if (zahl >= Value)
    {
     output.TagName = "p";
     output.Attributes.Add("class", "bg-success");
     return;
    };
   }
   output.TagName = "p";
   output.Attributes.Add("class", "bg-danger");
  }
 }
}