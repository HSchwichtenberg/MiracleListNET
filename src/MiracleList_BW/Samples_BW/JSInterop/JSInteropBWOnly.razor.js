export function getMessage(name) {
 return 'Hallo ' + name + ', JavaScript grüßt Dich!';
}

export async function setMessage1() {
 var text = await DotNet.invokeMethodAsync("MiracleList_BW", "GetMessageFromDotnet1", "Holger");
 document.getElementById("JSOutput").innerText = text;
}

export async function setMessage2() {
 const { getAssemblyExports } = await globalThis.getDotnetRuntime(0);
 var exports = await getAssemblyExports("MiracleList_BW.dll");
 var text = exports.Web.Pages.JSInteropBWOnly.GetMessageFromDotnet2("Holger");
 document.getElementById("JSOutput").innerText = text;
}