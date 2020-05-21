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
        public void Run() => Thread.Sleep(250);
    }

    class Test2 : IPerformanceTest
    {
        public string Description => "Half second test";
        public void Run() => Thread.Sleep(500);
    }

    class Test3 : IPerformanceTest
    {
        public string Description => "Three quarter second test";
        public void Run() => Thread.Sleep(750);
    }

    class Test4 : IPerformanceTest
    {
        public string Description => "One second test";
        public void Run() => Thread.Sleep(1000);
    }

    class Program
    {
        static void Main(string[] args)
        {
            PerformanceTester tester = new PerformanceTester();
            IEnumerable<TestResult> results = tester.Run(Assembly.GetExecutingAssembly());

            foreach (TestResult result in results)
            {
                Console.WriteLine("{0} ({1}ms)", result.Description, result.Milliseconds);
                Console.WriteLine("[{0}]", result.GetRelativePerformanceBar(60));
                Console.WriteLine();
            }
        }
    }
}
