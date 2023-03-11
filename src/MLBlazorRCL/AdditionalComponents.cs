using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web;

/// <summary>
/// Hier können die Kopfprojekte zusätzliche Komponenten angeben, die nur einige Blazor-Varianten einblenden können
/// </summary>
public static class AdditionalComponents
{
 /// <summary>
 /// Wird eingefügt in Main.razor
 /// </summary>
 public static Type TaskExportAdditionalComponent;
 /// <summary>
 /// Wird eingefügt in TaskEdit.razor
 /// </summary
 public static Type TaskEditAdditionalComponent;
}
