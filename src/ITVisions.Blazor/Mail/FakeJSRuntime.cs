using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace ITVisions.Blazor.Mail;

/// <summary>
/// Wird z.B. verwendet, wenn eine Komponente für Mailrendering, wenn Komponente auch für UI verwendet wird
/// </summary>
public class FakeJSRuntime : IJSRuntime
{
 public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, object[] args)
 {
  throw new NotImplementedException();
 }

 public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, CancellationToken cancellationToken, object[] args)
 {
  throw new NotImplementedException();
 }
}
