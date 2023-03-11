using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Bunit;
using Radzen.Blazor;
using Xunit;

namespace MiracleListTests;

public class TestUtil
{

 public static void ClickCommand(IElement table, int rowNum, int colNum, string tagName, string cmdName) {
  var commandSave = GetCell(table, rowNum, colNum, tagName);
  Assert.Equal(cmdName, commandSave.TextContent);
  commandSave.Click();
 }

 public static void ClickCommandButtonByTitle(IElement table, int rowNum, int colNum, string tagName, string cmdName)
 {
  var commandSave = GetCell(table, rowNum, colNum, tagName);
  Assert.Equal(cmdName, (commandSave as IHtmlButtonElement).Title);
  commandSave.Click();
 }

 public static IElement GetCell(IElement table, int rowNum, int colNum, string tagName)
 {
  var commandColumn = table.Children[rowNum].Children[colNum];
  var commandEdit = commandColumn.GetElementsByTagName(tagName)[0];
  return commandEdit;
 }
}