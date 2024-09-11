console.log("SamplesTS.js: Loading...");
var TSUtil = (function () {
    function TSUtil() {
    }
    TSUtil.TSFunction = function () {
        return "Hallo aus TypeScript der TypeScript-Klasse!!!";
    };
    TSUtil.FibonacciMany = function (repeat, start, end, useMemory) {
        var results = [];
        var count = 0;
        for (var j = 0; j < repeat; j++) {
            for (var i = start; i < end; i++) {
                var f = this.Fibonacci(i);
                count++;
                if (useMemory)
                    results.push(f);
            }
        }
        return count;
    };
    TSUtil.Fibonacci = function (n) {
        var a = 0;
        var b = 1;
        for (var i = 0; i < n; i++) {
            var temp = a;
            a = b;
            b = temp + b;
        }
        return a;
    };
    return TSUtil;
}());
export { TSUtil };
console.log("SamplesTS.js: Loaded!");
//# sourceMappingURL=SamplesTS.js.map