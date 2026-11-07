export function getMessage(name) {
 return 'Hallo ' + name + ', JavaScript grüßt Dich!';
}

export async function setMessage() {
 var text = await DotNet.invokeMethodAsync("MiracleList_BW", "GetMessageFromDotnetClassic", "Holger");
 document.getElementById("JSOutput").innerText = text;
}

export async function setMessage2() {
 const { getAssemblyExports } = await globalThis.getDotnetRuntime(0);
 var exports = await getAssemblyExports("MiracleList_BW.dll");
 var text = exports.Web.Samples_BW.JSInterop.JSInteropBWOnly.GetMessageFromDotnetNew("Holger");
 document.getElementById("JSOutput").innerText = text;
}