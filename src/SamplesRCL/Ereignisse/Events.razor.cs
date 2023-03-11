using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reflection;
using System.Linq.Expressions;
using ITVisions.Blazor;

namespace Web
{
 public class EventsModel : ComponentBase
 {

  #region Standard-Events
  protected override void OnInitialized()
  {

  }

  protected override async System.Threading.Tasks.Task OnAfterRenderAsync(bool firstRender)
  {

  }
  #endregion

  #region Reaktionen auf Ereignisse
  //public void OnChange<TObject, TValue>
  // (ChangeEventArgs changeEventArgs, TObject target, Expression<Func<TObject, TValue>> propertyGetter)
  //{
  // if (changeEventArgs?.Value != null)
  // {
  //  var expression = (MemberExpression)propertyGetter.Body;
  //  var property = (PropertyInfo)expression.Member;
  //  property.SetValue(target, changeEventArgs.Value);
  // }
  //}
  #endregion
 }
}