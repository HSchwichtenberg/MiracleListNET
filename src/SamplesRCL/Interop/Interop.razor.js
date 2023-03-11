console.log("Interop.razor.js: Loading...");

// Variante 1: Es wird eine JavaScript-Methode realisiert, die diese Information sekündlich direkt in dem TextArea-Element ausgibt. 
export function SetContentInJS(element) {
 console.log("START SetContentInJS");
 var count = 0;
 myTimer();
 var myVar = setInterval(myTimer, 1000);

 function myTimer() {
  count++;
  var data = [];
  data.push("### Information from JavaScript");
  data.push("Counter: " + count);
  data.push("DateTime: " + new Date().toLocaleTimeString());
  data.push("Browser Type: " + navigator.appCodeName);
  data.push("Browser Version: " + navigator.appVersion);
  data.push("Window: " + window.innerWidth + "x" + window.innerHeight);
  data.push("Screen: " + screen.width + "x" + screen.height);
  element.value = data.join("\n"); // Parameter: Assembly, Methode, Daten
  console.log("Set textarea", data);
 }
}

// Variante 1b: bisher nicht im Buch: Variante 1 mit Callback zu .NET, um weitere Informationen zu sammeln
export function SetContentInJS2(element, assembly, method) {
 console.log("START SetContentInJS2");
 var count = 0;
 myTimer();
 var myVar = setInterval(myTimer, 1000);

 function myTimer() {
  count++;
  DotNet.invokeMethodAsync(assembly, method) // Parameter: Assembly, Methode
   .then(data => {
    data.push("### Information from JavaScript");
    data.push("Counter: " + count);
    data.push("DateTime: " + new Date().toLocaleTimeString());
    data.push("Browser Type: " + navigator.appCodeName);
    data.push("Browser Version: " + navigator.appVersion);
    element.value = data.join("\n");
   });
 }
}

// Variante 2: Es wird eine JavaScript-Methode realisiert, die diese Information sekündlich per Callback an .NET zurückliefert (über statische Methode)
export function SetContentInDOTNET(assembly, method) {
 console.log("START SetContentInDOTNET");
 var count = 0;
 myTimer();
 var myVar = setInterval(myTimer, 1000);

 function myTimer() {
  count++;
  var data = [];
  data.push("### Information from JavaScript");
  data.push("Counter: " + count);
  data.push("DateTime: " + new Date().toLocaleTimeString());
  data.push("Browser Type: " + navigator.appCodeName);
  data.push("Browser Version: " + navigator.appVersion);
  data.push("Window: " + window.innerWidth + "x" + window.innerHeight);
  data.push("Screen: " + screen.width + "x" + screen.height);

  // Callback to .NET
  DotNet.invokeMethodAsync(assembly, method, data.join("\n")); // Parameter: Assembly, Methode, Daten
  console.log("Data send to .NET", data);
 }
}

// Variante 2b: Es wird eine JavaScript-Methode realisiert, die diese Information sekündlich per Callback an .NET zurückliefert (über Instanzmethode)
export function SetContentInDOTNETInstance(dotnetHelper, methodname) {
 console.log("START SetContentInDOTNETInstance: " + typeof dotnetHelper);
 var count = 0;
 myTimer();
 var myVar = setInterval(myTimer, 1000);

 function myTimer() {
  count++;
  var data = [];
  data.push("### Information from JavaScript");
  data.push("Counter: " + count);
  data.push("DateTime: " + new Date().toLocaleTimeString());
  data.push("Browser Type: " + navigator.appCodeName);
  data.push("Browser Version: " + navigator.appVersion);
  data.push("Window: " + window.innerWidth + "x" + window.innerHeight);
  data.push("Screen: " + screen.width + "x" + screen.height);

  // Callback to .NET
  dotnetHelper.invokeMethodAsync(methodname, data.join("\n"));

  console.log("Data send to .NET", data);
 }
}


export function CreateRandomSentence(length) {
 var words = ["The sky", "above", "the port", "was", "the color of television", "tuned", "to", "a dead channel", ".", "All", "this happened", "more or less", ".", "I", "had", "the story", "bit by bit", "from various people", "and", "as generally", "happens", "in such cases", "each time", "it", "was", "a different story", ".", "It", "was", "a pleasure", "to", "burn"];
 var text = [];
 var x = length;
 while (--x) text.push(words[Math.floor(Math.random() * words.length)]);
 document.write(text.join(" "))
 return text;
}

export function CreateRandomLetters(length) {
 var result = '';
 var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
 var charactersLength = characters.length;
 for (var i = 0; i < length; i++) {
  result += characters.charAt(Math.floor(Math.random() *
   charactersLength));
 }
 return result;
}

// JS-Code for Streaming Sample: Must return ArrayBuffer!
export function GetTextAsStream(empty, len) {
 if (empty) {
  console.log("Sending " + len + "x random byte zero data from JS...");
  return new Uint8Array(len);
 }
 else {
  console.log("Sending " + len + "x random chars from JS...");
  var text = CreateRandomLetters(len);
  var enc = new TextEncoder();
  return enc.encode(text);
 }
}

// wird in Interop.razor aufgerufen
export function getName(message) {
 var name = prompt(message, '');
 alert("Hallo " + name + "!");
 return name;
}


export function FibonacciMany(repeat, start, end, useMemory) {
 const results = [];
 var count = 0;
 for (var j = 0; j < repeat; j++) {
  for (var i = start; i < end; i++) {
   var f = Fibonacci(i);
   count++;
   if (useMemory) results.push(f); // just for optional memory pressure ;-)
  }
 }
 return count;
}

export function Fibonacci(n) {
 var a = 0;
 var b = 1;
 for (var i = 0; i < n; i++) {
  var temp = a;
  a = b;
  b = temp + b;
 }
 return a;
}

console.log("Interop.razor.js: Loaded!");