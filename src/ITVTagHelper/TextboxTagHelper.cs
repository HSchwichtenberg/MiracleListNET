using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace ITVisions
{

 /// <summary>
 /// Umranden des Inhalts. Grenze da, wo Inhalt mehrfach verwendet werden muss. 
 /// </summary>
 public class Textbox1TagHelper : TagHelper
 {
  [HtmlAttributeName("size")]
  public string Size { get; set; }
  [HtmlAttributeName("prompt")]
  public string prompt { get; set; }

  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "";
   output.PreContent.AppendHtml("<div><label>");
   output.PostContent.AppendHtml("</label>&nbsp;<input type='textbox' size='" + Size + "' placeholder='" + prompt + "'></div>");
  }
 }

 /// <summary>
 /// Textbox2 liest daher den Inhalt explizit aus mit GetChildContentAsync
 /// </summary>
 public class Textbox2TagHelper : TagHelper
 {
  [HtmlAttributeName("size")]
  public string Size { get; set; }


  [HtmlAttributeName("prompt")]
  public string prompt { get; set; }

  public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
  {
   TagHelperContent inhalt = await output.GetChildContentAsync();
   output.TagName = "";
   output.Content.AppendHtml("<div><label>" + inhalt.GetContent() + "</label>&nbsp;" +
   "<input type='textbox' size='" + Size + "' placeholder='" + inhalt.GetContent().Replace(":","") + "'></div>");
  }
 }
}