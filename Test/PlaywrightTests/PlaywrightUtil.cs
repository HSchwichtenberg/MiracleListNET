using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlaywrightTests;

public class PlaywrightUtil
{

 /// <summary>
 /// Prüfe, ob alle Kinder (bis auf die letzten n) ein bestimmtes Tag sind
 /// Zugriff auf die Children ist leider umständlich in Playwright, siehe auch https://github.com/microsoft/playwright/issues/17703 und https://github.com/microsoft/playwright/issues/4845
 /// </summary>
 public static async Task VerifyChildren(IElementHandle parent, string expectedTag, int tail = 0)
 {
  Assert.IsTrue(await parent.EvaluateAsync<bool>($$"""
   (x) => {
    const children = Array.from(x.children);
    for (let i = 0; i < children.length - {{tail}}; i++) {
        if (children[i].tagName.toLowerCase() !== '{{expectedTag}}') {
            return false;
        }
    }
    return true;
   }
   """
));
 }

 /// <summary>
 /// Liefere letztes Kind-Tag
 /// Zugriff auf die Children ist leider umständlich in Playwright, siehe auch https://github.com/microsoft/playwright/issues/17703 und https://github.com/microsoft/playwright/issues/4845
 /// </summary>
 public static async Task<string> GetLastChildTag(IElementHandle parent)
 {
  return await parent.EvaluateAsync<string>(@"(x) => {
    const children = Array.from(x.children);
    const lastElement = children[children.length - 1];
    return lastElement.tagName.toLowerCase();
}");
 }
}
