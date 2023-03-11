using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ITVisions
{

 public class MDTagHelper : TagHelper
 {
  public MDTagHelper(int size)
  {
   this.Size = size;
  }
  public int Size { get; set; }
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-MD-" + Size);
  }
 }

 // ReSharper disable once InconsistentNaming
 public class MD1TagHelper : TagHelper // : MDTagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-MD-1");
  }
 }

 public class MD2TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-MD-2");
  }
 }

 public class MD3TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-MD-3");
  }
 }

 public class MD4TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-MD-4");
  }
 }
 public class MD5TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-MD-5");
  }
 }
 public class MD6TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-MD-6");
  }
 }
 public class MD7TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-MD-7");
  }
 }
 public class MD8TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-MD-8");
  }
 }
 public class MD9TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-MD-9");
  }
 }
 public class MD10TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-MD-10");
  }
 }
 public class MD11TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-MD-11");
  }
 }
 public class MD12TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-MD-12");
  }
 }

}