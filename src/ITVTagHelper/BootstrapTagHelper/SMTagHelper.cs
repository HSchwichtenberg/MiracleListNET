using System;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ITVisions
{

 public class SMTagHelper : TagHelper
 {
  public SMTagHelper(int size)
  {
   this.Size = size;
  }
  public int Size { get; set; }
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-SM-" + Size);
  }
 }


 // ReSharper disable once InconsistentNaming
 public class SM1TagHelper : TagHelper // : SMTagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-SM-1");
  }
 }

 public class SM2TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-SM-2");
  }
 }

 public class SM3TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-SM-3");
  }
 }

 public class SM4TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-SM-4");
  }
 }
 public class SM5TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-SM-5");
  }
 }
 public class SM6TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-SM-6");
  }
 }
 public class SM7TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-SM-7");
  }
 }
 public class SM8TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-SM-8");
  }
 }
 public class SM9TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-SM-9");
  }
 }
 public class SM10TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-SM-10");
  }
 }
 public class SM11TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-SM-11");
  }
 }
 public class SM12TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-SM-12");
  }
 }

}