using System;
using Xunit;
using Bunit;
using static Bunit.ComponentParameterFactory;
using BlazorTests.Teststyles;

namespace BlazorTests
{
 public class HelloWorldTestCSStyle : TestContext
 {
  [Fact]
  public void HelloWorldTest()
  {
   var cut = RenderComponent<HelloWorld>();
   var e = cut.Find(".content"); // . = css class
   e.InnerHtml.MarkupMatches(@"<h1>Hello world from Blazor</h1>");
  }
 }
}