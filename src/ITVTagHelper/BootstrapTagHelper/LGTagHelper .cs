using System;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ITVisions
{

 public class LGTagHelper : TagHelper
 {
  public LGTagHelper(int size)
  {
   this.Size = size;
  }
  public int Size { get; set; }
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-LG-" + Size);
  }
 }


 // ReSharper disable once InconsistentNaming
 public class LG1TagHelper : TagHelper // : LGTagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-LG-1");
  }
 }

 public class LG2TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-LG-2");
  }
 }

 public class LG3TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-LG-3");
  }
 }

 public class LG4TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-LG-4");
  }
 }
 public class LG5TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-LG-5");
  }
 }
 public class LG6TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-LG-6");
  }
 }
 public class LG7TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-LG-7");
  }
 }
 public class LG8TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-LG-8");
  }
 }
 public class LG9TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-LG-9");
  }
 }
 public class LG10TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-LG-10");
  }
 }
 public class LG11TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-LG-11");
  }
 }
 public class LG12TagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "col-LG-12");
  }
 }

}