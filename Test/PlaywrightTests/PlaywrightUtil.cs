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
 /// </summary>
 public static async Task VerifyChilddren(IElementHandle parent, string expectedTag, int tail = 0)
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
