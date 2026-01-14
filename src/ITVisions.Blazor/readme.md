Hilfskomponenten und Hilfsfunktionen für Blazor
Copyright © Dr. Holger Schwichtenberg, www.IT-Visions.de, 2001-2026

# Hinweis

Diese Bibliothek steht leider nicht auf NuGet.org zur Verfügung.

# Verwendung

DI Registration
services.AddScoped<BlazorUtil>();

DI Registration nur für Blazor WebAssembly
services.AddScoped<IHttpContextAccessor, WebAssemblyHttpContextAccessorDummy>();

index.html / _host.cshtml
<script src="/_content/ITVisions.Blazor/BlazorUtil.js"></script>

_Imports.razor
@using ITVisions.Blazor
@inject BlazorUtil util