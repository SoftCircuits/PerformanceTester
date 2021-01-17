// Copyright (c) 2020 Jonathan Wood (www.softcircuits.com)
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
        public void Run(object data) => Thread.Sleep(250);
    }

    class Test2 : IPerformanceTest
    {
        public string Description => "Half second test";
        public void Run(object data) => Thread.Sleep(500);
    }

    class Test3 : IPerformanceTest
    {
        public string Description => "Three quarter second test";
        public void Run(object data) => Thread.Sleep(750);
    }

    class Test4 : IPerformanceTest
    {
        public string Description => "One second test";
        public void Run(object data) => Thread.Sleep(1000);
    }

    class IterationTest : IPerformanceTest
    {
        private int Counter = 0;

        public string Description => $"Iteration Test (Counter = {Counter})";

        public void Run(object data)
        {
            Counter++;
            Thread.Sleep(100);
        }
    }

    class DataTest : IPerformanceTest
    {
        private object Data = null;
        public string Description => $"Data Test (Data = {Data?.ToString() ?? "NULL"})";

        public void Run(object data)
        {
            Data = data;
            Thread.Sleep(100);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            PerformanceTester tester = new PerformanceTester();
            IEnumerable<TestResult> results = tester.Run(Assembly.GetExecutingAssembly());

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

            IPerformanceTest test = new IterationTest();
            tester.Run(test, 5);
            Console.WriteLine(test.Description);
            test = new DataTest();
            tester.Run(test, 1, "Sample Data!");
            Console.WriteLine(test.Description);
        }
    }
}
