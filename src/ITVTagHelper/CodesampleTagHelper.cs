using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITVisions
{

 /// <summary>
 /// Nutzung von Content
 /// </summary>
 public class CodesampleTagHelper : TagHelper
 {

  public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "";
   var inhalt = await output.GetChildContentAsync();
   output.Content.AppendHtml("<p style='Font-family:courier'>" + inhalt.GetContent().Replace(System.Environment.NewLine, "<br>" + System.Environment.NewLine) + "<p>");
  }
 }
}
