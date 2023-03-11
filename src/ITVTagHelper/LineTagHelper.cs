using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITVTagHelper
{
 /// <summary>
 /// Nur Umwandlung des Tags
 /// </summary>
 [HtmlTargetElement("line", TagStructure = TagStructure.WithoutEndTag)]
 public class LineTagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "hr";
  }
 }
}
