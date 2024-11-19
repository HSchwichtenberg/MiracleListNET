//Für Blazor Server -, Blazor WebAssembly - und Blazor Hybrid - Apps:
export function beforeStart(options) {
 console.warn("ITVisions.Blazor JavaScript initializers beforeStart", options);
}
export function afterStarted(blazor) {
 console.warn("ITVisions.Blazor JavaScript initializers afterStarted", blazor);
}

// Blazor Web Apps
export function beforeWebStart(options) {
 console.warn("ITVisions.Blazor JavaScript initializers beforeWebStart", options);
}
export function afterWebStarted(blazor) {
 console.warn("ITVisions.Blazor JavaScript initializers afterWebStarted", blazor);
}

export function beforeServerStart(options) {
 console.warn("ITVisions.Blazor JavaScript initializers beforeServerStart", options);
}
export function afterServerStarted(blazor) {
 console.warn("ITVisions.Blazor JavaScript initializers afterServerStarted", blazor);
}

export function beforeWebAssemblyStart(options) {
 console.warn("ITVisions.Blazor JavaScript initializers beforeWebAssemblyStart", options);
}
export function afterWebAssemblyStarted(blazor) {
 console.warn("ITVisions.Blazor JavaScript initializers afterWebAssemblyStarted", blazor);
}
