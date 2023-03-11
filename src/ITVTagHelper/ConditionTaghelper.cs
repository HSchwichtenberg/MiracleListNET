using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ITVisions
{
 [HtmlTargetElement(Attributes = nameof(Condition))]
 public class ConditionTagHelper : TagHelper
 {
  /// <summary>
  /// Alle TagHelper-Parameter, die nicht vom Typ string sind, werden beim Aufruf als Ausdrücke behandelt - auch ohne Razor-Syntax mit @! 
  /// </summary>
  public bool Condition { get; set; }

  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
   if (!Condition)
   {
    output.SuppressOutput();
   }

  }
 }
}
