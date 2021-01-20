# PerformanceTester

[![NuGet version (SoftCircuits.PerformanceTester)](https://img.shields.io/nuget/v/SoftCircuits.PerformanceTester.svg?style=flat-square)](https://www.nuget.org/packages/SoftCircuits.PerformanceTester/)

```
Install-Package SoftCircuits.PerformanceTester
```

## Introduction

PerformanceTester is a simple library to help compare the performance of different algorithms. If you have several ways of solving a problem where performance is a concern, use this library as a framework to quickly show the relative performance of different candidates.

## Creating Tests

You create a test by creating a class that implements `IPerformanceTest`. This interface has two members:

```cs
public string Description { get; }
```

`Description` is a read-only property that describes this test.

```cs
public void Run(object data);
```

`Run` is the method that performs the actual test. This is the method that will be timed. The `data` argument can optionally be used to pass data to the test.

The following code defines three simple tests. These tests simply call `Thread.Sleep()` to pause for varying amounts of time.

```cs
class Test1 : IPerformanceTest
{
    public string Description => "Quarter second test";
    public void Run(object data) => Thread.Sleep(250);
}

class Test2 : IPerformanceTest
{
    public string Description => "Half second test";
    public void Run(object data) => Thread.Sleep(500);
}

class Test3 : IPerformanceTest
{
    public string Description => "One second test";
    public void Run(object data) => Thread.Sleep(1000);
}
```

## Running the Tests

To test the performance of the tests you've created, create an instance of the `PerformanceTester` class and call the `Run()` method. This method has many overloads to support different scenarios. Often, the easiest way to call this method is by providing the current assembly. In this case, the `Run()` method will run all the tests (classes that implement `IPerformanceTest`) in the given assembly.

```cs
PerformanceTester tester = new PerformanceTester();
IEnumerable<TestResult> results = tester.Run(Assembly.GetExecutingAssembly());
```

Most of the `Run()` overloads include an optional `iterations` and `data` argument.

The `iterations` argument specifies how many times to run each test. This can be handy when the test is too quick to measure, or if the test execution time can vary and you want to compare the average time to perform each test.

The `data` argument is simply an argument that gets pass to each test. You might using for sending test data, for example. Because this argument is of type `object`, it can pass any type of data.

## Displaying Test Results

The `Run()` methods return an `IEnumerable` of type `TestResult`. The `TestResult` class provides the description of the test, the number of milliseconds taken by that test, and also the percent of time taken relative to the slowest test.

The `TestResult` class also provides the `GetRelativePerformanceBar()` method. This method will create a character *bar* that represents the result's relative time. This is demonstrated in the following snippet.

```cs
foreach (TestResult result in results)
{
    Console.WriteLine("{0} ({1}ms)", result.Description, result.Milliseconds);
    Console.WriteLine("[{0}]", result.GetRelativePerformanceBar(60));
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
