using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITVisions
{

 /// <summary>
 /// Weiterer TagHelper für <a>
 /// </summary>
 [HtmlTargetElement("a")]
 public class Link1TagHelper : TagHelper
 {

  public string btn { get; set; }

  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   if (String.IsNullOrEmpty(btn)) return;
   TagHelperAttribute htmlclass = null;
   if (!output.Attributes.TryGetAttribute("class", out htmlclass))
   {
    output.Attributes.Add("class", "btn btn-" + btn);
   }
   else
   {
    output.Attributes.Remove(htmlclass);
    output.Attributes.Add("class", "btn btn-" + htmlclass.Value + " btn btn-" + btn);

   }
  }
 }


 [HtmlTargetElement("a")]
 public class Link2TagHelper : TagHelper
 {
  public string size { get; set; }
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   if (!string.IsNullOrEmpty(size))
   {
    TagHelperAttribute htmlclass = null;
    if (!output.Attributes.TryGetAttribute("class", out htmlclass))
    {
     output.Attributes.Add("class", "btn btn-" + size);
    }
    else
    {
     output.Attributes.Remove(htmlclass);
     output.Attributes.Add("class", htmlclass.Value + " btn btn-" + size);
    }
   }
  }
 }
}
