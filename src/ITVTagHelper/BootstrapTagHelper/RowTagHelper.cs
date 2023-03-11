using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ITVisions
{
 [RestrictChildren(
  "xs", "xs1", "xs2", "xs3", "xs4", "xs5", "xs6", "xs7","xs8", "xs9", "xs10", "xs11", "xs12",
  "sm", "sm1", "sm2", "sm3", "sm4", "sm5", "sm6", "sm7", "sm8", "sm9", "sm10", "sm11", "sm12",
  "md", "md1", "md2", "md3", "md4", "md5", "md6", "md7", "md8", "md9", "md10", "md11", "md12",
  "lg", "lg1", "lg2", "lg3", "lg4", "lg5", "lg6", "lg7", "lg8", "lg9", "lg10", "lg11", "lg12")]
 public class RowTagHelper : TagHelper
 {
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   output.TagName = "div";
   output.Attributes.Add("class", "row");
  }
 }
}