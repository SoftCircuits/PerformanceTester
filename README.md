# PerformanceTester

[![NuGet version (SoftCircuits.PerformanceTester)](https://img.shields.io/nuget/v/SoftCircuits.PerformanceTester.svg?style=flat-square)](https://www.nuget.org/packages/SoftCircuits.PerformanceTester/)

```
Install-Package SoftCircuits.PerformanceTester
```

## Introduction

PerformanceTester is a simple library to help compare the performance of different algorithms. If you have several ways of solving a problem where performance is a concern, use this library as a framework to quickly show the relative performance of different candidates.

## Using the Library

To use the library, create each candidate test as a class that implements the `IPerformanceTest1`. This interface has three members:

```cs
public string Description { get; }
```

A read-only property that describes this test.

```cs
public void Initialize();
```

A method that is called before the test to perform any initialization. This initialization is run before the time starts for this test.

```cs
public void Run();
```

Method that performs the actual test.

## Example

As an example, three simple tests are defined below. These tests simply call `Thread.Sleep()` to pause varying amounts of time.

```
class Test1 : IPerformanceTest
{
    public string Description => "Quarter second test";
    public void Initialize() { }
    public void Run() => Thread.Sleep(250);
}

class Test2 : IPerformanceTest
{
    public string Description => "Half second test";
    public void Initialize() { }
    public void Run() => Thread.Sleep(500);
}

class Test3 : IPerformanceTest
{
    public string Description => "One second test";
    public void Initialize() { }
    public void Run() => Thread.Sleep(1000);
}
```

To test the performance of these three tests, create an instance of the `PerformanceTester` class and call the `Run()` method. The `Run()` method accepts an `Assembly` argument. It will run all the tests in the given assembly.

```cs
PerformanceTester tester = new PerformanceTester();
IEnumerable<TestResult> results = tester.Run(Assembly.GetExecutingAssembly());
```

The following code could be used to show relative performance bars in a console application.

```cs
foreach (TestResult result in results)
{
    Console.WriteLine("{0} ({1}ms)", result.Description, result.Milliseconds);
    int length = (int)(result.Percent / 100.0 * MaxBarLength);
    Console.WriteLine($"[{new string('*', length),-MaxBarLength}]");
    Console.WriteLine();
}
```

The code above produces the following output.

```
Quarter second test (255ms)
[***************                                             ]

Half second test (500ms)
[*****************************                               ]

One second test (1001ms)
[************************************************************]
```
