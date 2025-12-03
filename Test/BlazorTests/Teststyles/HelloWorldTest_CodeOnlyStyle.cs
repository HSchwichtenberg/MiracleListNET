using System;
using Xunit;
using Bunit;
using BlazorTests.Teststyles;

namespace BlazorTests.Teststyles
{
 public class HelloWorldTest_CodeOnlyStyle : BunitContext
 {
  [Fact]
  public void HelloWorldTest()
  {
   var cut = Render<HelloWorld>();
   var e = cut.Find(".content"); // . = css class
   e.InnerHtml.MarkupMatches(@"<h1>Hello world from Blazor</h1>");
  }
 }
}