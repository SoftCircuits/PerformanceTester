// Copyright (c) 2020-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//

using SoftCircuits.PerformanceTester;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace TestPerformanceTester
{
    class Test1 : IPerformanceTest
    {
        public string Description => "Quarter second test";
        public void Initialize(object? data) { }
        public void Run(object? data) => Thread.Sleep(250);
    }

    class Test2 : IPerformanceTest
    {
        public string Description => "Half second test";
        public void Initialize(object? data) { }
        public void Run(object? data) => Thread.Sleep(500);
    }

    class Test3 : IPerformanceTest
    {
        public string Description => "Three quarter second test";
        public void Initialize(object? data) { }
        public void Run(object? data) => Thread.Sleep(750);
    }

    class Test4 : IPerformanceTest
    {
        public string Description => "One second test";
        public void Initialize(object? data) { }
        public void Run(object? data) => Thread.Sleep(1000);
    }

    class Program
    {
        static void Main(string[] args)
        {
            PerformanceTester tester = new PerformanceTester();

            Console.WriteLine("Standard Tests");
            ShowResults(tester.Run(Assembly.GetExecutingAssembly()));

            Console.WriteLine("Multiple Iteration Tests");
            ShowResults(tester.Run(Assembly.GetExecutingAssembly(), 5));

            Console.WriteLine("Multiple Iteration Tests with Averaged Results");
            tester.AverageResults = true;
            ShowResults(tester.Run(Assembly.GetExecutingAssembly(), 5));
        }

        // Display results
        static void ShowResults(IEnumerable<TestResult> results)
        {
            foreach (TestResult result in results)
            {
                Console.WriteLine("{0} ({1}ms/{2}%)",
                    result.Description,
                    result.Milliseconds,
                    result.Percent);
                Console.WriteLine("[{0}]", result.GetRelativePerformanceBar(60));
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
