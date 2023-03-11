
DI Registration
services.AddScoped<BlazorUtil>();

DI Registration nur für Blazor WebAssembly
services.AddScoped<IHttpContextAccessor, WebAssemblyHttpContextAccessorDummy>();

index.html / _host.cshtml
<script src="/_content/ITVisions.Blazor/BlazorUtil.js"></script>

Use in Components
@using ITVisions.Blazor
@inject BlazorUtil util 