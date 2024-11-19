// Native Code in C, wird in NativeCall.razor verwendet
#include <time.h>
#include <stdlib.h>

int Fact(int n)
{
 if (n == 0) return 1;
 return n * Fact(n - 1);
}

int Fibonacci(int n, int* arr)
{
 int counter;
 long num1 = 1, num2 = 1, sum;
 for (counter = 0; counter < n; counter++) {
  arr[counter] = num1; 
  sum = num1 + num2;
  num1 = num2;
  num2 = sum;
 }
 return sum;
}

int GetRandomNumberList(int n, int* arr)
{
 int counter;
 int sum;
 srand(time(NULL));  
 for (counter = 0; counter < n; counter++) {
  arr[counter] = rand();  
  sum += arr[counter];
 }
 return sum;
}