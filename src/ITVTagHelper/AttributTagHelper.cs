using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ITVisions
{
 [HtmlTargetElement(Attributes = "fett")]
 public class FettTagHelper : Microsoft.AspNetCore.Razor.TagHelpers.TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.Attributes.RemoveAll("fett");
   output.Attributes.Add("style","font-weight:bold");
   //oder: 
   //output.PreContent.SetHtmlContent("<strong>");
   //output.PostContent.SetHtmlContent("</strong>");

  }
 }
 
 [HtmlTargetElement(Attributes = "kursiv")]
 public class Italic : Microsoft.AspNetCore.Razor.TagHelpers.TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.Attributes.RemoveAll("kursiv");
   output.Attributes.Add("style", "font-style:italic");
   //oder: 
   //output.PreContent.SetHtmlContent("<i>");
   //output.PostContent.SetHtmlContent("</i>");
  }
 }
}
