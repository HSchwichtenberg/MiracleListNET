console.log("SamplesTS.js: Loading...");

export class TSUtil {

 public static TSFunction(): string {
  return "Hallo aus TypeScript der TypeScript-Klasse!!!"
 }

 public static  FibonacciMany(repeat, start, end, useMemory) {
  const results = [];
  var count = 0;
  for (var j = 0; j < repeat; j++) {
   for (var i = start; i < end; i++) {
    var f = this.Fibonacci(i);
    count++;
    if (useMemory) results.push(f); // just for optional memory pressure ;-)
   }
  }
  return count;
 }

 public static  Fibonacci(n) {
  var a = 0;
  var b = 1;
  for (var i = 0; i < n; i++) {
   var temp = a;
   a = b;
   b = temp + b;
  }
  return a;
 }

}

console.log("SamplesTS.js: Loaded!");