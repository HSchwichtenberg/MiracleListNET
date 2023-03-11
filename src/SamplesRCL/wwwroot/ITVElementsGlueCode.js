console.log("Loading ITVElementsGlueCode.js...");

export function init(dotNet, element) {
 console.log("ITVElementsGlueCode.init");

 //oder Zugriff auf bestimmtes Element: var element = document.querySelector('angular-counter');
 element.addEventListener("changed", function (eventData) {
  // Behandlung des Ereignisses in JS
  var text = "angular-counter: Changed = " + eventData.detail;
  console.log('JS: ' + text, eventData);
  document.getElementById("ausgabeJS").innerText = text;

  // Weitergabe des Ereignisses an C#
  dotNet.invokeMethodAsync("NewValueArrived", eventData.detail)
   .then(data => {
    console.log("JS: Ereignis an .NET gesendet!");
   });
 });
}

/* DEMO: 17 <angular-grid> Glue Code */
export function initGrid(dotNet) {
 // finde das Grid
 var e = document.querySelector('angular-grid');
 // binde die Ereinisbehandlung
 e.addEventListener("selectedRowsChanged", function (eventData) {
  console.log('selected-Rows-Changed event fired!', eventData);
  // Behandlung des Ereignisses in JS, hier als Beispiel: direktes Ändern des DOM
  const selectedDataStringPresentation = eventData.detail.map(node => node.name + ' ' + node.alternateReality + ' ' + node.yearOfDeath).join(', ');
  document.getElementById("gridResultJS").innerText = selectedDataStringPresentation;
  // Weitergabe des Ereignisses an C# an die Methode NewSelection
  dotNet.invokeMethodAsync("NewSelection", eventData.detail)
   .then(data => {console.log("JS: Ereignis an .NET gesendet!");});
 });

}