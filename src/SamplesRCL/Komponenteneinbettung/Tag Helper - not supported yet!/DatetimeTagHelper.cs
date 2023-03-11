using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Globalization;

namespace ITVisions
{
 /// <summary>
 /// Tag wird in Text verwandelt
 /// <Datetime />
 /// </summary>
 //[HtmlTargetElement("Zeit", TagStructure = TagStructure.Unspecified)]
 public class DatetimeTagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = null;
   output.Content.SetHtmlContent("<span style='color:red'>" + DateTime.Now.ToString() + "</span>");
  }
 }

 /// <summary>
 /// Anderer Name
 /// <Date-Time />
 /// </summary>
 //[TargetElement(TagStructure = TagStructure.NormalOrSelfClosing)]
 public class DateTimeTagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = null;
      output.Content.SetHtmlContent("<span style='color:blue'>" + DateTime.Now.ToString() + "</span>");
  }
 }

 [HtmlTargetElement("Zeitpunkt", TagStructure = TagStructure.WithoutEndTag)]
 public class DateTimeTagHelper2 : TagHelper
 {

  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = null;
   
   output.Content.SetHtmlContent("<span style='color:green'>" + DateTime.Now.ToString() + "</span>");

  }
 }
 
 /// <summary>
 /// Tag-Helper mit Attributen
 /// </summary>
 public class DatetimeformatTagHelper : TagHelper
 {
  public string Format { get; set; }
  public string Culture { get; set; }

  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   System.Globalization.CultureInfo ci;
   if (!String.IsNullOrEmpty(Culture))
   {
    ci = new System.Globalization.CultureInfo(Culture);
   }
   else
   {
    ci = CultureInfo.CurrentCulture;
   }
   output.TagName = null;
  
   output.Content.SetHtmlContent("<span style='color:red'>" + DateTime.Now.ToString(Format, ci) + "</span>");
  }
 }
}