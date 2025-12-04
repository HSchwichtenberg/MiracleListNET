 // DEMO: 23. JS-Interop komplex mit JS Isolation und Callback (JS)
// Modaler Bestätigungsdialog mit Bootstrap (Skript wird nachgeladen )
export function confirmBootstrap(dotnetObj, callbackMethodName, id, text, log = false) {
 if (log) console.log("confirmBootstrap", dotnetObj, callbackMethodName, id, text);
 // Setze Text
 $("#confirmModalText").html(text);
 // Binde Ereignis für Schaltfläche 1 ("Yes")
 $("#confirmModalText-btn-yes").on("click", function () {
  if (log) console.log("#confirmModalText-btn-si", id);
  $("#confirmModal").modal('hide');
  dotnetObj.invokeMethodAsync(callbackMethodName, id, true);
  $("#confirmModalText-btn-yes").off();
 });
 // Binde Ereignis für Schaltfläche 2 ("No")
 $("#confirmModalText-btn-no").on("click", function () {
  if (log) console.log("#confirmModalText-btn-no", id);
  $("#confirmModal").modal('hide');
  dotnetObj.invokeMethodAsync(callbackMethodName, id, false);
  $("#confirmModalText-btn-no").off();
 });
 // Zeige Dialog
 //console.log("Zeige Dialog...");
 $("#confirmModal").modal();
}

// nicht mehr verwendet
//export function confirmWithLog(text, log = false) {
// if (log) console.log("confirm: " + text);
// var e = confirm(text);
// if (log) console.log("confirm: " + text + "=" + e);
// return e;
//}