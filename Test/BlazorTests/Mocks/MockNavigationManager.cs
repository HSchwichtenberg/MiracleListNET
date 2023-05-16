using System;
using Microsoft.AspNetCore.Components;

namespace BlazorTests.Mocks
{
 class MockNavigationManager : NavigationManager
 {

  const string DefaultStartUri = "http://server/test";
  Uri StartUri;
  string baseUri;

  public MockNavigationManager(string Uri = DefaultStartUri)
  {
   this.StartUri = new Uri(Uri);
  }

  protected override void EnsureInitialized()
  {
   // As described in the comment block above, BrowserNavigationManager is only for
   // client-side (Mono) use, so it's OK to rely on synchronicity here.
   baseUri = StartUri.Scheme + "://" + StartUri.Authority + "/";
   Initialize(baseUri, StartUri.ToString());
  }

  public void SetLocation(string uri, bool isInterceptedLink)
  {
   Uri = uri;
   NotifyLocationChanged(isInterceptedLink);
  }

  protected override void SetNavigationLockState(bool value)
  {
   value = false; // Navigation ist erlaubt!
  }

  /// <inheritdoc />
  protected override void NavigateToCore(string uri, bool forceLoad)
  {
   if (uri == null)
   {
    throw new ArgumentNullException(nameof(uri));
   }

   // combine URI with BaseUri
   this.Uri = new Uri(new Uri(baseUri), uri).ToString();
  }
 }
}