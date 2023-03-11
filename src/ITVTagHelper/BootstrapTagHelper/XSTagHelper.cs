using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ITVisions
{

 public class XSTagHelper : TagHelper
 {
  public XSTagHelper(int size)
  {
   this.Size = size;
  }

  public int Size { get; set; }
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-xs-" + Size);
  }
 }
 
 // ReSharper disable once InconsistentNaming
 public class XS1TagHelper : TagHelper // : XSTagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-xs-1");
  }
 }

 public class XS2TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-xs-2");
  }
 }

 public class XS3TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-xs-3");
  }
 }

 public class XS4TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-xs-4");
  }
 }
 public class XS5TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-xs-5");
  }
 }
 public class XS6TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-xs-6");
  }
 }
 public class XS7TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-xs-7");
  }
 }
 public class XS8TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-xs-8");
  }
 }
 public class XS9TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-xs-9");
  }
 }
 public class XS10TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-xs-10");
  }
 }
 public class XS11TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-xs-11");
  }
 }
 public class XS12TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-xs-12");
  }
 }

}